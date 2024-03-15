using Microsoft.AspNetCore.Components;
using System.Diagnostics;
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
        private async Task LaunchApp()
        {
            if (!string.IsNullOrEmpty(AppDataJson.Settings.TarkovExePath))
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
                _launchAppIsLoading = false;
            }
        }
    }
}
