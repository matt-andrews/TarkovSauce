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
        private AppDataJson? _appData;
        private string _previousLogPath = "";
        protected override void OnInitialized()
        {
            _appData = AppDataManager.GetAppData();
            _previousLogPath = _appData.Settings.TarkovPath;
        }
        private void OnSaveEvent()
        {
            if (_appData is null)
            {
                return;
            }

            AppDataManager.WriteAppData(_appData);
            if (_appData.Settings.TarkovPath != _previousLogPath)
            {
                Monitor.ChangePath(_appData.Settings.TarkovPath);
            }
            ToastService.Toast("Your settings have been saved", ToastType.Success);
        }
    }
}
