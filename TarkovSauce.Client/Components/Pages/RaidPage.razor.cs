using Microsoft.AspNetCore.Components;
using TarkovSauce.MapTools;

namespace TarkovSauce.Client.Components.Pages
{
    public partial class RaidPage
    {
        [Inject]
        public IMapTools MapTools { get; set; } = default!;
        private MapTools.IMap? _map;
        private string ImgSrc => _map is not null ? string.Format("data:image/png;base64,{0}", Convert.ToBase64String(_map.Image)) : "";

        private bool _mapShowPmcExtract = true;
        private bool _mapShowScavExtract;
        private bool _showCurrentPos = true;
        protected override async Task OnInitializedAsync()
        {
            await BuildMap();
        }
        private async Task BuildMap()
        {
            var builder = MapTools
                .GetMap("Customs")
                .GetBuilder();

            FilterType filter = GetFilterType();

            _map = await builder
                    .WithPos(new GameCoord(-125.4f, 0.8f, -2.4f), "sprites/red-yourehere.png", FilterType.CurrentPos)
                    .Build(filter);
        }
        private FilterType GetFilterType()
        {
            FilterType filter = FilterType.None;
            if (!_mapShowPmcExtract)
                filter |= FilterType.PmcExtract;
            if (!_mapShowScavExtract)
                filter |= FilterType.ScavExtract;
            if (!_showCurrentPos)
                filter |= FilterType.CurrentPos;
            return filter;
        }
        private async Task OnMapShowPmcExtract(bool val)
        {
            _mapShowPmcExtract = val;
            await BuildMap();
            StateHasChanged();
        }
        private async Task OnMapShowScavExtract(bool val)
        {
            _mapShowScavExtract = val;
            await BuildMap();
            StateHasChanged();
        }
        private async Task OnMapCurrentPos(bool val)
        {
            _showCurrentPos = val;
            await BuildMap();
            StateHasChanged();
        }
    }
}
