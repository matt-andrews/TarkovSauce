using System.Text;
using TarkovSauce.Watcher.Interfaces;

namespace TarkovSauce.Watcher
{
    internal class Watcher(string _path, string _name, IWatcherEventListener _listener)
    {
        private readonly int _maxBufferLength = 1024;
        private bool _cancel = false;

        public async Task Start(bool readFromStart = false)
        {
            long fileBytesRead = 0;
            if (!readFromStart)
            {
                try
                {
                    fileBytesRead = new FileInfo(_path).Length;
                }
                catch (Exception ex)
                {
                    _listener.OnError(_name, $"getting initial {_name} log data size", ex);
                    await Task.Delay(5000);
                    _ = Start();
                    return;
                }
            }

            while (true)
            {
                if (_cancel) break;
                try
                {
                    var fileSize = new FileInfo(_path).Length;
                    if (fileSize > fileBytesRead)
                    {
                        using var fs = new FileStream(_path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                        fs.Seek(fileBytesRead, SeekOrigin.Begin);
                        var buffer = new byte[_maxBufferLength];
                        var chunks = new List<string>();
                        var bytesRead = fs.Read(buffer, 0, buffer.Length);
                        var newBytesRead = 0;
                        while (bytesRead > 0)
                        {
                            newBytesRead += bytesRead;
                            var text = Encoding.UTF8.GetString(buffer, 0, bytesRead);

                            chunks.Add(text);
                            bytesRead = fs.Read(buffer, 0, buffer.Length);
                        }
                        _listener.OnMessage(_name, string.Join("", [.. chunks]));
                        fileBytesRead += newBytesRead;
                    }
                }
                catch (Exception ex)
                {
                    _listener.OnError(_name, $"reading {_name} log data", ex);
                }

                await Task.Delay(5000);
            }
        }
        public void Stop()
        {
            _cancel = true;
        }
    }
}

