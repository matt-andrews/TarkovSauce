using Microsoft.AspNetCore.Components;

namespace TarkovSauce.Client.Components.Layout
{
    public partial class TSCard
    {
        [Parameter]
        public string Header { get; set; } = "";
        [Parameter]
        public RenderFragment? HeaderContent { get; set; }
        [Parameter]
        public RenderFragment? ChildContent { get; set; }
        protected override string CssImpl => $"tscard {base.CssImpl}";
    }
}
