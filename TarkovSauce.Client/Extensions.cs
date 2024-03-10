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
    }
}
