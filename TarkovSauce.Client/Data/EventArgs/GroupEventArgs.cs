using TarkovSauce.Client.Data.Models;

namespace TarkovSauce.Client.Data.EventArgs
{
    public class GroupEventArgs : JsonEventArgs
    {
        public PlayerInfo Info { get; set; } = new();
        public bool IsLeader { get; set; }
    }
}
