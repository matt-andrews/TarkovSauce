using Microsoft.Extensions.Logging;
using TarkovSauce.Client.Data.MessageListeners;
using TarkovSauce.Client.Data.Providers;
using TarkovSauce.Client.HttpClients;
using TarkovSauce.Client.Services;
using TarkovSauce.Client.Utils;
using TarkovSauce.Watcher;

namespace TarkovSauce.Client
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                });

            builder.Services.AddSingleton<StateContainer>();

            var tarkovDevHttpClient = new TarkovDevHttpClient(new HttpClient()
            {
                BaseAddress = new Uri("https://api.tarkov.dev")
            });
            builder.Services.AddSingleton<ITarkovDevHttpClient>(provider => tarkovDevHttpClient);

            builder.Services.AddSingleton<IRawLogProvider, RawLogProvider>();
            builder.Services.AddSingleton<IFleaSalesProvider, FleaSalesProvider>();

            builder.Services
                .AddWatcher(options =>
                {
                    options.LogPath = RegistryFinder.GetTarkovLogsLocation();
                    options.AddFile(new WatcherFile("application", "application.log"));
                    options.AddFile(new WatcherFile("notifications", "notifications.log"));
                });

            builder.Services.AddMauiBlazorWebView();

#if DEBUG
            builder.Services.AddBlazorWebViewDeveloperTools();
            builder.Logging.AddDebug();
#endif

            var app = builder.Build();

            app.Services.UseWatcher((provider, listeners) =>
            {
                string targetNamespace = "TarkovSauce.Client.Data.MessageListeners";
                var types = typeof(MauiProgram).Assembly.GetTypes()
                    .Where(w => string.Equals(w.Namespace, targetNamespace, StringComparison.OrdinalIgnoreCase))
                    .ToArray();
                foreach (var type in types)
                {
                    if (type.IsSubclassOf(typeof(BaseMessageListener)))
                    {
                        List<object> parameters = [];
                        foreach (var parameter in type.GetConstructors().First().GetParameters())
                        {
                            parameters.Add(provider.GetRequiredService(parameter.ParameterType));
                        }
                        var obj = Activator.CreateInstance(type, [.. parameters]) as BaseMessageListener;
                        if (obj is not null)
                        {
                            listeners.Add(obj);
                        }
                    }
                }
            });

            return app;
        }
    }
}
