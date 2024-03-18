using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Timers;
using TarkovSauce.Watcher.Interfaces;

namespace TarkovSauce.Watcher
{
    public interface IMonitor
    {
        bool IsLoading { get; }
        event Action<bool>? IsLoadingChanged;
        void ChangePath(string path);
        void GoToCheckpoint();
    }
    internal class Monitor : IWatcherEventListener, IMonitor
    {
        public bool IsLoading { get; private set; }
        public event Action<bool>? IsLoadingChanged;
        private Process? _process;
        private readonly Dictionary<string, Watcher> _watchers = [];
        private MessageFactory? _messageFactory;
        private readonly MonitorOptions _options;
        private readonly System.Timers.Timer _processTimer;
        private FileSystemWatcher _logFileCreateWatcher;
        private readonly FileSystemWatcher _screenshotWatcher;
        private readonly string _screenshotPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Escape From Tarkov", "Screenshots");

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
            _screenshotWatcher = new FileSystemWatcher();
        }

        public void RegisterListeners(List<IMessageListener> listeners)
        {
            _messageFactory = new(listeners, _options.CheckpointPath);
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
            SetupScreenshotWatcher();
            return this;
        }

        public void ChangePath(string path)
        {
            foreach (var watcher in _watchers)
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

        public void GoToCheckpoint()
        {
            if (_messageFactory is null)
                throw new Exception("Cannot go to checkpoint before listeners have been assigned");
            IsLoading = true;
            IsLoadingChanged?.Invoke(IsLoading);
            DateTime checkpoint = DateTime.MinValue;
            if (File.Exists(_options.CheckpointPath))
            {
                string txt = File.ReadAllText(_options.CheckpointPath);
                _ = DateTime.TryParse(txt, out checkpoint);
            }
            var folders = new FolderFinder(_options.LogPath).GetLogFoldersNewerThan(checkpoint);
            foreach (var folder in folders)
            {
                var files = Directory.GetFiles(folder);
                foreach (var file in files)
                {
                    string? name = _options.Files.FirstOrDefault(f => file.Contains(f.File))?.Name;
                    if (string.IsNullOrWhiteSpace(name))
                        continue;
                    string[] contents = File.ReadAllLines(file);
                    int index = 0;
                    foreach (var line in contents)
                    {
                        string left = line.Split('|')[0];
                        if (DateTime.TryParse(left, out DateTime linedt) && linedt >= checkpoint)
                        {
                            break;
                        }
                        index++;
                    }
                    _messageFactory?.OnMessage(name, string.Join(Environment.NewLine, contents.Skip(index)));
                }
            }
            IsLoading = false;
            IsLoadingChanged?.Invoke(IsLoading);
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

        private void SetupScreenshotWatcher()
        {
            try
            {
                bool screenshotPathExists = Directory.Exists(_screenshotPath);
                string watchPath = screenshotPathExists ? _screenshotPath : Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                _screenshotWatcher.Path = watchPath;
                _screenshotWatcher.IncludeSubdirectories = !screenshotPathExists;
                _screenshotWatcher.Created -= ScreenshotWatcher_Created;
                _screenshotWatcher.Created -= ScreenshotWatcher_FolderCreated;
                if (screenshotPathExists)
                {
                    _screenshotWatcher.Filter = "*.png";
                    _screenshotWatcher.Created += ScreenshotWatcher_Created;
                }
                else
                {
                    _screenshotWatcher.Created += ScreenshotWatcher_FolderCreated;
                }
                _screenshotWatcher.EnableRaisingEvents = true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private void ScreenshotWatcher_FolderCreated(object sender, FileSystemEventArgs e)
        {
            if (e.FullPath == _screenshotPath)
            {
                SetupScreenshotWatcher();
            }
        }
        private void ScreenshotWatcher_Created(object sender, FileSystemEventArgs e)
        {
            string filename = e.Name ?? "";
            _messageFactory?.OnScreenshot(filename);
        }
    }
}
