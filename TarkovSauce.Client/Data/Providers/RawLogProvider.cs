namespace TarkovSauce.Client.Data.Providers
{
    public interface IRawLogProvider : IProvider
    {
        Queue<string> RawLogs { get; }
        void AppendLog(string message);
    }
    public class RawLogProvider : IRawLogProvider
    {
        public Action? OnStateChanged { get; set; }
        public Queue<string> RawLogs { get; } = [];
        public void AppendLog(string message)
        {
            if (RawLogs.Count > 1000)
                RawLogs.Dequeue();
            RawLogs.Enqueue(message);
            OnStateChanged?.Invoke();
        }
    }
}
