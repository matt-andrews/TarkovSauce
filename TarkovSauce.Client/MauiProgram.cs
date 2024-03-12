using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using TarkovSauce.Client.Components;
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

            builder.Configuration.AddJsonFile(AppDataManager.SettingsFile, true, true);

            builder.Services.AddTSComponentServices();
            builder.Services.AddSingleton<StateContainer>();

            var appDataManager = new AppDataManager();
            builder.Services.AddSingleton<IAppDataManager>(appDataManager);

            var tarkovDevHttpClient = new TarkovDevHttpClient(new HttpClient()
            {
                BaseAddress = new Uri("https://api.tarkov.dev")
            });
            builder.Services.AddSingleton<ITarkovDevHttpClient>(provider => tarkovDevHttpClient);
            builder.Services.AddSingleton<ITarkovTrackerHttpClient>(provider
                => new TarkovTrackerHttpClient(new HttpClient(),
                    provider.GetRequiredService<IConfiguration>(),
                    provider.GetRequiredService<ILogger<TarkovTrackerHttpClient>>()));


            builder.Services.AddSingleton<IRawLogProvider, RawLogProvider>();
            builder.Services.AddSingleton<IFleaSalesProvider, FleaSalesProvider>();
            builder.Services.AddSingleton<ITarkovTrackerProvider, TarkovTrackerProvider>();

            builder.Services
                .AddWatcher(options =>
                {
                    string? path = builder.Configuration["Settings:TarkovPath"];
                    if (string.IsNullOrWhiteSpace(path))
                    {
                        path = RegistryFinder.GetTarkovLogsLocation();
                        var appdata = appDataManager.GetAppData();
                        appdata.Settings.TarkovPath = path;
                        appDataManager.WriteAppData(appdata);
                    }
                    options.LogPath = path;
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
