using Microsoft.Extensions.DependencyInjection;

namespace TarkovSauce.MapTools
{
    public static class MapExtensions
    {
        public static IMapTools AddMapTools(this IServiceCollection services)
        {
            MapTools tools = new();
            services.AddSingleton<IMapTools>(tools);
            return tools;
        }
        public static IMapTools AddMap(this IMapTools mapTools, Action<IMapOptions> optionsBuilder)
        {
            if (mapTools is not MapTools tools)
            {
                throw new Exception("Wrong implementation of IMapTools");
            }

            MapOptions options = new();
            optionsBuilder(options);
            tools.Maps.Add(new Map(options.Name, options.BaseImage, [.. options.Anchors]));
            return tools;
        }
    }
    public interface IMapOptions
    {
        string Name { get; set; }
        string BaseImage { get; set; }
        void AddAnchor(GameCoord gameCoord, MapCoord mapCoord);
    }
    internal class MapOptions : IMapOptions
    {
        public string Name { get; set; } = "";
        public string BaseImage { get; set; } = "";
        public List<Anchor> Anchors { get; set; } = [];

        public void AddAnchor(GameCoord gameCoord, MapCoord mapCoord)
        {
            Anchors.Add(new Anchor() { Game = gameCoord, Map = mapCoord });
        }
    }
}
