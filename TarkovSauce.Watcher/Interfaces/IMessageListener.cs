namespace TarkovSauce.Watcher.Interfaces
{
    public interface IMessageListener
    {
        string Match { get; }
        void OnEvent(string str);
    }
}
