using System.Text.Json.Serialization;
using TarkovSauce.Client.Data.Models;

namespace TarkovSauce.Client.Data.EventArgs
{
    public class SystemChatMessageWithItems : SystemChatMessage
    {
        public MessageItems Items { get; set; } = new();
        [JsonPropertyName("dt")]
        public long Timestamp { get; set; }
    }
}
