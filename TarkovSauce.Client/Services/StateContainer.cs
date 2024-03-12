using TarkovSauce.Client.Data.Models.Remote;
using TarkovSauce.Client.Utils;

namespace TarkovSauce.Client.Services
{
    public class StateContainer
    {
        public ObservableValue<State> State { get; set; } = new(Services.State.ComponentTests);
        public ObservableValue<bool> IsLoading { get; set; } = new(false);
        public TasksGraphQLWrapper? TasksCache { get; set; }
    }
    public enum State
    {
        Home,
        RawLogs,
        FleaSales,
        ComponentTests,
        Settings,
        Tasks,
    }
}
