using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Configuration;

namespace TarkovSauce.Client.Components.Pages
{
    public partial class Home
    {
        [Inject]
        public IConfiguration Configuration { get; set; } = default!;
        private string Test()
        {
            var val = Configuration["Settings:Test"];
            if (val is null)
                return "N/A";
            return val;
        }
    }
}
