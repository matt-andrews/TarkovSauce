
namespace TarkovSauce.Client.Data.EventArgs
{
    public class GroupCreateLobbyEventArgs : JsonEventArgs
    {
        public string Map
        {
            get
            {
                switch (RaidSettings.Location)
                {
                    case "bigmap":
                        return "customs";
                    case "Shoreline":
                        return "shoreline";
                    case "Woods":
                        return "woods";
                    case "Interchange":
                        return "interchange";
                    case "TarkovStreets":
                        return "streets-of-tarkov";
                    case "Sandbox":
                        return "ground-zero";
                    case "Lighthouse":
                        return "lighthouse";
                    case "RezervBase":
                        return "reserve";
                    case "laboratory":
                        return "the-lab";
                    case "factory4_day":
                        return "factory";
                    case "factory..night?"://TODO
                        return "night-factory";
                    default:
                        if (RaidSettings.Location.StartsWith("factory"))
                            return "factory";
                        return "";
                }
            }
        }
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
