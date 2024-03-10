using Microsoft.AspNetCore.Components;
using TarkovSauce.Client.Data.Providers;

namespace TarkovSauce.Client.Components.Pages
{
    public partial class FleaSales
    {
        [Inject]
        public IFleaSalesProvider FleaSalesProvider { get; set; } = default!;
        protected override void OnInitialized()
        {
            FleaSalesProvider.OnStateChanged = () =>
            {
                InvokeAsync(StateHasChanged);
            };
        }
    }
}
