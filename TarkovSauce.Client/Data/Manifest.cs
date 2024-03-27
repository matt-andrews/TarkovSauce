using System.Text.Json.Serialization;

namespace TarkovSauce.Client.Data
{
    internal class Manifest
    {
        [JsonPropertyName("version")]
        public string Version { get; set; } = "";
        [JsonPropertyName("versionId")]
        public int VersionId { get; set; }
    }
}
