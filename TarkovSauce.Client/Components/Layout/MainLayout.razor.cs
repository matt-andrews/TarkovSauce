using Microsoft.AspNetCore.Components;
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
        private void LaunchApp()
        {
            if (!string.IsNullOrEmpty(AppDataJson.Settings.TarkovExePath))
            {
                // Use Process.Start to launch the Tarkov application
                System.Diagnostics.Process.Start(AppDataJson.Settings.TarkovExePath);
            }
        }
    }
}
