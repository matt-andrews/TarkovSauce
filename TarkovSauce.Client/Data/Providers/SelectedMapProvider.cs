using TarkovSauce.Client.Data.EventArgs;

namespace TarkovSauce.Client.Data.Providers
{
    public interface ISelectedMapProvider : IProvider
    {
        string Map { get; }
        void SelectMap(GroupCreateLobbyEventArgs args);
        void ClearMap();
    }
    internal class SelectedMapProvider : ISelectedMapProvider
    {
        public Action? OnStateChanged { get; set; }
        public string Map { get; private set; } = "";
        public void SelectMap(GroupCreateLobbyEventArgs args)
        {
            if (args.RaidType == RaidType.PMC)
            {
                Map = args.Map;
                OnStateChanged?.Invoke();
            }
        }
        public void ClearMap()
        {
            Map = "";
            OnStateChanged?.Invoke();
        }
    }
}
