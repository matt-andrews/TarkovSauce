using Microsoft.Extensions.DependencyInjection;
using System.Drawing;
using System.Text.Json;

namespace TarkovSauce.MapTools
{
    public static class MapExtensions
    {
        public static IMapTools AddMapTools(this IServiceCollection services, string baseUri)
        {
            services.AddSingleton(new MapToolsHttpClient(new HttpClient() { BaseAddress = new Uri(baseUri) }));
            MapTools tools = new();
            services.AddSingleton<IMapTools>(provider => tools.AddHttpClient(provider.GetRequiredService<MapToolsHttpClient>()));
            return tools;
        }

        public static IMapTools AddMap(this IMapTools mapTools, string configPath)
        {
            if (mapTools is not MapTools tools)
            {
                throw new Exception("Wrong implementation of IMapTools");
            }

            using var fs = new FileStream(configPath, FileMode.Open, FileAccess.Read);
            MapConfig config = JsonSerializer.Deserialize<MapConfig>(fs) ?? throw new Exception("Config linked is not a map config");

            var map = new Map(config.Name, config.NormalizedName, config.Map, config.Anchors.Select(s => new Anchor()
            {
                Game = new GameCoord(s.GameCoord[0], s.GameCoord[1], s.GameCoord[2]),
                Map = new MapCoord(s.MapCoord[0], s.MapCoord[1], s.MapCoord[2])
            }).ToArray(), config.Layers);

            map.AddDefaultPos(config.Extracts.Pmc, FilterType.PmcExtract);
            map.AddDefaultPos(config.Extracts.Scav, FilterType.ScavExtract);
            map.AddDefaultPos(config.Spawns.Pmc, FilterType.PmcSpawns);
            map.AddDefaultPos(config.Spawns.Scav, FilterType.ScavSpawns);

            tools.Maps.Add(map);

            return tools;
        }

        internal static string ToHex(this Color color)
        {
            return $"#{color.R:X2}{color.G:X2}{color.B:X2}";
        }
    }
}
