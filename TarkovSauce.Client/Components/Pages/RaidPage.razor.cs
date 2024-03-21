using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using TarkovSauce.Client.Data.Models.Remote;
using TarkovSauce.Client.Data.Providers;
using TarkovSauce.Client.Services;
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
        [Inject]
        public IJSRuntime JSRuntime { get; set; } = default!;
        [Inject]
        public ITasksService TasksService { get; set; } = default!;
        private IEnumerable<TaskModel>? _tasks;
        private MapTools.IMap? _map;
        private readonly List<string> _debugObjs = [];
        private string ImgSrc => _map is not null ? string.Format("data:image/png;base64,{0}", Convert.ToBase64String(_map.Image)) : "";

        private readonly List<PosObj> _currentPositions = [
            //new PosObj() { Coord = new GameCoord(0,0,0), FilterType = FilterType.CurrentPos, Sprite = "sprites/red-yourehere.png" },
            /*new PosObj() { Coord = new GameCoord(-151.7f, 2.9f, -258.4f), FilterType = FilterType.CurrentPos, Sprite = "sprites/red-yourehere.png" },
            new PosObj() { Coord = new GameCoord(-131.1f, 2.2f, -263.4f), FilterType = FilterType.CurrentPos, Sprite = "sprites/red-yourehere.png" },
            new PosObj() { Coord = new GameCoord(-117.4f, 1.5f, -253.1f), FilterType = FilterType.CurrentPos, Sprite = "sprites/red-yourehere.png" },
            new PosObj() { Coord = new GameCoord(-232.4f, 6.3f, -343.1f), FilterType = FilterType.CurrentPos, Sprite = "sprites/red-yourehere.png" }
            */
            ];
        private readonly List<PosObj> _customMarks = [];
        private string _selectedLayer = "Main";
        private FilterType _currentFilter
            = FilterType.ScavExtract
            | FilterType.PmcSpawns
            | FilterType.ScavSpawns
            | FilterType.CustomMarks;
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
            if (_map is null) return;
            _selectedLayer = layerName;
            await RebuildMap();
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
            _customMarks.Clear();
            if (MapTools.Maps.FirstOrDefault(f => f.NormalizedName.Contains(mapName)) == null)
            {
                return;
            }
            var builder = MapTools
                .GetMap(mapName)
                .GetBuilder();

            _map = await builder.Build(_currentFilter);
            _tasks = await TasksService.BuildCurrentQuests(_map.NormalizedName);
        }
        private async Task RebuildMap()
        {
            if (_map is null) return;
            var layer = _map.Layers.FirstOrDefault(f => f.Name == _selectedLayer);

            var builder = _map.GetBuilder(layer?.Layer ?? 0);
            foreach (var pos in _currentPositions.Concat(_customMarks))
            {
                builder.WithPos(pos.Coord, pos.Sprite, pos.FilterType);
            }
            _map = await builder.Build(_currentFilter);
            _tasks = await TasksService.BuildCurrentQuests(_map.NormalizedName);
            await InvokeAsync(StateHasChanged);
        }
        private async Task ChangeFilter(FilterType filter)
        {
            if (_currentFilter.HasFlag(filter))
                _currentFilter &= ~filter;
            else
                _currentFilter |= filter;
            await RebuildMap();
            StateHasChanged();
        }
        private async Task OnMapClick(MouseEventArgs args)
        {
            if (_map is null || _currentFilter.HasFlag(FilterType.CustomMarks)) return;
            var imgSize = await JSRuntime.InvokeAsync<float[]>("GetRaidImgSize");
            var pos = await _map.GetPos(new MapCoord((int)args.OffsetX, (int)args.OffsetY, 0), imgSize);
            _customMarks.Add(new PosObj()
            {
                Coord = pos,
                Sprite = "sprites/red-x.png",
                FilterType = FilterType.CustomMarks
            });
            await RebuildMap();
        }
        private async Task ClearCustomMarks()
        {
            _customMarks.Clear();
            await RebuildMap();
        }
        private void OnMapClickDebug(MouseEventArgs args)
        {
            if (_map is null) return;
            System.Diagnostics.Debug.WriteLine($"Map: ({args.OffsetX}, {args.OffsetY})");
            var gamecoord = _map.GetPos(new MapCoord((int)args.OffsetX, (int)args.OffsetY, 0));
            System.Diagnostics.Debug.WriteLine($"Game: {gamecoord}");
            var json = "{\"XYZ\":[" + $"{gamecoord.X},{gamecoord.Y},{gamecoord.Z}" + "],\"Sprite\":\"\"}";
            System.Diagnostics.Debug.WriteLine(json);
            _debugObjs.Add(json);
        }

        private void OnDebugDump()
        {
            var path = Path.Combine(FileSystem.Current.AppDataDirectory, DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss-ff") + ".json");
            File.WriteAllText(path, string.Join($",{Environment.NewLine}", _debugObjs));
            _debugObjs.Clear();
            System.Diagnostics.Debug.WriteLine("Saved debug data");
        }
        private class PosObj
        {
            public required GameCoord Coord { get; set; }
            public required string Sprite { get; set; }
            public required FilterType FilterType { get; set; }
        }
    }
}
