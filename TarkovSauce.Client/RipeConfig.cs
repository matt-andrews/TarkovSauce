using Ripe.Sdk.Core;

namespace TarkovSauce.Client
{
    internal class RipeConfig : IRipeConfiguration
    {
        public int TimeToLive { get; set; }
        public string ApiVersion { get; set; } = "";
        public ClientConfig Client { get; set; } = new();
    }

    internal class ClientConfig
    {
        public string BlobSource { get; set; } = "";
        public string TarkovDevApi { get; set; } = "";
        public string TarkovTrackerApi { get; set; } = "";
    }
}
