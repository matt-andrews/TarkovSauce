using System.IO.Compression;
using System.Net.Http.Json;

namespace TarkovSauce.Launcher
{
    internal class BuildHttpClient
    {
        private readonly HttpClient _httpClient;
        public BuildHttpClient()
        {
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri("https://tarkovsauce.blob.core.windows.net/static/builds/")
            };
        }
        public async Task<T> Get<T>(string url)
        {
            return await _httpClient.GetFromJsonAsync<T>(url) ?? throw new Exception();
        }
        public async Task DownloadZip(string url)
        {
            using var response = await _httpClient.GetAsync(url);
            using var stream = await response.Content.ReadAsStreamAsync();
            ZipFile.ExtractToDirectory(stream, url.Split('/').Last().Split('.').First(), true);
        }
    }
}
