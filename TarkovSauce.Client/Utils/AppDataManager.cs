using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;
using TarkovSauce.Client.Data;

namespace TarkovSauce.Client.Utils
{
    public interface IAppDataManager
    {
        AppDataJson GetAppData();
        void WriteAppData(AppDataJson data);
    }
    public class AppDataManager : IAppDataManager
    {
        public static bool IsDev => Environment.GetCommandLineArgs().Contains("--dev");
        public static string SettingsDirectory
        {
            get
            {
                string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App Data");
                if (!Path.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                return path;
            }
        }
        public static string SettingsFile => Path.Combine(SettingsDirectory, "settings.json");
        public static string DatabaseFile => Path.Combine(SettingsDirectory, "data.db");
        public static string CheckpointFile => Path.Combine(SettingsDirectory, "checkpoint");
        private readonly static JsonSerializerOptions _options = new() { WriteIndented = true };
        public static string Version { get; private set; } = "N/A";
        public static void GetVersion()
        {
            try
            {
                string manifestFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "manifest.json");
                if (File.Exists(manifestFile))
                {
                    using var fs = new FileStream(manifestFile, FileMode.Open, FileAccess.Read);
                    var manifest = JsonSerializer.Deserialize<Manifest>(fs);
                    Version = manifest?.Version ?? "N/A";
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }
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
