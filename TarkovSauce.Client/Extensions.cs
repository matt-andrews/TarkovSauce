using System.Text.Json;

namespace TarkovSauce.Client
{
    internal static class Extensions
    {
        private readonly static JsonSerializerOptions _defaultOptions = new()
        {
            PropertyNameCaseInsensitive = true,
        };
        public static T? Deserialize<T>(this string json)
        {
            return JsonSerializer.Deserialize<T>(json, _defaultOptions);
        }
        public static string Serialize<T>(this T value)
        {
            return JsonSerializer.Serialize(value, _defaultOptions);
        }
        public static string NormalizeMap(this string map)
        {
            switch (map)
            {
                case "bigmap":
                    return "customs";
                case "Shoreline":
                    return "shoreline";
                case "Woods":
                    return "woods";
                case "Interchange":
                    return "interchange";
                case "TarkovStreets":
                    return "streets-of-tarkov";
                case "Sandbox":
                    return "ground-zero";
                case "Lighthouse":
                    return "lighthouse";
                case "RezervBase":
                    return "reserve";
                case "laboratory":
                    return "the-lab";
                case "factory4_day":
                    return "factory";
                case "factory4_night":
                    return "night-factory";
                default:
                    if (map.StartsWith("factory"))
                        return "factory";
                    return map.ToLower();
            }
        }
    }
}
