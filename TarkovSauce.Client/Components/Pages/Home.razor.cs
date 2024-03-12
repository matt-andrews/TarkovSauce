using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Configuration;
using TarkovSauce.Client.HttpClients;

namespace TarkovSauce.Client.Components.Pages
{
    public partial class Home
    {
        [Inject]
        public IConfiguration Configuration { get; set; } = default!;
        [Inject]
        public ITarkovTrackerHttpClient HttpClient { get; set; } = default!;
        private string _httpClientResult = "null";
        protected override async Task OnInitializedAsync()
        {
            Data.Models.Remote.TokenResponse? x = await HttpClient.TestToken();
            if (x is not null)
            {
                _httpClientResult = x.Serialize();
            }
            await base.OnInitializedAsync();
        }
        private string Test()
        {
            var val = Configuration["Settings:Test"];
            if (val is null)
                return "N/A";
            return val;
        }
    }
}
