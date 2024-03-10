namespace TarkovSauce.Client.Data.Models
{
    public class PlayerVisualRepresentation
    {
        public PlayerInfo Info { get; set; } = new();
        public PlayerEquipment Equipment { get; set; } = new();
        public PlayerClothes Customization { get; set; } = new();
    }
}
