namespace TarkovSauce.Client.Data.Providers
{
    public interface IRawLogProvider : IProvider
    {
        List<string> RawLogs { get; }
        void AppendLog(string message);
    }
    public class RawLogProvider : IRawLogProvider
    {
        public Action? OnStateChanged { get; set; }
        public List<string> RawLogs { get; } = [];
        public void AppendLog(string message)
        {
            RawLogs.Add(message);
            OnStateChanged?.Invoke();
        }
    }
}
