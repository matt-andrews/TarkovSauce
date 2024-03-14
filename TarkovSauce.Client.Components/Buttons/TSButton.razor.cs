using Microsoft.AspNetCore.Components;
using System.Text;

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
        [Parameter]
        public bool IsDisabled { get; set; }
        protected override string CssImpl
        {
            get
            {
                StringBuilder sb = new StringBuilder($"tsbutton {base.CssImpl} ");
                if (IsPrimary)
                {
                    sb.Append("primary ");
                }
                if (IsDisabled)
                {
                    sb.Append("disabled ");
                }

                return sb.ToString();
            }
        }

        private async Task OnClickEvent()
        {
            if(IsDisabled) return;
            await OnClick.InvokeAsync();
        }
    }
}
