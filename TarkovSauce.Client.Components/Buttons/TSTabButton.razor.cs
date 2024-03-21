using Microsoft.AspNetCore.Components;

namespace TarkovSauce.Client.Components.Buttons
{
    public partial class TSTabButton
    {
        private readonly string _id = Guid.NewGuid().ToString();
        [Parameter]
        public IEnumerable<string>? DataSource { get; set; }
        [Parameter]
        public string Css { get; set; } = "";
        [Parameter]
        public string Placeholder { get; set; } = "";
        [Parameter]
        public string Selection { get; set; } = "";
        [Parameter]
        public EventCallback<string> OnSelection { get; set; }
        [Parameter]
        public string Help { get; set; } = "";
        public bool IsExpanded { get; set; }

        private async Task OnClick(string val)
        {
            if (val == Selection) return;
            IsExpanded = false;
            StateHasChanged();
            await OnSelection.InvokeAsync(val);
        }
        private void OnExpandClick()
        {
            IsExpanded = !IsExpanded;
            StateHasChanged();
        }
    }
}
