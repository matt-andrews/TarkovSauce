using Microsoft.AspNetCore.Components;

namespace TarkovSauce.Client.Components.Layout
{
    public partial class TSCollapsibleMenu
    {
        [Parameter]
        public RenderFragment? ChildContent { get; set; }
        [Parameter]
        public bool IsExpanded { get; set; }
        [Parameter]
        public EventCallback<bool> IsExpandedChanged { get; set; }
        [Parameter]
        public bool AlignRight { get; set; }

        private void OnExpandClick()
        {
            IsExpanded = !IsExpanded;
            StateHasChanged();
        }
    }
}
