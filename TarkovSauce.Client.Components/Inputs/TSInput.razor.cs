using Microsoft.AspNetCore.Components;
using System;

namespace TarkovSauce.Client.Components.Inputs
{
    public partial class TSInput
    {
        private readonly string _id = Guid.NewGuid().ToString();
        [Parameter]
        public string Placeholder { get; set; } = "";
        [Parameter]
        public string Value { get; set; } = "";
        [Parameter]
        public EventCallback<string> ValueChanged { get; set; }
    }
}