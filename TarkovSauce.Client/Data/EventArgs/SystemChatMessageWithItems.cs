using TarkovSauce.Client.Data.Models;

namespace TarkovSauce.Client.Data.EventArgs
{
    public class SystemChatMessageWithItems : SystemChatMessage
    {
        public MessageItems Items { get; set; } = new();
    }
}
