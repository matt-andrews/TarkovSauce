using System.Text.Json.Serialization;

namespace TarkovSauce.Client.Data.Models
{
    public class LoadoutItem
    {
        [JsonPropertyName("_id")]
        public string Id { get; set; } = "";
        [JsonPropertyName("_tpl")]
        public string Tpl { get; set; } = "";
        public string? ParentId { get; set; }
        public string? SlotId { get; set; }
        public string? Name { get; set; }
        public LoadoutItemProperties? Upd { get; set; }
        public override string ToString()
        {
            var displayName = Tpl;
            if (Name != null) displayName = Name;
            if (Upd?.StackObjectsCount > 1) displayName += $" (x{Upd.StackObjectsCount})";
            if (Upd?.Repairable != null) displayName += $" ({Math.Round(Upd.Repairable.Durability, 2)}/{Upd.Repairable.MaxDurability})";
            return displayName;
        }
    }
}
