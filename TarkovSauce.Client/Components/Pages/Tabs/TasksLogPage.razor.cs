using Microsoft.AspNetCore.Components;
using TarkovSauce.Client.Data.Providers;

namespace TarkovSauce.Client.Components.Pages.Tabs
{
    public partial class TasksLogPage
    {
        [Inject]
        public ITarkovTrackerLogsProvider TarkovTrackerLogsProvider { get; set; } = default!;
        protected override void OnInitialized()
        {
            TarkovTrackerLogsProvider.OnStateChanged = () => InvokeAsync(StateHasChanged);
        }
    }
}
