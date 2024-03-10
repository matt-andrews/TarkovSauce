using Microsoft.AspNetCore.Components;
using TarkovSauce.Client.Services;

namespace TarkovSauce.Client.Components.Pages
{
    public partial class Index
    {
        [Inject]
        public StateContainer StateContainer { get; set; } = default!;
        protected override void OnInitialized()
        {
            StateContainer.State.OnValueChanged += _ => StateHasChanged();
        }
    }
}
