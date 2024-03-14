using SQLite;

namespace TarkovSauce.Client.Data.Models.Remote
{
    public class ItemModel
    {
        [PrimaryKey]
        public string Id { get; set; } = "";
        public string Name { get; set; } = "";
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
