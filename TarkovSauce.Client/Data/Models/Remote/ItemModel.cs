using SQLite;

namespace TarkovSauce.Client.Data.Models.Remote
{
    public class ItemModel
    {
        [PrimaryKey]
        public string Id { get; set; } = "";
        public string Name { get; set; } = "";
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
