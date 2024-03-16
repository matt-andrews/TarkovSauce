namespace TarkovSauce.MapTools
{
    public interface IMapTools
    {
        IMap GetMap(string name);
    }
    internal class MapTools : IMapTools
    {
        public List<Map> Maps { get; } = [];

        public IMap GetMap(string name)
        {
            return Maps.FirstOrDefault(f => f.Name == name) 
                ?? throw new Exception($"Map name {name} could not be found");
        }
    }
}
