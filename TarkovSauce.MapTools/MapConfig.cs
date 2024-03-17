namespace TarkovSauce.MapTools
{
    internal class MapConfig
    {
        public string Name { get; set; } = "";
        public string Map { get; set; } = "";
        public AnchorConfig[] Anchors { get; set; } = [];
        public ExtractsConfig Extracts { get; set; } = new();
    }
    internal class ExtractsConfig
    {
        public PmcConfig[] Pmc { get; set; } = [];
        internal class PmcConfig : IPos
        {
            public float[] XYZ { get; set; } = [];
            public string Sprite { get; set; } = "";
            public string Title { get; set; } = "";
            public int[] TitleColor { get; set; } = [];
        }
    }
    internal class AnchorConfig
    {
        public float[] GameCoord { get; set; } = [];
        public int[] MapCoord { get; set; } = [];
    }
    internal interface IPos
    {
        float[] XYZ { get; set; }
        string Sprite { get; set; }
        string Title { get; set; }
        int[] TitleColor { get; set; }
    }
}
