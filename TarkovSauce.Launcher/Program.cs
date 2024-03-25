using System.Diagnostics;
using System.Text.Json;
using TarkovSauce.Launcher;

string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Sauce App", "win10-x64");
string exe = Path.Combine(path, "TarkovSauce.Client.Exe");
string manifest = Path.Combine(path, "manifest.json");

var httpClient = new BuildHttpClient();

var jumbler = new ConsoleJumbler();
await jumbler.WriteLine("Welcome to Tarkov Sauce");

await jumbler.Write("Checking for updates");

Manifest remoteManifest = await jumbler.Load(() => httpClient.Get<Manifest>("manifest.json"));

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
        await jumbler.WriteLine("Fresh install detected... Do you have a Tarkov Tracker API Key? (y/n)");
        var askApiKey = Console.ReadKey();
        if (askApiKey.Key == ConsoleKey.Y)
        {
            await jumbler.WriteLine("Enter your Tarkov Tracker API Key:");
            var apiKey = Console.ReadLine();
            await jumbler.WriteLine();
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
