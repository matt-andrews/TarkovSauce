namespace TarkovSauce.Client.Data.Providers
{
    public interface ITarkovTrackerProvider : IProvider
    {

    }
    public class TarkovTrackerProvider : ITarkovTrackerProvider
    {
        public Action? OnStateChanged { get; set; }
    }
}
