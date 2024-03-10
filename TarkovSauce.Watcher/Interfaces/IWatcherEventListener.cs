namespace TarkovSauce.Watcher.Interfaces
{
    public interface IWatcherEventListener
    {
        void OnMessage(string name, string message);
        void OnError(string name, string message, Exception ex);
    }
}
