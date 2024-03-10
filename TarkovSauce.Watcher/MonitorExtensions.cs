using Microsoft.Extensions.DependencyInjection;
using TarkovSauce.Watcher.Interfaces;

namespace TarkovSauce.Watcher
{
    public static class MonitorExtensions
    {
        public static IServiceCollection AddWatcher(this IServiceCollection services, Action<IMonitorOptions> optionsBuilder)
        {
            MonitorOptions options = new();
            optionsBuilder(options);
            services.AddSingleton(new Monitor(options).Start());
            return services;
        }
        public static void UseWatcher(this IServiceProvider services, Action<IServiceProvider, List<IMessageListener>> listenersBuilder)
        {
            var monitor = services.GetRequiredService<Monitor>();
            List<IMessageListener> listeners = [];
            listenersBuilder(services, listeners);
            monitor.RegisterListeners(listeners);
        }
    }
    public interface IMonitorOptions
    {
        string LogPath { get; set; }
        void AddFile(WatcherFile file);
    }
    internal class MonitorOptions : IMonitorOptions
    {
        public string LogPath { get; set; } = "";//"C:\\Battlestate Games\\Escape from Tarkov\\Logs"
        public List<WatcherFile> Files { get; set; } = [];

        public void AddFile(WatcherFile file)
        {
            Files.Add(file);
        }
    }
    public class WatcherFile(string name, string file)
    {
        public string Name { get; set; } = name;
        public string File { get; set; } = file;
    }
}
