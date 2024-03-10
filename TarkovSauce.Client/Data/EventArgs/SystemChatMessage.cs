using TarkovSauce.Client.Data.Models;

namespace TarkovSauce.Client.Data.EventArgs
{
    public class SystemChatMessage : ChatMessage
    {
        public string TemplateId { get; set; } = "";
    }
}
