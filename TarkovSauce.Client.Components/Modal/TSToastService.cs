namespace TarkovSauce.Client.Components.Modal
{
    public interface ITSToastService
    {
        void Toast(string message, ToastType type);
    }
    internal class TSToastService : ITSToastService
    {
        public event Action<string, ToastType>? OnEvent;
        public void Toast(string message, ToastType type)
        {
            OnEvent?.Invoke(message, type);
        }
    }
    public enum ToastType
    {
        Success,
        Error,
        Warning
    }
}
