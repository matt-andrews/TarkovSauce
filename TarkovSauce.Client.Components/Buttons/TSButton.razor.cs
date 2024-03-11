using Microsoft.AspNetCore.Components;

namespace TarkovSauce.Client.Components.Buttons
{
    public partial class TSButton
    {
        [Parameter]
        public EventCallback OnClick { get; set; }
        [Parameter]
        public string Content { get; set; } = "";
        [Parameter]
        public bool IsPrimary { get; set; }
        protected override string CssImpl => IsPrimary ? $"primary {base.CssImpl}" : base.CssImpl;
    }
}
