namespace TarkovSauce.Client.Data.EventArgs
{
    public class FleaSoldMessageEventArgs : SystemChatMessageEventArgs
    {
        public string Buyer => Message.SystemData.BuyerNickname;
        public string SoldItemId => Message.SystemData.SoldItem;
        public string SoldItemIdMask { get; set; } = "";
        public int SoldItemCount => Message.SystemData.ItemCount;
        private readonly Dictionary<string, string> _currency = new()
        {
            { "5449016a4bdc2d6f028b456f", "₽" },
            { "569668774bdc2da2298b4568", "€" },
            { "5696686a4bdc2da3298b456a", "$" }
        };
        public Dictionary<string, int> ReceivedItems
        {
            get
            {
                Dictionary<string, int> items = [];
                foreach (var item in Message.Items.Data)
                {
                    items.Add(_currency[item.Tpl], item.Upd?.StackObjectsCount ?? 1);
                }
                return items;
            }
        }
        public new FleaMarketSoldChatMessage Message { get; set; } = new();
    }
}
