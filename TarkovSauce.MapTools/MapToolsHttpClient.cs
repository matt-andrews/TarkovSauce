namespace TarkovSauce.MapTools
{
    internal class MapToolsHttpClient(HttpClient _httpClient)
    {
        private readonly List<CachedImage> _cachedImages = [];

        public async Task<byte[]> GetImage(string url)
        {
            var cache = _cachedImages.FirstOrDefault(f => f.Url == url);
            if (cache is not null)
                return cache.Data;

            var result = await _httpClient.GetByteArrayAsync(url);
            _cachedImages.Add(new CachedImage() { Data = result, Url = url });
            return result;
        }
    }
}
