using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using TarkovSauce.Client.Data.Providers;
using TarkovSauce.Client.Utils;
using TarkovSauce.MapTools;

namespace TarkovSauce.Client.Components.Pages
{
    public partial class RaidPage
    {
        [Inject]
        public AppDataJson AppData { get; set; } = default!;
        [Inject]
        public IMapTools MapTools { get; set; } = default!;
        [Inject]
        public ISelectedMapProvider SelectedMapProvider { get; set; } = default!;
        [Inject]
        public IScreenshotWatcherProvider ScreenshotWatcherProvider { get; set; } = default!;
        private MapTools.IMap? _map;
        private string ImgSrc => _map is not null ? string.Format("data:image/png;base64,{0}", Convert.ToBase64String(_map.Image)) : "";

        private readonly List<PosObj> _currentPositions = [];
        private string _selectedLayer = "Main";
        private bool _mapShowPmcExtract = true;
        private bool _mapShowScavExtract;
        private bool _showCurrentPos = true;
        private bool _showPmcSpawns;
        private bool _showScavSpawns;
        protected override void OnInitialized()
        {
            SelectedMapProvider.OnStateChanged = async () =>
            {
                await BuildMap(SelectedMapProvider.Map);
                await InvokeAsync(StateHasChanged);
            };
            ScreenshotWatcherProvider.OnStateChanged = async () =>
            {
                if (ScreenshotWatcherProvider.Position is null) return;
                _currentPositions.Clear();
                _currentPositions.Add(new PosObj()
                {
                    Coord = ScreenshotWatcherProvider.Position.Value,
                    Sprite = "sprites/red-yourehere.png",
                    FilterType = FilterType.CurrentPos
                });
                await RebuildMap();
            };
        }
        private async Task SelectLayer(string layerName)
        {
            var layer = _map?.Layers.FirstOrDefault(f=>f.Name == layerName);
            if (layer is null) return;
            _selectedLayer = layer.Name;
            await RebuildMap(layer.Layer);
        }
        private async Task SelectMap(string mapName)
        {
            var normal = MapTools.Maps.FirstOrDefault(f => f.Name == mapName)?.NormalizedName;
            if (!string.IsNullOrWhiteSpace(normal))
                await BuildMap(normal);
        }
        private string GetImgCss()
        {
            if (_map is null) return "";
            return _map.NormalizedName;
        }
        private async Task BuildMap(string mapName)
        {
            if (MapTools.Maps.FirstOrDefault(f => f.NormalizedName == mapName) == null)
            {
                return;
            }
            var builder = MapTools
                .GetMap(mapName)
                .GetBuilder();

            FilterType filter = GetFilterType();
            _map = await builder.Build(filter);
        }
        private async Task RebuildMap(int layer = 0)
        {
            if (_map is null) return;
            FilterType filter = GetFilterType();
            var builder = _map.GetBuilder(layer);
            foreach (var pos in _currentPositions)
            {
                builder.WithPos(pos.Coord, pos.Sprite, pos.FilterType);
            }
            _map = await builder.Build(filter);
            await InvokeAsync(StateHasChanged);
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
            if (!_showPmcSpawns)
                filter |= FilterType.PmcSpawns;
            if (!_showScavSpawns)
                filter |= FilterType.ScavSpawns;
            return filter;
        }
        private async Task OnMapShowPmcExtract(bool val)
        {
            _mapShowPmcExtract = val;
            await RebuildMap();
            StateHasChanged();
        }
        private async Task OnMapShowScavExtract(bool val)
        {
            _mapShowScavExtract = val;
            await RebuildMap();
            StateHasChanged();
        }
        private async Task OnMapCurrentPos(bool val)
        {
            _showCurrentPos = val;
            await RebuildMap();
            StateHasChanged();
        }
        private async Task OnShowPmcSpawns(bool val)
        {
            _showPmcSpawns = val;
            await RebuildMap();
            StateHasChanged();
        }
        private async Task OnShowScavSpawns(bool val)
        {
            _showScavSpawns = val;
            await RebuildMap();
            StateHasChanged();
        }
        private void OnMapClick(MouseEventArgs args)
        {
            if (_map is null) return;
            System.Diagnostics.Debug.WriteLine($"Map: ({args.OffsetX}, {args.OffsetY})");
            var gamecoord = _map.GetPos(new MapCoord((int)args.OffsetX, (int)args.OffsetY, 0));
            System.Diagnostics.Debug.WriteLine($"Game: {gamecoord}");
            System.Diagnostics.Debug.WriteLine("{\"XYZ\":[" + $"{gamecoord.X},{gamecoord.Y},{gamecoord.Z}" + "],\"Sprite\":\"\"}");
        }
        private class PosObj
        {
            public required GameCoord Coord { get; set; }
            public required string Sprite { get; set; }
            public required FilterType FilterType { get; set; }
        }
    }
}
