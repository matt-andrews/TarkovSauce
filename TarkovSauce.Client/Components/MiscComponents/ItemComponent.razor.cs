using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TarkovSauce.Client.Data.Models.Remote;
using TarkovSauce.Client.Data.Providers;

namespace TarkovSauce.Client.Components.MiscComponents
{
    public partial class ItemComponent
    {
        [Parameter]
        [EditorRequired]
        public FleaEventModel Item { get; set; } = default!;
    }
}
