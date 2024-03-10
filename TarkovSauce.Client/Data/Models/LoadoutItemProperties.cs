namespace TarkovSauce.Client.Data.Models
{
    public class LoadoutItemProperties
    {
        public int? StackObjectsCount { get; set; }
        public bool? SpawnedInSession { get; set; }
        public LoadoutItemPropertiesDurability? Repairable { get; set; }
        public LoadoutItemPropertiesHpResource? MedKit { get; set; }
        public LoadoutItemPropertiesHpResource? FoodDrink { get; set; }
        public LoadoutItemPropertiesFireMode? FireMode { get; set; }
        public LoadoutItemPropertiesScope? Sight { get; set; }
        public LoadoutItemPropertiesResource? Resource { get; set; }
        public LoadoutItemPropertiesDogtag? Dogtag { get; set; }
        public LoadoutItemPropertiesTag? Tag { get; set; }
        public LoadoutItemPropertiesKey? Key { get; set; }
    }
}
