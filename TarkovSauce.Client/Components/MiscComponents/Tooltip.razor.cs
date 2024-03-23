using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TarkovSauce.Client.Components.MiscComponents
{
    public partial class Tooltip
    {
        [Parameter] public RenderFragment? ChildContent { get; set; }
        [Parameter] public string? Name { get; set; }
        [Parameter] public string? ShortName { get; set; }
        [Parameter] public string? Description { get; set; }
        [Parameter] public int Avg24hPrice { get; set; }
    }
}