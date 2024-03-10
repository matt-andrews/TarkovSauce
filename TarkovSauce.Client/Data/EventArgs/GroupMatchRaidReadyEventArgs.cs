using TarkovSauce.Client.Data.Models;

namespace TarkovSauce.Client.Data.EventArgs
{
    public class GroupMatchRaidReadyEventArgs : JsonEventArgs
    {
        public ExtendedProfile ExtendedProfile { get; set; } = new();
        public override string ToString()
        {
            return $"{ExtendedProfile.Info.Nickname} ({ExtendedProfile.PlayerVisualRepresentation.Info.Side}, {ExtendedProfile.PlayerVisualRepresentation.Info.Level})";
        }
    }
}
