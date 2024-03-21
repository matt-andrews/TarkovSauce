using TarkovSauce.Client.Data.Models.Remote;
using TarkovSauce.Client.HttpClients;

namespace TarkovSauce.Client.Services
{
    public interface ITasksService
    {
        Task<IEnumerable<TaskModel>> BuildCurrentQuests();
        Task<IEnumerable<TaskModel>> BuildCurrentQuests(string map);
        bool TestForMap(TaskModel model, string mapName);
    }
    internal class TasksService(StateContainer _stateContainer, ITarkovDevHttpClient _devHttpClient, ITarkovTrackerHttpClient _trackerHttpClient) : ITasksService
    {
        public async Task<IEnumerable<TaskModel>> BuildCurrentQuests()
        {
            var progress = await _trackerHttpClient.GetProgress();
            _stateContainer.TasksCache ??= await _devHttpClient.GetTasksBatch();

            return _stateContainer.TasksCache?.Data?.Tasks?
                .Where(task => progress?.Data.TasksProgress
                    .Any(tp => tp.Id == task.Id && !tp.Complete && !tp.Failed && !tp.Invalid) ?? false)
                ?? [];
        }

        public async Task<IEnumerable<TaskModel>> BuildCurrentQuests(string map)
        {
            var progress = await _trackerHttpClient.GetProgress();
            _stateContainer.TasksCache ??= await _devHttpClient.GetTasksBatch();

            return _stateContainer.TasksCache?.Data?.Tasks?
                .Where(task => progress?.Data.TasksProgress
                    .Any(tp => tp.Id == task.Id && !tp.Complete && !tp.Failed && !tp.Invalid) ?? false)
                .Where(task => TestForMap(task, map))
                ?? [];
        }

        public bool TestForMap(TaskModel model, string mapName)
        {
            if (string.IsNullOrWhiteSpace(mapName)) return true;
            if (model.Objectives.Any(objective => objective.Maps.Any(map => map.NormalizedName == mapName) || (objective.Maps.Length == 0 && objective.Type != "extract" && objective.Type != "giveQuestItem")))
                return true;
            return false;
        }

    }
}
