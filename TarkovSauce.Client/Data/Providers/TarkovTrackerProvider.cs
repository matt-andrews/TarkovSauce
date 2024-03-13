using Microsoft.Extensions.Logging;
using TarkovSauce.Client.Components.Modal;
using TarkovSauce.Client.Data.EventArgs;
using TarkovSauce.Client.Data.Models.Remote;
using TarkovSauce.Client.HttpClients;

namespace TarkovSauce.Client.Data.Providers
{
    public interface ITarkovTrackerProvider : IProvider
    {
        Task OnTaskComplete(TaskStatusMessageEventArgs? args);
        Task OnTaskFailed(TaskStatusMessageEventArgs? args);
        Task OnTaskStarted(TaskStatusMessageEventArgs? args);
    }
    public class TarkovTrackerProvider(ITarkovTrackerHttpClient _httpClient,
        ITSToastService _toastService,
        ILogger<TarkovTrackerProvider> _logger,
        ITarkovTrackerLogsProvider _tarkovTrackerLogs)
        : ITarkovTrackerProvider
    {
        public Action? OnStateChanged { get; set; }
        public async Task OnTaskComplete(TaskStatusMessageEventArgs? args)
        {
            await OnTaskChanged(args);
            _toastService.Toast("Completed Task", ToastType.Success);
        }
        public async Task OnTaskFailed(TaskStatusMessageEventArgs? args)
        {
            await OnTaskChanged(args);
            _toastService.Toast("Failed Task", ToastType.Success);
        }
        public async Task OnTaskStarted(TaskStatusMessageEventArgs? args)
        {
            await OnTaskChanged(args);
            _toastService.Toast("Started Task", ToastType.Success);
        }
        private async Task OnTaskChanged(TaskStatusMessageEventArgs? args)
        {
            if (args is null) return;
            string? result = await _httpClient.SetTaskStatusBatch(
            [
                TaskStatusBody.From(args.TaskId, args.Status)
            ]) ?? string.Empty;
            _logger.LogInformation("Tarkov Tracker Set Result {result}", result);
            _tarkovTrackerLogs.Log(result);
            OnStateChanged?.Invoke();
        }
    }
}
