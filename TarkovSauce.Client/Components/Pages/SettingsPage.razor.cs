using Microsoft.AspNetCore.Components;
using TarkovSauce.Client.Components.Modal;
using TarkovSauce.Client.Services;
using TarkovSauce.Client.Utils;
using TarkovSauce.Watcher;

namespace TarkovSauce.Client.Components.Pages
{
    public partial class SettingsPage
    {
        [Inject]
        public IAppDataManager AppDataManager { get; set; } = default!;
        [Inject]
        public ITSToastService ToastService { get; set; } = default!;
        [Inject]
        public IMonitor Monitor { get; set; } = default!;
        [Inject]
        public StateContainer StateContainer { get; set; } = default!;
        [Inject]
        public AppDataJson AppDataJson { get; set; } = default!;
        private string _previousLogPath = "";
        protected override void OnInitialized()
        {
         
            _previousLogPath = AppDataJson.Settings.TarkovPath;
        }
        private void OnSaveEvent()
        {
            if (AppDataJson is null)
            {
                return;
            }

            AppDataManager.WriteAppData(AppDataJson);
            if (AppDataJson.Settings.TarkovPath != _previousLogPath)
            {
                Monitor.ChangePath(AppDataJson.Settings.TarkovPath);
            }
            ToastService.Toast("Your settings have been saved", ToastType.Success);
        }
    }
}
