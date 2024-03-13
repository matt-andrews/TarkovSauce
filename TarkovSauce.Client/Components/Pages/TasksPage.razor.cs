using Microsoft.AspNetCore.Components;
using TarkovSauce.Client.Data.Models.Remote;
using TarkovSauce.Client.Data.Providers;
using TarkovSauce.Client.HttpClients;
using TarkovSauce.Client.Services;

namespace TarkovSauce.Client.Components.Pages
{
    public partial class TasksPage
    {
        [Inject]
        public ITarkovTrackerHttpClient TrackerHttpClient { get; set; } = default!;
        [Inject]
        public ITarkovDevHttpClient DevHttpClient { get; set; } = default!;
        [Inject]
        public StateContainer StateContainer { get; set; } = default!;
        [Inject]
        public ISelectedMapProvider SelectedMapProvider { get; set; } = default!;
        [Inject]
        public ITarkovTrackerProvider TarkovTrackerProvider { get; set; } = default!;
        private IEnumerable<TaskModel>? _tasks;
        protected override async Task OnInitializedAsync()
        {
            StateContainer.IsLoading.Value = true;
            await BuildCurrentQuests();
            StateContainer.IsLoading.Value = false;
            SelectedMapProvider.OnStateChanged = () => InvokeAsync(StateHasChanged);
            TarkovTrackerProvider.OnStateChanged = () => InvokeAsync(StateHasChanged);
            await base.OnInitializedAsync();
        }

        private async Task BuildCurrentQuests()
        {
            var progress = await TrackerHttpClient.GetProgress();
            if (StateContainer.TasksCache is null)
            {
                StateContainer.TasksCache = await DevHttpClient.GetTasksBatch();
            }

            var tasks = StateContainer.TasksCache?.Data?.Tasks?
                .Where(task => task.MinPlayerLevel <= progress?.Data.PlayerLevel)
                .Where(task => !progress?.Data.TasksProgress.Any(tp => tp.Id == task.Id) ?? false)
                .Where(task => task.TraderRequirements.Length == 0)
                ?? [];

            List<TaskModel> results = [];
            foreach (var task in tasks)
            {
                bool skip = false;
                foreach (var req in task.TaskRequirements)
                {
                    if (!progress?.Data.TasksProgress.Any(tp => tp.Id == req.Task.Id) ?? false)
                    {
                        skip = true;
                        break;
                    }
                }
                if (skip) continue;
                results.Add(task);
            }
            _tasks = results;
        }

        private bool TestForMap(TaskModel model)
        {
            if (string.IsNullOrWhiteSpace(SelectedMapProvider.Map)) return true;
            if (model.Objectives.Any(objective => objective.Maps.Any(map => map.NormalizedName == SelectedMapProvider.Map) || (objective.Maps.Length == 0 && objective.Type != "extract" && objective.Type != "giveQuestItem")))
                return true;
            return false;
        }
    }
}
