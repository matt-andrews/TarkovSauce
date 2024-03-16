using Microsoft.AspNetCore.Components;
using System.Text;

namespace TarkovSauce.Client.Components.Buttons
{
    public partial class TSButton
    {
        [Parameter]
        public string Content { get; set; } = "";
        [Parameter]
        public bool IsPrimary { get; set; }
        [Parameter]
        public bool IsLoading { get; set; }
        [Parameter]
        public EventCallback<bool> IsLoadingChanged { get; set; }
        protected override string CssImpl
        {
            get
            {
                StringBuilder sb = new($"tsbutton {base.CssImpl} ");
                if (IsPrimary)
                {
                    sb.Append("primary ");
                }

                return sb.ToString();
            }
        }

        private async Task OnClickEvent()
        {
            if(IsDisabled || IsLoading) return;
            await OnClick.InvokeAsync();
        }
    }
}
