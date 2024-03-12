using System.Net.Http.Json;
using TarkovSauce.Client.Data.Models.Remote;

namespace TarkovSauce.Client.HttpClients
{
    public interface ITarkovDevHttpClient
    {
        Task<ItemsGraphQLWrapper?> GetItemNameBatch(params string[] ids);
        Task<TasksGraphQLWrapper?> GetTasksBatch();
    }
    public class TarkovDevHttpClient(HttpClient _httpClient) : ITarkovDevHttpClient
    {
        public async Task<ItemsGraphQLWrapper?> GetItemNameBatch(params string[] ids)
        {
            Dictionary<string, string> content = new()
            {
                { "query", "{items(ids: [\"" + string.Join("\",\"", ids) + "\"]) { id name }}" }
            };

            return await Post<ItemsGraphQLWrapper>(content);
        }
        public async Task<TasksGraphQLWrapper?> GetTasksBatch()
        {
            Dictionary<string, string> content = new()
            {
                { "query", "{\r\n  tasks {\r\n    id\r\n    name\r\n    minPlayerLevel\r\n    trader {\r\n      name\r\n      imageLink\r\n    }\r\n    taskRequirements {\r\n      task {\r\n        id\r\n      }\r\n      status\r\n    }\r\n    objectives {\r\n      id\r\n      type\r\n      description\r\n      maps {\r\n        normalizedName\r\n      }\r\n      ... on TaskObjectiveQuestItem {\r\n        requiredKeys {\r\n          name\r\n          shortName\r\n          gridImageLink\r\n        }\r\n      }\r\n      ... on TaskObjectiveItem {\r\n        item {\r\n          name\r\n          shortName\r\n        }\r\n        items {\r\n          name\r\n          shortName\r\n        }\r\n        count\r\n        foundInRaid\r\n      }\r\n      ... on TaskObjectiveShoot {\r\n        targetNames\r\n        count\r\n      }\r\n    }\r\n    traderRequirements {\r\n      trader {\r\n        name\r\n        imageLink\r\n      }\r\n      requirementType\r\n      compareMethod\r\n      value\r\n    }\r\n  }\r\n}" }
            };
            return await Post<TasksGraphQLWrapper>(content);
        }
        private async Task<TResult?> Post<TResult>(object content)
        {
            var response = await _httpClient.PostAsJsonAsync("graphql", content);
            var responseContent = await response.Content.ReadAsStringAsync();
            return responseContent.Deserialize<TResult>();
        }
    }
}
