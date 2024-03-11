using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TarkovSauce.Client.Components.Modal;

namespace TarkovSauce.Client.Components
{
    public static class Extensions
    {
        public static IServiceCollection AddTSComponentServices(this IServiceCollection services)
        {
            return services
                .AddSingleton<ITSToastService, TSToastService>();
        }
    }
}
