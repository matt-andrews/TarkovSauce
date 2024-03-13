using System.Diagnostics;
using System.Timers;
using TarkovSauce.Watcher.Interfaces;

namespace TarkovSauce.Watcher
{
    public interface IMonitor
    {
        void ChangePath(string path);
    }
    internal class Monitor : IWatcherEventListener, IMonitor
    {
        private Process? _process;
        private readonly Dictionary<string, Watcher> _watchers = [];
        private MessageFactory? _messageFactory;
        private readonly MonitorOptions _options;
        private readonly System.Timers.Timer _processTimer;
        private FileSystemWatcher _logFileCreateWatcher;

        public Monitor(MonitorOptions options)
        {
            _options = options;
            _logFileCreateWatcher = new FileSystemWatcher
            {
                Filter = "*.log",
                IncludeSubdirectories = true,
            };
            _processTimer = new System.Timers.Timer(TimeSpan.FromSeconds(30).TotalMilliseconds)
            {
                AutoReset = true,
                Enabled = false
            };
        }

        public void RegisterListeners(List<IMessageListener> listeners)
        {
            _messageFactory = new(listeners);
        }

        public Monitor Start()
        {
            _logFileCreateWatcher.Path = _options.LogPath;
            _logFileCreateWatcher.Created += LogFileCreateWatcher_Created;
            _logFileCreateWatcher.EnableRaisingEvents = true;
            _processTimer.Elapsed += ProcessTimer_Elapsed;
            _processTimer.Start();
            UpdateProcess();
            WatchFolders();
            return this;
        }

        public void ChangePath(string path)
        {
            foreach(var watcher in _watchers)
            {
                watcher.Value.Stop();
            }
            _watchers.Clear();
            _options.LogPath = path;
            _logFileCreateWatcher = new FileSystemWatcher
            {
                Filter = "*.log",
                IncludeSubdirectories = true,
            };
            Start();
        }

        private void WatchFolders()
        {
            string folder = new FolderFinder(_options.LogPath).GetLatestLogFolder();
            var files = Directory.GetFiles(folder);
            foreach (var file in files)
            {
                WatchFile(file);
            }
        }

        private void WatchFile(string file)
        {
            if (string.IsNullOrWhiteSpace(file))
                return;

            string? name = _options.Files.FirstOrDefault(f => file.Contains(f.File))?.Name;

            if (string.IsNullOrWhiteSpace(name))
                return;

            if (_watchers.TryGetValue(name, out Watcher? watcher))
            {
                watcher.Stop();
                _watchers.Remove(name);
            }

            watcher = new Watcher(file, name, this);
            _ = watcher.Start();
            _watchers.Add(name, watcher);
        }

        private void UpdateProcess()
        {
            try
            {
                if (_process != null)
                {
                    if (!_process.HasExited)
                    {
                        return;
                    }
                    _process = null;
                }
                var processes = Process.GetProcessesByName("EscapeFromTarkov");
                if (processes.Length == 0)
                {
                    _process = null;
                    return;
                }
                _process = processes.First();

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void ProcessTimer_Elapsed(object? sender, ElapsedEventArgs e)
        {
            UpdateProcess();
        }

        private void LogFileCreateWatcher_Created(object sender, FileSystemEventArgs e)
        {
            WatchFile(e.FullPath);
        }

        public void OnMessage(string name, string message)
        {
            _messageFactory?.OnMessage(name, message);
        }

        public void OnError(string name, string message, Exception ex)
        {
            _messageFactory?.OnError(name, message, ex);
        }
    }
}
