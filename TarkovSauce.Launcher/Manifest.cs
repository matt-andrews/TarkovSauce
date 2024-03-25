using System.Text.Json.Serialization;

namespace TarkovSauce.Launcher
{
    internal class Manifest
    {
        [JsonPropertyName("version")]
        public string Version { get; set; } = "";
        [JsonPropertyName("versionId")]
        public int VersionId { get; set; }
    }
}
