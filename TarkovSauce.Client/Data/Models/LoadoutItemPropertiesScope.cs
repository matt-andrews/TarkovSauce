namespace TarkovSauce.Client.Data.Models
{
    public class LoadoutItemPropertiesScope
    {
        public List<int> ScopesCurrentCalibPointIndexes { get; set; } = [];
        public List<int> ScopesSelectedModes { get; set; } = [];
        public int SelectedScope { get; set; }
    }
}
