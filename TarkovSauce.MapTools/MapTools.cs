namespace TarkovSauce.MapTools
{
    public interface IMapTools
    {
        IMap GetMap(string name);
        IReadOnlyList<IMap> Maps { get; }
    }
    internal class MapTools : IMapTools
    {
        public List<Map> Maps { get; } = [];
        IReadOnlyList<IMap> IMapTools.Maps => Maps;

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
            return Maps.FirstOrDefault(f => f.NormalizedName == name)?.AddHttpClient(_httpClient) 
                ?? throw new Exception($"Map name {name} could not be found");
        }
    }
}
