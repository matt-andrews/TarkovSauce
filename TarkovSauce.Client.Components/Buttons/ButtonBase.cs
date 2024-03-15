using Microsoft.AspNetCore.Components;

namespace TarkovSauce.Client.Components.Buttons
{
    public class ButtonBase : StyleableComponentBase
    {
        [Parameter]
        public EventCallback OnClick { get; set; }
        [Parameter]
        public string Label { get; set; } = "";
        [Parameter]
        public bool IsDisabled { get; set; }
        protected override string CssImpl => IsDisabled ? "disabled " : base.CssImpl;
    }
}
