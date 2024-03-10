using Microsoft.AspNetCore.Components;
using TarkovSauce.Client.Data.Providers;

namespace TarkovSauce.Client.Components.Pages
{
    public partial class RawLog
    {
        [Inject]
        public IRawLogProvider RawLogProvider { get; set; } = default!;
        protected override void OnInitialized()
        {
            RawLogProvider.OnStateChanged = () =>
            {
                InvokeAsync(StateHasChanged);
            };
        }
    }
}
