using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using TarkovSauce.Client.Components.Modal;
using TarkovSauce.Client.Services;
using TarkovSauce.Client.Utils;
using TarkovSauce.Watcher;

namespace TarkovSauce.Client.Components.Layout
{
    public partial class MainLayout
    {
        [Inject]
        public StateContainer StateContainer { get; set; } = default!;
        [Inject]
        public AppDataJson AppDataJson { get; set; } = default!;
        [Inject]
        public IMonitor Monitor { get; set; } = default!;
        [Inject]
        public ITSToastService ToastService { get; set; } = default!;
        [Inject]
        public ILogger<MainLayout> Logger { get; set; } = default!;
        private readonly string[] _tabButtons = AppDataManager.IsDev
            ? ["Component Tests", "Home", "Logs", "Tasks", "Flea Sales", "Raid"]
            : ["Home", "Logs", "Tasks", "Flea Sales", "Raid"];
        private string _tabSelection = AppDataManager.IsDev
            ? "Component Tests"
            : "Home";
        private bool _launchAppIsLoading;
        protected override void OnInitialized()
        {
            StateContainer.IsLoading.OnValueChanged += b => InvokeAsync(StateHasChanged);
            StateContainer.MainLayoutChangedEvent += () => InvokeAsync(StateHasChanged);
            Monitor.IsLoadingChanged += b =>
            {
                StateContainer.IsLoading.Value = b;
            };
            Task.Run(() =>
            {
                Monitor.GoToCheckpoint();
            });
        }
        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
        }
        private void OnTabSelection(string selection)
        {
            _tabSelection = selection;
            switch (selection)
            {
                case "Component Tests":
                    StateContainer.State.Value = State.ComponentTests;
                    break;
                case "Logs":
                    StateContainer.State.Value = State.Logs;
                    break;
                case "Tasks":
                    StateContainer.State.Value = State.Tasks;
                    break;
                case "Flea Sales":
                    StateContainer.State.Value = State.FleaSales;
                    break;
                case "Raid":
                    StateContainer.State.Value = State.Raid;
                    break;
                case "Home":
                    StateContainer.State.Value = State.Home;
                    break;
            }
        }
        private string GetPageTitle()
        {
            if (StateContainer.State.Value == State.ComponentTests)
                return "Component Tests";
            else if (StateContainer.State.Value == State.FleaSales)
                return "Flea Sales";
            return StateContainer.State.Value.ToString();
        }
        private async Task LaunchApp()
        {
            if (!string.IsNullOrEmpty(AppDataJson.Settings.TarkovExePath))
            {
                try
                {
                    // Check for launcher already started so we can skip the loading button
                    Process[] processes = Process.GetProcessesByName("BsgLauncher");
                    _launchAppIsLoading = true;
                    // Use Process.Start to launch the Tarkov application
                    Process process = new()
                    {
                        StartInfo = new ProcessStartInfo()
                        {
                            FileName = AppDataJson.Settings.TarkovExePath
                        }
                    };
                    process.Start();
                    if (processes.Length <= 0)
                    {
                        // Wait for the bsg launcher to open so we can turn off the loading spinner
                        while (process.MainWindowTitle != "BsgLauncher")
                        {
                            process.Refresh();
                            await Task.Delay(150);
                        }
                    }
                }
                catch (Exception ex)
                {
                    ToastService.Toast(ex.Message, ToastType.Error);
                    Logger.LogError(ex, ex.Message);
                }
                _launchAppIsLoading = false;
            }
        }
    }
}
