namespace TarkovSauce.Client.Data.Models
{
    public class ExtendedProfile
    {
        public PlayerInfo Info { get; set; } = new();
        public bool IsLeader { get; set; }
        public PlayerVisualRepresentation PlayerVisualRepresentation { get; set; } = new();
    }
}
