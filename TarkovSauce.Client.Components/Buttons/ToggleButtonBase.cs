using Microsoft.AspNetCore.Components;
namespace TarkovSauce.Client.Components.Buttons
{
    public class ToggleButtonBase : ButtonBase
    {
        [Parameter]
        public bool IsOn { get; set; }
        [Parameter]
        public EventCallback<bool> IsOnChanged { get; set; }
        protected async Task OnToggleEvent()
        {
            IsOn = !IsOn;
            await IsOnChanged.InvokeAsync(IsOn);
            await OnClick.InvokeAsync();
        }
    }
}
