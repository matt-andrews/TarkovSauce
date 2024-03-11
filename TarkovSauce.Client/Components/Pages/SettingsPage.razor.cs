using Microsoft.AspNetCore.Components;
using TarkovSauce.Client.Utils;

namespace TarkovSauce.Client.Components.Pages
{
    public partial class SettingsPage
    {
        [Inject]
        public IAppDataManager AppDataManager { get; set; } = default!;
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
        }
    }
}
