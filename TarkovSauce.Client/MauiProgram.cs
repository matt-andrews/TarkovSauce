﻿using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using TarkovSauce.Client.Components;
using TarkovSauce.Client.Data.MessageListeners;
using TarkovSauce.Client.Data.Providers;
using TarkovSauce.Client.HttpClients;
using TarkovSauce.Client.Services;
using TarkovSauce.Client.Utils;
using TarkovSauce.MapTools;
using TarkovSauce.Watcher;
using Ripe.Sdk.DependencyInjection;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace TarkovSauce.Client
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            try
            {
                var builder = MauiApp.CreateBuilder();
                builder
                    .UseMauiApp<App>()
                    .ConfigureFonts(fonts =>
                    {
                        fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    });

                using var loggerFactory = LoggerFactory.Create(loggingBuilder => loggingBuilder
                    .SetMinimumLevel(LogLevel.Trace)
                    .AddDebug());

                AppDataManager.GetVersion();

                if (!Environment.GetCommandLineArgs().Contains("--dev") && !Environment.GetCommandLineArgs().Contains("--launcher"))
                {
                    throw new ApplicationException("Running Tarkov Sauce without the launcher is not supported");
                }

                // Make statecontainer a thing
                var stateContainer = new StateContainer();
                var appData = new AppDataJson();
                var configuration = new ConfigurationBuilder()
                    .AddJsonFile(AppDataManager.SettingsFile, true, true)
                    .AddRipe(builder.Services, (httpClient, options) =>
                    {
                        var setup = JsonSerializer.Deserialize<JsonObject>(Convert.FromBase64String(File.ReadAllText("ripe.sh")))
                            ?? throw new JsonException("Failed to pull setup file");
                        options.Uri = setup["Uri"]?.ToString();
                        options.ApiKey = setup["ApiKey"]?.ToString();
                        options.Version = AppDataManager.Version;
                    }, out RipeConfig config)
                    .Build();
                configuration.Bind(appData);
                builder.Configuration.AddConfiguration(configuration);

                ChangeToken.OnChange(() => configuration.GetReloadToken(), () =>
                {
                    configuration.Bind(appData);
                    stateContainer.MainLayoutHasChanged();
                });

                builder.Services.AddSingleton(appData);
                builder.Services.AddTSComponentServices();
                builder.Services.AddSingleton(stateContainer);

                var sqlService = new SqlService(AppDataManager.DatabaseFile, loggerFactory.CreateLogger<SqlService>());
                var appDataManager = new AppDataManager();
                var tarkovDevHttpClient = new TarkovDevHttpClient(new HttpClient()
                {
                    BaseAddress = new Uri(config.Client.TarkovDevApi)
                }, sqlService);

                builder.Services.AddSingleton<IAppDataManager>(appDataManager);
                builder.Services.AddSingleton<ITarkovDevHttpClient>(provider => tarkovDevHttpClient);
                builder.Services.AddSingleton<ITarkovTrackerHttpClient>(provider
                    => new TarkovTrackerHttpClient(new HttpClient()
                    {
                        BaseAddress = new Uri(config.Client.TarkovTrackerApi)
                    },
                        provider.GetRequiredService<IConfiguration>(),
                        provider.GetRequiredService<ILogger<TarkovTrackerHttpClient>>()));

                builder.Services.AddSingleton<ISqlService>(sqlService);
                builder.Services.AddSingleton<ITasksService, TasksService>();

                builder.Services.AddSingleton<IRawLogProvider, RawLogProvider>();
                builder.Services.AddSingleton<IFleaSalesProvider, FleaSalesProvider>();
                builder.Services.AddSingleton<ITarkovTrackerProvider, TarkovTrackerProvider>();
                builder.Services.AddSingleton<ISelectedMapProvider, SelectedMapProvider>();
                builder.Services.AddSingleton<ITarkovTrackerLogsProvider, TarkovTrackerLogsProvider>();
                builder.Services.AddSingleton<IScreenshotWatcherProvider, ScreenshotWatcherProvider>();

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
                        string? launcher = builder.Configuration["Settings:TarkovExePath"];
                        if (string.IsNullOrWhiteSpace(launcher))
                        {
                            launcher = RegistryFinder.GetBsgLauncherLocation();
                            var appdata = appDataManager.GetAppData();
                            appdata.Settings.TarkovExePath = launcher;
                            appDataManager.WriteAppData(appdata);
                        }
                        options.LogPath = path;
                        options.CheckpointPath = AppDataManager.CheckpointFile;
                        options.AddFile(new WatcherFile("application", "application.log"));
                        options.AddFile(new WatcherFile("notifications", "notifications.log"));
                    });

                builder.Services
                    .AddMapTools(config.Client.BlobSource)
                    .AddMap(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "customs.json"))
                    .AddMap(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "woods.json"))
                    .AddMap(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "shoreline.json"))
                    .AddMap(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "streets.json"))
                    .AddMap(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "lighthouse.json"))
                    .AddMap(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "reserve.json"))
                    .AddMap(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "labs.json"))
                    .AddMap(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "interchange.json"))
                    .AddMap(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "factory.json"))
                    .AddMap(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ground-zero.json"))
                    ;

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
            catch (Exception ex)
            {
                File.WriteAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"dump.{DateTime.Now:yyyyMMdd-HHmmssffff}.log"),
                    JsonSerializer.Serialize(
                    new
                    {
                        ex.Message,
                        ex.StackTrace,
                        Full = ex.ToString()
                    }));
                throw;
            }
        }
    }
}
