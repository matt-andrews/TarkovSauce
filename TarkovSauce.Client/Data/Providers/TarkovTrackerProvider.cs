using Microsoft.Extensions.Logging;
using TarkovSauce.Client.Components.Modal;
using TarkovSauce.Client.Data.EventArgs;
using TarkovSauce.Client.Data.Models.Remote;
using TarkovSauce.Client.HttpClients;

namespace TarkovSauce.Client.Data.Providers
{
    public interface ITarkovTrackerProvider : IProvider
    {
        void OnTaskComplete(TaskStatusMessageEventArgs? args);
        void OnTaskFailed(TaskStatusMessageEventArgs? args);
        void OnTaskStarted(TaskStatusMessageEventArgs? args);
    }
    public class TarkovTrackerProvider : ITarkovTrackerProvider, IDisposable
    {
        public Action? OnStateChanged { get; set; }
        private readonly Queue<TaskStatusBody> _taskQueue = [];
        private readonly ITarkovTrackerHttpClient _httpClient;
        private readonly ITSToastService _toastService;
        private readonly ILogger<TarkovTrackerProvider> _logger;
        private readonly ITarkovTrackerLogsProvider _tarkovTrackerLogs;
        private readonly Timer _timer;
        private bool _disposedValue;

        public TarkovTrackerProvider(ITarkovTrackerHttpClient httpClient,
            ITSToastService toastService,
            ILogger<TarkovTrackerProvider> logger,
            ITarkovTrackerLogsProvider tarkovTrackerLogs)
        {
            _httpClient = httpClient;
            _toastService = toastService;
            _logger = logger;
            _tarkovTrackerLogs = tarkovTrackerLogs;
            _timer = new Timer(async (state) => await TaskDispatch(state), null, 30_000, 30_000);
        }

        public void OnTaskComplete(TaskStatusMessageEventArgs? args)
        {
            OnTaskChanged(args);
            _toastService.Toast("Completed Task", ToastType.Success);
        }
        public void OnTaskFailed(TaskStatusMessageEventArgs? args)
        {
            OnTaskChanged(args);
            _toastService.Toast("Failed Task", ToastType.Warning);
        }
        public void OnTaskStarted(TaskStatusMessageEventArgs? args)
        {
            OnTaskChanged(args);
            _toastService.Toast("Started Task", ToastType.Success);
        }
        private void OnTaskChanged(TaskStatusMessageEventArgs? args)
        {
            if (args is null) return;
            _taskQueue.Enqueue(TaskStatusBody.From(args.TaskId, args.Status));
        }
        private async Task TaskDispatch(object? _)
        {
            var tasks = _taskQueue.ToList();
            _taskQueue.Clear();
            if (tasks.Count == 0) return;
            string? result = await _httpClient.SetTaskStatusBatch(tasks);
            _logger.LogInformation("Tarkov Tracker Set Result {result}", result);
            _tarkovTrackerLogs.Log(result ?? "");
            OnStateChanged?.Invoke();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    try
                    {
                        _timer.Dispose();
                    }
                    catch { }
                }

                _disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
