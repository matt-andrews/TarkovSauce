using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using TarkovSauce.Client.Data.Models.Remote;

namespace TarkovSauce.Client.HttpClients
{
    public interface ITarkovTrackerHttpClient
    {
        Task<TokenResponse?> TestToken();
        Task<ProgressResponse?> GetProgress();
        Task<string?> SetTaskStatusBatch(List<TaskStatusBody> body);
    }
    public class TarkovTrackerHttpClient(HttpClient _httpClient, IConfiguration _config, ILogger<TarkovTrackerHttpClient> _logger)
        : ITarkovTrackerHttpClient
    {
        private readonly static string _baseUri = "https://tarkovtracker.io/api/v2";
        private string? AuthToken => _config["Settings:TarkovTrackerKey"];
        private TokenResponse? _token;
        public async Task<TokenResponse?> TestToken()
        {
            if (string.IsNullOrWhiteSpace(AuthToken))
                return null;
            if (_token is not null && _token.Token == AuthToken)
                return _token;

            HttpRequestMessage request = CreateRequest(HttpMethod.Get, _baseUri + "/token");
            HttpResponseMessage response = await _httpClient.SendAsync(request);
            string resultString = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
            {
                _token = resultString.Deserialize<TokenResponse>() ?? null;
                return _token;
            }
            else
            {
                _logger.LogError(resultString);
                return null;
            }
        }
        public async Task<ProgressResponse?> GetProgress()
        {
            if (string.IsNullOrWhiteSpace(AuthToken))
                return null;
            HttpRequestMessage request = CreateRequest(HttpMethod.Get, _baseUri + "/progress");
            HttpResponseMessage response = await _httpClient.SendAsync(request);
            string resultString = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
            {
                return resultString.Deserialize<ProgressResponse>() ?? null;
            }
            else
            {
                _logger.LogError(resultString);
                return null;
            }
        }
        public async Task<string?> SetTaskStatusBatch(List<TaskStatusBody> body)
        {
            if (string.IsNullOrWhiteSpace(AuthToken))
                return null;
            HttpRequestMessage request = CreateRequest(HttpMethod.Post, _baseUri + "/progress/tasks");
            request.Content = new StringContent(body.Serialize());
            HttpResponseMessage response = await _httpClient.SendAsync(request);
            string resultString = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
            {
                return resultString;
            }
            else
            {
                _logger.LogError(resultString);
                return null;
            }
        }
        private HttpRequestMessage CreateRequest(HttpMethod method, string uri)
        {
            var request = new HttpRequestMessage(method, uri);
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("bearer", AuthToken);
            return request;
        }
    }
}
