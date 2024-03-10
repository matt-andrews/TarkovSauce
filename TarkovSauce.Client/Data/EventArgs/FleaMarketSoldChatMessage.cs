using TarkovSauce.Client.Data.Models;

namespace TarkovSauce.Client.Data.EventArgs
{
    public class FleaMarketSoldChatMessage : SystemChatMessageWithItems
    {
        public FleaSoldData SystemData { get; set; } = new();
    }
}
