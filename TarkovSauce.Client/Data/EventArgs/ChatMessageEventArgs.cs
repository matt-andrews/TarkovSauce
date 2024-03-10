using TarkovSauce.Client.Data.Models;

namespace TarkovSauce.Client.Data.EventArgs
{
    public class ChatMessageEventArgs : JsonEventArgs
    {
        public ChatMessage Message { get; set; } = new();
    }
}
