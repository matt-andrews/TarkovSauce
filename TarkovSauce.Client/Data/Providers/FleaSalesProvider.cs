using TarkovSauce.Client.Data.EventArgs;
using TarkovSauce.Client.HttpClients;

namespace TarkovSauce.Client.Data.Providers
{
    public interface IFleaSalesProvider : IProvider
    {
        public List<FleaEventModel> Events { get; }
        Task AppendSale(FleaSoldMessageEventArgs? args);
        Task AppendExpiry(FleaExpiredeMessageEventArgs? args);
    }
    public class FleaSalesProvider(ITarkovDevHttpClient _httpClient) : IFleaSalesProvider
    {
        public List<FleaEventModel> Events { get; } = [];
        public Action? OnStateChanged { get; set; }

        public async Task AppendExpiry(FleaExpiredeMessageEventArgs? args)
        {
            if (args is null) return;
            var item = (await _httpClient.GetItemNameBatch(args.ItemId))?.Data?.Items?.FirstOrDefault()?.Name ?? "Not Found";
            Events.Add(new FleaEventModel()
            {
                Type = FleaEventType.Expired,
                ItemName = item,
                Quantity = args.ItemCount
            });
            OnStateChanged?.Invoke();
        }

        public async Task AppendSale(FleaSoldMessageEventArgs? args)
        {
            if (args is null) return;
            var soldItem = (await _httpClient.GetItemNameBatch(args.SoldItemId))?.Data?.Items?.FirstOrDefault()?.Name ?? "Not Found";
            var currency = args.ReceivedItems.FirstOrDefault(f => !string.IsNullOrWhiteSpace(f.Key));
            Events.Add(new FleaEventModel()
            {
                Type = FleaEventType.Sale,
                Buyer = args.Buyer,
                ItemName = soldItem,
                Currency = currency.Key,
                Reward = currency.Value,
                Quantity = args.SoldItemCount
            });
            OnStateChanged?.Invoke();
        }
    }
    public class FleaEventModel
    {
        public FleaEventType Type { get; set; }
        public string ItemName { get; set; } = "";
        public string Buyer { get; set; } = "";
        public int Quantity { get; set; }
        public string Currency { get; set; } = "";
        public int Reward { get; set; }
    }
    public enum FleaEventType
    {
        Sale,
        Expired
    }
}
