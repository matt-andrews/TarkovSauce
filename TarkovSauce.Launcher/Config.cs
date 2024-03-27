using Ripe.Sdk.Core;

namespace TarkovSauce.Launcher
{
    internal class Config : IRipeConfiguration
    {
        public int TimeToLive { get; set; }
        public string ApiVersion { get; set; } = "";
        public LauncherConfig Launcher { get; set; } = new();
    }
    internal class LauncherConfig
    {
        public string AppParentFolder { get; set; } = "";
        public string AppChildFolder { get; set; } = "";
        public string AppExe { get; set; } = "";
        public string Manifest { get; set; } = "";
    }
}
