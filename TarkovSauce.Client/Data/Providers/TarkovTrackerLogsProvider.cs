namespace TarkovSauce.Client.Data.Providers
{
    public interface ITarkovTrackerLogsProvider : IProvider
    {
        List<string> Logs { get; }
        void Log(string message);
    }
    internal class TarkovTrackerLogsProvider : ITarkovTrackerLogsProvider
    {
        public Action? OnStateChanged { get; set; }
        public List<string> Logs { get; } = [];
        public void Log(string message)
        {

        }
    }
}
