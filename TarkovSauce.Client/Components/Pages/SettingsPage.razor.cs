using Microsoft.AspNetCore.Components;
using TarkovSauce.Client.Components.Modal;
using TarkovSauce.Client.Utils;

namespace TarkovSauce.Client.Components.Pages
{
    public partial class SettingsPage
    {
        [Inject]
        public IAppDataManager AppDataManager { get; set; } = default!;
        [Inject]
        public ITSToastService ToastService { get; set; } = default!;
        private AppDataJson? _appData;
        protected override void OnInitialized()
        {
            _appData = AppDataManager.GetAppData();
        }
        private void OnSaveEvent()
        {
            if (_appData is null)
            {
                return;
            }

            AppDataManager.WriteAppData(_appData);
            ToastService.Toast("Your settings have been saved", ToastType.Success);
        }
    }
}
