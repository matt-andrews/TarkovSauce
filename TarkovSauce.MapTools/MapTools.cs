namespace TarkovSauce.MapTools
{
    public interface IMapTools
    {
        IMap GetMap(string name);
    }
    internal class MapTools : IMapTools
    {
        public List<Map> Maps { get; } = [];
        private MapToolsHttpClient? _httpClient;
        public MapTools AddHttpClient(MapToolsHttpClient httpClient)
        {
            _httpClient = httpClient;
            return this;
        }

        public IMap GetMap(string name)
        {
            if (_httpClient is null)
                throw new Exception("Http Client is missing!");
            return Maps.FirstOrDefault(f => f.Name == name)?.AddHttpClient(_httpClient) 
                ?? throw new Exception($"Map name {name} could not be found");
        }
    }
}
