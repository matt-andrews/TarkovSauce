using TarkovSauce.Client.Utils;

namespace TarkovSauce.Client.Services
{
    public class StateContainer
    {
        public ObservableValue<State> State { get; set; } = new(Services.State.Home);
    }
    public enum State
    {
        Home,
        RawLogs,
        FleaSales,
    }
}
