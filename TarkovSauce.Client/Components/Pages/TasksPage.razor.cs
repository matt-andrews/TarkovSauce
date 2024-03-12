using Microsoft.AspNetCore.Components;
using TarkovSauce.Client.Data.Models.Remote;
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
        private IEnumerable<TaskModel>? _tasks;
        protected override async Task OnInitializedAsync()
        {
            StateContainer.IsLoading.Value = true;
            var current = await TrackerHttpClient.GetProgress();
            if (StateContainer.TasksCache is null)
            {
                StateContainer.TasksCache = await DevHttpClient.GetTasksBatch();
            }

            var tasks = StateContainer.TasksCache?.Data?.Tasks?
                .Where(w => w.MinPlayerLevel <= current?.Data.PlayerLevel)
                .Where(w => !current?.Data.TasksProgress.Any(a => a.Id == w.Id) ?? false)
                .Where(w => !(w.Name.StartsWith("Compensation for") && w.MinPlayerLevel == 0))
                ?? [];
            //this can probably be linqified but I'm too tired to think about it lol
            List<TaskModel> results = [];
            foreach (var task in tasks)
            {
                bool skip = false;
                foreach (var req in task.TaskRequirements)
                {
                    if (!current?.Data.TasksProgress.Any(a => a.Id == req.Task.Id) ?? false)
                    {
                        skip = true;
                        break;
                    }
                }
                if (skip) continue;
                results.Add(task);
            }
            _tasks = results;

            StateContainer.IsLoading.Value = false;
            await base.OnInitializedAsync();
        }
    }
}
