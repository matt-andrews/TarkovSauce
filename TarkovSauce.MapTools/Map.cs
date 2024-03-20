using SkiaSharp;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Reflection.Emit;
using static TarkovSauce.MapTools.IMap;

namespace TarkovSauce.MapTools
{
    public interface IMap
    {
        byte[] Image { get; }
        string Name { get; }
        string NormalizedName { get; }
        LayerObj[] Layers { get; }
        MapCoord GetPos(GameCoord gameCoord);
        GameCoord GetPos(MapCoord mapCoord);
        Task<GameCoord> GetPos(MapCoord mapCoord, float[] scaledSize);
        IMapBuilder GetBuilder(int layer = 0);

        public interface IMapBuilder
        {
            IMapBuilder WithPos(GameCoord pos, string sprite, FilterType filterType, int layer = -1);
            IMapBuilder WithPos(GameCoord pos, string sprite, FilterType filterType, SpriteText title, int layer = -1);
            Task<IMap> Build(FilterType filterType);
        }
    }
    internal class Map : IMap
    {
        public byte[] Image { get; private set; } = [];
        public string Name { get; }
        public string NormalizedName { get; }
        public Anchor[] Anchors { get; }
        private readonly List<PosObj> _defaultPositions = [];
        private readonly string _baseImage;
        private readonly bool _invertedXZ;
        private MapToolsHttpClient? _httpClient;
        public LayerObj[] Layers { get; }

        public Map(string name, string normalizedName, string baseImage, Anchor[] anchors, LayerObj[] layers, bool invertedXZ)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException(nameof(name));
            }
            if (string.IsNullOrWhiteSpace(normalizedName))
            {
                throw new ArgumentNullException(nameof(normalizedName));
            }
            if (string.IsNullOrWhiteSpace(baseImage))
            {
                throw new ArgumentNullException(nameof(baseImage));
            }
            if (anchors.Length % 2 != 0)
            {
                throw new ArgumentException("Must include at least two anchors per level; ensure that you've set anchors correctly");
            }

            Name = name;
            _baseImage = baseImage;
            Anchors = anchors;
            if (invertedXZ)
            {
                foreach (var anchor in Anchors)
                {
                    anchor.Game = anchor.Game.Invert();
                }
            }
            NormalizedName = normalizedName;
            Layers = layers;
            _invertedXZ = invertedXZ;
            if (Layers.Length != 0)
            {
                Layers = [new LayerObj() { Name = "Main", Map = _baseImage }, .. layers];
            }
        }

        public IMap.IMapBuilder GetBuilder(int layer = 0)
        {
            return new MapBuilder(this, layer);
        }

        internal Map AddHttpClient(MapToolsHttpClient httpClient)
        {
            _httpClient = httpClient;
            return this;
        }

        internal async Task<IMap> Build(List<PosObj> positions, FilterType filterType, int layer)
        {
            if (_httpClient is null)
                throw new Exception("Http Client is missing!");

            SKBitmap bitmap = await GetMap(layer);

            var newBitmap = new SKBitmap(bitmap.Width, bitmap.Height);
            using var canvas = new SKCanvas(newBitmap);
            canvas.DrawBitmap(bitmap, new SKPoint(0, 0));
            foreach (var pos in _defaultPositions.Concat(positions))
            {
                var coord = pos.Coord;
                if (filterType != FilterType.None && filterType.HasFlag(pos.FilterType))
                    continue;
                if (pos.Layer != -1 && pos.Layer != layer)
                    continue;
                if (_invertedXZ)
                    coord = coord.Invert();
                var mapPos = GetPos(coord);
                var sprite = SKBitmap.Decode(await _httpClient.GetImage(pos.Sprite));
                if (pos.FilterType == FilterType.CustomMarks)
                {
                    mapPos = mapPos.GetCenter(sprite.Width, sprite.Height);
                }
                canvas.DrawBitmap(sprite, new SKPoint(mapPos.X, mapPos.Y - sprite.Height));
                if (pos.Title is not null)
                {
                    var textCoord = new SKPoint(mapPos.X, mapPos.Y + 24);
                    // Draw stroke
                    var strokePaint = new SKPaint()
                    {
                        Color = SKColor.Parse("#000"),
                        TextSize = 24,
                        Style = SKPaintStyle.Stroke,
                        IsAntialias = true,
                        StrokeWidth = 6,
                        IsStroke = true
                    };
                    // Draw text
                    canvas.DrawText(pos.Title.Text, textCoord, strokePaint);
                    var textPaint = new SKPaint()
                    {
                        Color = SKColor.Parse(pos.Title.Color.ToHex()),
                        TextSize = 24,
                        FakeBoldText = true,
                        Style = SKPaintStyle.Fill,
                        IsAntialias = true,
                        StrokeWidth = 1,

                    };
                    canvas.DrawText(pos.Title.Text, textCoord, textPaint);
                }
            }
            using var ms = new MemoryStream();
            newBitmap.Encode(ms, SKEncodedImageFormat.Png, 80);
            ms.Position = 0;
            Image = ms.ToArray();
            return this;
        }

        internal void AddDefaultPos(IPos obj, FilterType filterType)
        {
            if (!string.IsNullOrWhiteSpace(obj.Title))
            {
                _defaultPositions.Add(
                    new PosObj(
                        new GameCoord(obj.XYZ[0], obj.XYZ[1], obj.XYZ[2]),
                        obj.Sprite,
                        new SpriteText(obj.Title, Color.FromArgb(obj.TitleColor[0], obj.TitleColor[1], obj.TitleColor[2])),
                        filterType,
                        obj.Layer
                        )
                    );
            }
            else
            {
                _defaultPositions.Add(
                    new PosObj(
                        new GameCoord(obj.XYZ[0], obj.XYZ[1], obj.XYZ[2]),
                        obj.Sprite,
                        null,
                        filterType,
                        obj.Layer
                        )
                    );
            }
        }
        private async Task<SKBitmap> GetMap(int layer)
        {
            if (_httpClient is null)
                throw new Exception("Http client is null");
            SKBitmap bitmap;
            if (Layers.Length > 0)
            {
                bitmap = SKBitmap.Decode(await _httpClient.GetImage(Layers.FirstOrDefault(f => f.Layer == layer)?.Map ?? Layers.First().Map));
            }
            else
            {
                bitmap = SKBitmap.Decode(await _httpClient.GetImage(_baseImage));
            }
            return bitmap;
        }
        internal void AddDefaultPos(IEnumerable<IPos> objs, FilterType filterType)
        {
            foreach (var obj in objs)
                AddDefaultPos(obj, filterType);
        }
        public GameCoord GetPos(MapCoord mapCoord)
        {
            PointF[] realWorldPoints = Anchors.Select(s => s.Game.ToPointF()).ToArray();
            PointF[] imagePoints = Anchors.Select(s => s.Map.ToPointF()).ToArray();

            Matrix transformationMatrix = CalculateTransformationMatrix(imagePoints, realWorldPoints);

            PointF realCoordinate = mapCoord.ToPointF();
            PointF mappedPixelCoordinate = MapCoordinate(realCoordinate, transformationMatrix);

            var coord = mappedPixelCoordinate.ToGameCoord();
            if (_invertedXZ)
                coord = coord.Invert();
            return coord;
        }
        public async Task<GameCoord> GetPos(MapCoord mapCoord, float[] scaledSize)
        {
            //find scale in percentage, apply to coords, do work?
            SKBitmap map = await GetMap(0);
            float xDif = map.Width / scaledSize[0];
            float yDif = map.Height / scaledSize[1];
            return GetPos(new MapCoord((int)(mapCoord.X * xDif), (int)(mapCoord.Y * yDif), 0));
        }
        /// <summary>
        /// Get the map coordinates from the game coordinates
        /// </summary>
        /// <param name="gameCoord"></param>
        /// <returns>Map Coordinates</returns>
        public MapCoord GetPos(GameCoord gameCoord)
        {
            PointF[] realWorldPoints = Anchors.Select(s => s.Game.ToPointF()).ToArray();
            PointF[] imagePoints = Anchors.Select(s => s.Map.ToPointF()).ToArray();

            Matrix transformationMatrix = CalculateTransformationMatrix(realWorldPoints, imagePoints);

            PointF realCoordinate = gameCoord.ToPointF();
            PointF mappedPixelCoordinate = MapCoordinate(realCoordinate, transformationMatrix);

            return mappedPixelCoordinate.ToMapCoord();
        }
