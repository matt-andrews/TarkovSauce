namespace TarkovSauce.Client.Data.Models
{
    public class ChatMessage
    {
        public MessageType Type { get; set; }
        public string Text { get; set; } = "";
        public bool HasRewards { get; set; }
    }
}
