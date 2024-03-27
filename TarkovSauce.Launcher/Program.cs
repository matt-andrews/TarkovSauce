using Ripe.Sdk.Core;
using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Nodes;
using TarkovSauce.Launcher;

RipeSdk<Config> ripeSdk = new(options =>
{
    var setup = JsonSerializer.Deserialize<JsonObject>(Convert.FromBase64String(File.ReadAllText("ripe.sh"))) 
        ?? throw new JsonException("Failed to pull setup file");
    options.Uri = setup["Uri"]?.ToString();
    options.ApiKey = setup["ApiKey"]?.ToString();
    options.Version = typeof(Program).Assembly.GetName().Version?.ToString() ?? "0.0.0.0";
});
Config config = await ripeSdk.HydrateAsync();

string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, config.Launcher.AppParentFolder, config.Launcher.AppChildFolder);
string exe = Path.Combine(path, config.Launcher.AppExe);
string manifest = Path.Combine(path, config.Launcher.Manifest);

var httpClient = new BuildHttpClient();

var jumbler = new ConsoleJumbler();
jumbler.WriteLineFast("Tarkov Sauce");
await jumbler.Write("Checking for updates");

Manifest remoteManifest = await jumbler.Load(() => httpClient.Get<Manifest>(config.Launcher.Manifest));

bool update = false;
bool isFresh = false;
if (Directory.Exists(path) && File.Exists(exe) && File.Exists(manifest))
{
    using var fs = new FileStream(manifest, FileMode.OpenOrCreate, FileAccess.ReadWrite);
    var manifestObj = JsonSerializer.Deserialize<Manifest>(fs);
    if (manifestObj?.VersionId != remoteManifest.VersionId)
    {
        update = true;
    }
}
else
{
    update = true;
    isFresh = true;
}

if (update)
{
    await jumbler.Write("Updating");
    await jumbler.Load(() => httpClient.DownloadZip($"Sauce App.{remoteManifest.Version}.zip"));
    if (isFresh)
    {
        await jumbler.WriteLine("Fresh install detected... Do you have a Tarkov Tracker API Key? (Y/N)");
        var askApiKey = jumbler.ReadKey();
        if (askApiKey == ConsoleKey.Y)
        {
            await jumbler.WriteLine("Enter your Tarkov Tracker API Key:");
            var apiKey = jumbler.ReadLine();
            if (!string.IsNullOrWhiteSpace(apiKey))
            {
                string settingsFileData = "{\"Settings\": {\"TarkovTrackerKey\": \" " + apiKey + "\"}}";
                string appDataPath = Path.Combine(path, "App Data");
                if(!Directory.Exists(appDataPath))
                {
                    Directory.CreateDirectory(appDataPath);
                }
                string appDataFile = Path.Combine(appDataPath, "settings.json");
                File.WriteAllText(appDataFile, settingsFileData);
            }
        }
    }
}

await jumbler.WriteLine("Launching...");
Process.Start(exe, "--launcher");
await Task.Delay(1000);
