using SQLite;
using System.Text.Json.Serialization;

namespace TarkovSauce.Client.Data.Models.Remote
{
    public class ItemModel : IItemModel
    {
        [PrimaryKey]
        public string Id { get; set; } = "";
        [JsonPropertyName("Name")]
        public string ItemName { get; set; } = "";
        public string ShortName { get; set; } = "";
        public string Description { get; set; } = "No description";
        public int Avg24hPrice { get; set; }
        public string GridImageLink { get; set; } = "";
    }
    public class ItemsWrapper
    {
        public ItemModel[]? Items { get; set; }
    }
    public class ItemsGraphQLWrapper
    {
        public ItemsWrapper? Data { get; set; }
    }
}