#pragma warning disable CA1416 // Validate platform compatibility
        private static Matrix CalculateTransformationMatrix(PointF[] sourcePoints, PointF[] destPoints)
        {
            Matrix matrix = new();

            matrix.Reset();
            matrix.Translate(-sourcePoints[0].X, -sourcePoints[0].Y, MatrixOrder.Append);
            matrix.Scale((destPoints[1].X - destPoints[0].X) / (sourcePoints[1].X - sourcePoints[0].X),
                         (destPoints[1].Y - destPoints[0].Y) / (sourcePoints[1].Y - sourcePoints[0].Y),
                         MatrixOrder.Append);
            matrix.Translate(destPoints[0].X, destPoints[0].Y, MatrixOrder.Append);

            return matrix;
        }
        private static PointF MapCoordinate(PointF point, Matrix transformationMatrix)
        {
            PointF[] points = [point];
            transformationMatrix.TransformPoints(points);
            return points[0];
        }
#pragma warning restore CA1416 // Validate platform compatibility

        internal class MapBuilder(Map _map, int _layer) : IMapBuilder
        {
            private readonly List<PosObj> _list = [];
            public async Task<IMap> Build(FilterType filterType)
            {
                return await _map.Build(_list, filterType, _layer);
            }

            public IMapBuilder WithPos(GameCoord pos, string sprite, FilterType filterType, int layer = -1)
            {
                _list.Add(new PosObj(pos, sprite, null, filterType, layer));
                return this;
            }
            public IMapBuilder WithPos(GameCoord pos, string sprite, FilterType filterType, SpriteText title, int layer = -1)
            {
                _list.Add(new PosObj(pos, sprite, title, filterType, layer));
                return this;
            }
        }
        internal class PosObj(GameCoord coord, string sprite, SpriteText? title, FilterType filterType, int layer)
        {
            public GameCoord Coord { get; internal set; } = coord;
            public string Sprite { get; } = sprite;
            public SpriteText? Title { get; } = title;
            public FilterType FilterType { get; } = filterType;
            public int Layer { get; } = layer;
        }
    }
    public class SpriteText(string text, Color color)
    {
        public string Text { get; } = text;
        public Color Color { get; } = color;
    }
}
