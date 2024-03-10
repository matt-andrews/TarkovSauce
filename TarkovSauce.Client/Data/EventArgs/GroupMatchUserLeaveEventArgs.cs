namespace TarkovSauce.Client.Data.EventArgs
{
    public class GroupMatchUserLeaveEventArgs : JsonEventArgs
    {
        public string Nickname { get; set; } = "You";
    }
}
