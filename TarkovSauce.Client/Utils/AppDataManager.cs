using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace TarkovSauce.Client.Utils
{
    public interface IAppDataManager
    {
        AppDataJson GetAppData();
        void WriteAppData(AppDataJson data);
    }
    public class AppDataManager : IAppDataManager
    {
        public static string SettingsDirectory => FileSystem.Current.AppDataDirectory;
        public static string SettingsFile => Path.Combine(SettingsDirectory, "settings.json");
        public static string DatabaseFile => Path.Combine(SettingsDirectory, "data.db");
        public static string CheckpointFile => Path.Combine(SettingsDirectory, "checkpoint");
        private readonly static JsonSerializerOptions _options = new() { WriteIndented = true };
        public AppDataJson GetAppData()
        {
            if (!Directory.Exists(SettingsDirectory) || !Path.Exists(SettingsFile))
            {
                return new();
            }
            var file = File.ReadAllText(SettingsFile);
            try
            {
                return JsonSerializer.Deserialize<AppDataJson>(file) ?? new();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return new();
            }
        }
        public void WriteAppData(AppDataJson data)
        {
            if (!Directory.Exists(SettingsDirectory))
            {
                Directory.CreateDirectory(SettingsDirectory);
            }

            string file = JsonSerializer.Serialize(data, _options);
            File.WriteAllText(SettingsFile, file);
        }
    }
    public class AppDataJson
    {
        public Settings Settings { get; set; } = new();
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public bool Debug { get; set; }
    }
    public class Settings
    {
        public string TarkovPath { get; set; } = "";
        public string TarkovTrackerKey { get; set; } = "";
        public string TarkovExePath { get; set; } = "";
    }
}
