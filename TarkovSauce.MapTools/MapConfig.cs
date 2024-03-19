namespace TarkovSauce.MapTools
{
    internal class MapConfig
    {
        public string Name { get; set; } = "";
        public string NormalizedName { get; set; } = "";
        public string Map { get; set; } = "";
        public AnchorConfig[] Anchors { get; set; } = [];
        public ExtractsConfig Extracts { get; set; } = new();
        public SpawnsConfig Spawns { get; set; } = new();
        public LayerObj[] Layers { get; set; } = [];
    }
    internal class ExtractsConfig
    {
        public PosConfig[] Pmc { get; set; } = [];
        public PosConfig[] Scav { get; set; } = [];
    }
    internal class SpawnsConfig
    {
        public PosConfig[] Pmc { get; set; } = [];
        public PosConfig[] Scav { get; set; } = [];
    }
    internal class AnchorConfig
    {
        public float[] GameCoord { get; set; } = [];
        public int[] MapCoord { get; set; } = [];
    }
    internal class BossConfig
    {
        public PosConfig[] Bosses { get; set; } = [];
        public PosConfig[] Goons { get; set; } = [];
    }
    public class LayerObj
    {
        public string Name { get; set; } = "";
        public string Map { get; set; } = "";
        public int Layer { get; set; } = 0;
    }
    internal interface IPos
    {
        float[] XYZ { get; set; }
        string Sprite { get; set; }
        string Title { get; set; }
        int[] TitleColor { get; set; }
        int Layer { get; set; }
    }
    internal class PosConfig : IPos
    {
        public float[] XYZ { get; set; } = [];
        public string Sprite { get; set; } = "";
        public string Title { get; set; } = "";
        public int[] TitleColor { get; set; } = [];
        public int Layer { get; set; } = -1;
    }
}
