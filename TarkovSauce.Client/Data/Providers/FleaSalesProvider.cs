using SQLite;
using TarkovSauce.Client.Data.EventArgs;
using TarkovSauce.Client.HttpClients;
using TarkovSauce.Client.Services;
using TarkovSauce.Client.Data.Models;

namespace TarkovSauce.Client.Data.Providers
{
    public interface IFleaSalesProvider : IProvider
    {
        List<FleaEventModel> Events { get; }
        List<FleaEventModel> CachedEvents { get; }
        void InitCache();
        Task AppendSale(FleaSoldMessageEventArgs? args);
        Task AppendExpiry(FleaExpiredeMessageEventArgs? args);
    }
    public class FleaSalesProvider : IFleaSalesProvider
    {
        private readonly ITarkovDevHttpClient _httpClient;
        private readonly ISqlService _sqlService;

        public List<FleaEventModel> Events { get; } = [];
        public List<FleaEventModel> CachedEvents { get; private set; } = [];
        public Action? OnStateChanged { get; set; }

        public FleaSalesProvider(ITarkovDevHttpClient httpClient, ISqlService sqlService)
        {
            _httpClient = httpClient;
            _sqlService = sqlService;
        }
        public void InitCache()
        {
            CachedEvents = _sqlService.Get<FleaEventModel>().ToList();
        }
        public async Task AppendExpiry(FleaExpiredeMessageEventArgs? args)
        {
            if (args is null) return;
            var item = (await _httpClient.GetItemNameBatch(args.ItemId))?.Data?.Items?.FirstOrDefault();
            var ev = new FleaEventModel()
            {
                Type = FleaEventType.Expired,
                ItemName = item?.ItemName ?? "_NOT FOUND_",
                ShortName = item?.ShortName ?? "_NOT FOUND_",
                Description = item?.Description ?? "_NOT FOUND_",
                Avg24hPrice = item?.Avg24hPrice ?? 0,
                Quantity = args.ItemCount,
                GridImageLink = item?.GridImageLink ?? "",
                Timestamp = DateTime.UnixEpoch.AddSeconds(args.Message.Timestamp).ToLocalTime()
            };
            Events.Add(ev);
            _sqlService.Insert(ev);
            OnStateChanged?.Invoke();
        }

        public async Task AppendSale(FleaSoldMessageEventArgs? args)
        {
            if (args is null) return;
            var soldItem = (await _httpClient.GetItemNameBatch(args.SoldItemId))?.Data?.Items?.FirstOrDefault();
            var currency = args.ReceivedItems.FirstOrDefault(f => !string.IsNullOrWhiteSpace(f.Key));
            var ev = new FleaEventModel()
            {
                Type = FleaEventType.Sale,
                Buyer = args.Buyer,
                ItemName = soldItem?.ItemName ?? "_NOT FOUND_",
                ShortName = soldItem?.ShortName ?? "_NOT FOUND_",
                Description = soldItem?.Description ?? "_NOT FOUND_",
                Avg24hPrice = soldItem?.Avg24hPrice ?? 0,
                Currency = currency.Key,
                Reward = currency.Value,
                GridImageLink = soldItem?.GridImageLink ?? "",
                Quantity = args.SoldItemCount,
                Timestamp = DateTime.UnixEpoch.AddSeconds(args.Message.Timestamp).ToLocalTime()
            };
            Events.Add(ev);
            _sqlService.Insert(ev);
            OnStateChanged?.Invoke();
        }
    }
    public class FleaEventModel : IItemModel
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public FleaEventType Type { get; set; }
        public string ItemName { get; set; } = "";
        public string ShortName { get; set; } = "";
        public string Description { get; set; } = "";
        public int Avg24hPrice { get; set; }
        public string Buyer { get; set; } = "";
        public int Quantity { get; set; }
        public string Currency { get; set; } = "";
        public int Reward { get; set; }
        public DateTime Timestamp { get; set; }
        public string GridImageLink { get; set; } = "";
    }
    public enum FleaEventType
    {
        Sale,
        Expired
    }
}
