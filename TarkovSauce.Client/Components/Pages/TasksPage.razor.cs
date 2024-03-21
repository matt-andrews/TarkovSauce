using Microsoft.AspNetCore.Components;
using TarkovSauce.Client.Data.Models.Remote;
using TarkovSauce.Client.Data.Providers;
using TarkovSauce.Client.Services;

namespace TarkovSauce.Client.Components.Pages
{
    public partial class TasksPage
    {
        [Inject]
        public StateContainer StateContainer { get; set; } = default!;
        [Inject]
        public ISelectedMapProvider SelectedMapProvider { get; set; } = default!;
        [Inject]
        public ITarkovTrackerProvider TarkovTrackerProvider { get; set; } = default!;
        [Inject]
        public ITasksService TasksService { get; set; } = default!;
        private IEnumerable<TaskModel>? _tasks;
        protected override async Task OnInitializedAsync()
        {
            StateContainer.IsLoading.Value = true;
            _tasks = await TasksService.BuildCurrentQuests();
            StateContainer.IsLoading.Value = false;
            SelectedMapProvider.OnStateChanged = () => InvokeAsync(StateHasChanged);
            TarkovTrackerProvider.OnStateChanged = () => InvokeAsync(StateHasChanged);
            await base.OnInitializedAsync();
        }
    }
}
