using System.Net.Http.Json;
using TarkovSauce.Client.Data.Models.Remote;

namespace TarkovSauce.Client.HttpClients
{
    public interface ITarkovDevHttpClient
    {
        Task<ItemsGraphQLWrapper?> GetItemNameBatch(params string[] ids);
    }
    public class TarkovDevHttpClient(HttpClient _httpClient) : ITarkovDevHttpClient
    {
        public async Task<ItemsGraphQLWrapper?> GetItemNameBatch(params string[] ids)
        {
            Dictionary<string, string> content = new()
            {
                { "query", "{items(ids: [\"" + string.Join("\",\"", ids) +  "\"]) { id name }}" }
            };

            return await Post<ItemsGraphQLWrapper>(content);
        }
        private async Task<TResult?> Post<TResult>(object content)
        {
            var response = await _httpClient.PostAsJsonAsync("graphql", content);
            var responseContent = await response.Content.ReadAsStringAsync();
            return responseContent.Deserialize<TResult>();
        }
    }
}
