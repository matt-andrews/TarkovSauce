namespace TarkovSauce.Client.Data.EventArgs
{
    public class JsonEventArgs : System.EventArgs
    {
        public string Type { get; set; } = "";
        public string EventId { get; set; } = "";
    }
}
