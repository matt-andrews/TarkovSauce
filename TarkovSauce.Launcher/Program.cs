using System.Diagnostics;
using System.Text.Json;
using TarkovSauce.Launcher;

string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Sauce App", "Sauce App");
string exe = Path.Combine(path, "TarkovSauce.Client.Exe");
string manifest = Path.Combine(path, "manifest.json");

var httpClient = new BuildHttpClient();

var jumbler = new ConsoleJumbler();
await jumbler.WriteLine("Welcome to Tarkov Sauce");

await jumbler.Write("Checking for updates");

Manifest remoteManifest = await jumbler.Load(() => httpClient.Get<Manifest>("manifest.json"));

bool update = false;
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
}

if (update)
{
    await jumbler.Write("Updating");
    await jumbler.Load(() => httpClient.DownloadZip($"Sauce App.{remoteManifest.Version}.zip"));
}

await jumbler.WriteLine("Launching...");
Process.Start(exe);
await Task.Delay(1000);
