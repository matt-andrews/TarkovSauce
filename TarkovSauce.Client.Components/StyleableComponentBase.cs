using Microsoft.AspNetCore.Components;

namespace TarkovSauce.Client.Components
{
    public abstract class StyleableComponentBase : ComponentBase
    {
        [Parameter]
        public string Css { get; set; } = "";
        [Parameter]
        public string Style { get; set; } = "";
        protected virtual string CssImpl => Css;
        protected virtual string StyleImpl => Style;
    }
}
