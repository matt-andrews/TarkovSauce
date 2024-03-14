using System.Text.Json.Serialization;
using TarkovSauce.Client.Data.Models;

namespace TarkovSauce.Client.Data.EventArgs
{
    public class FleaMarketSoldChatMessage : SystemChatMessageWithItems
    {
        public FleaSoldData SystemData { get; set; } = new();
        [JsonPropertyName("dt")]
        public new long Timestamp { get; set; }
    }
}
