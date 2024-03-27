using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Text;
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
        private string? AuthToken => _config["Settings:TarkovTrackerKey"];
        private TokenResponse? _token;
        public async Task<TokenResponse?> TestToken()
        {
            if (string.IsNullOrWhiteSpace(AuthToken))
                return null;
            if (_token is not null && _token.Token == AuthToken)
                return _token;

            HttpRequestMessage request = CreateRequest(HttpMethod.Get, "api/v2/token");
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
                throw new Exception(resultString);
            }
        }
        public async Task<ProgressResponse?> GetProgress()
        {
            if (_token is null && await TestToken() is null)
                return null;
            HttpRequestMessage request = CreateRequest(HttpMethod.Get, "api/v2/progress");
            HttpResponseMessage response = await _httpClient.SendAsync(request);
            string resultString = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
            {
                return resultString.Deserialize<ProgressResponse>() ?? null;
            }
            else
            {
                _logger.LogError(resultString);
                throw new Exception(resultString);
            }
        }
        public async Task<string?> SetTaskStatusBatch(List<TaskStatusBody> body)
        {
            if (_token is null && await TestToken() is null)
                return null;
            HttpRequestMessage request = CreateRequest(HttpMethod.Post, "api/v2/progress/tasks");
            request.Content = new StringContent(body.Serialize(), Encoding.UTF8, "application/json");
            HttpResponseMessage response = await _httpClient.SendAsync(request);
            string resultString = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
            {
                return resultString;
            }
            else
            {
                _logger.LogError(resultString);
                throw new Exception(resultString);
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
