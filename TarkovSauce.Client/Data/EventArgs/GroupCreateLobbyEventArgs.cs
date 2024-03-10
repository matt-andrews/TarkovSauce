
namespace TarkovSauce.Client.Data.EventArgs
{
    public class GroupCreateLobbyEventArgs : JsonEventArgs
    {
        public string Map => RaidSettings.Location;
        public string RaidMode => RaidSettings.RaidMode;
        public RaidType RaidType => RaidSettings.Side switch
        {
            "Pmc" => RaidType.PMC,
            "Savage" => RaidType.Scav,
            _ => RaidType.Unknown,
        };
        public RaidSettingsObj RaidSettings { get; set; } = new();
        public class RaidSettingsObj
        {
            public string Location { get; set; } = "";
            public string RaidMode { get; set; } = "";
            public string Side { get; set; } = "";
        }
    }
}
