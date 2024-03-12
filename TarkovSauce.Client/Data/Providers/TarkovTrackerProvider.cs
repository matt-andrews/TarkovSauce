using Microsoft.Extensions.Logging;
using TarkovSauce.Client.Components.Modal;
using TarkovSauce.Client.Data.EventArgs;
using TarkovSauce.Client.Data.Models.Remote;
using TarkovSauce.Client.HttpClients;

namespace TarkovSauce.Client.Data.Providers
{
    public interface ITarkovTrackerProvider : IProvider
    {
        Task OnTaskComplete(TaskStatusMessageEventArgs args);
    }
    public class TarkovTrackerProvider(ITarkovTrackerHttpClient _httpClient,
        ITSToastService _toastService,
        ILogger<TarkovTrackerProvider> _logger)
        : ITarkovTrackerProvider
    {
        public Action? OnStateChanged { get; set; }
        public async Task OnTaskComplete(TaskStatusMessageEventArgs args)
        {
            string? result = await _httpClient.SetTaskStatusBatch(
            [
                TaskStatusBody.From(args.TaskId, args.Status)
            ]);

            _toastService.Toast(args.Message.Text, ToastType.Success);
            _logger.LogInformation("Tarkov Tracker Set Result {result}", result);
        }
    }
}
