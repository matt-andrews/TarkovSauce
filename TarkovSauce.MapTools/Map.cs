using SkiaSharp;
using System.Drawing;
using System.Drawing.Drawing2D;
using static TarkovSauce.MapTools.IMap;

namespace TarkovSauce.MapTools
{
    public interface IMap
    {
        byte[] Image { get; }
        string Name { get; }
        MapCoord GetPos(GameCoord gameCoord);
        IMapBuilder GetBuilder();

        public interface IMapBuilder
        {
            IMapBuilder WithPos(GameCoord pos, string sprite, FilterType filterType);
            IMapBuilder WithPos(GameCoord pos, string sprite, FilterType filterType, SpriteText title);
            Task<IMap> Build(FilterType filterType);
        }
    }
    internal class Map : IMap
    {
        public byte[] Image { get; private set; } = [];
        public string Name { get; }
        public Anchor[] Anchors { get; }
        private readonly List<PosObj> _defaultPositions = [];
        private readonly string _baseImage;
        private MapToolsHttpClient? _httpClient;

        public Map(string name, string baseImage, Anchor[] anchors)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException(nameof(name));
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
        }

        public IMap.IMapBuilder GetBuilder()
        {
            return new MapBuilder(this);
        }

        internal Map AddHttpClient(MapToolsHttpClient httpClient)
        {
            _httpClient = httpClient;
            return this;
        }

        internal async Task<IMap> Build(List<PosObj> positions, FilterType filterType)
        {
            if (_httpClient is null)
                throw new Exception("Http Client is missing!");

            var bitmap = SKBitmap.Decode(await _httpClient.GetImage(_baseImage));
            var newBitmap = new SKBitmap(bitmap.Width, bitmap.Height);
            using var canvas = new SKCanvas(newBitmap);
            canvas.DrawBitmap(bitmap, new SKPoint(0, 0));
            foreach (var pos in _defaultPositions.Concat(positions))
            {
                if (filterType != FilterType.None && filterType.HasFlag(pos.FilterType))
                    continue;
                var mapPos = GetPos(pos.Coord);
                var sprite = SKBitmap.Decode(await _httpClient.GetImage(pos.Sprite));
                canvas.DrawBitmap(sprite, new SKPoint(mapPos.X, mapPos.Y - sprite.Height));
                if (pos.Title is not null)
                {
                    var textCoord = new SKPoint(mapPos.X, mapPos.Y + 32);
                    // Draw stroke
                    var strokePaint = new SKPaint()
                    {
                        Color = SKColor.Parse("#000"),
                        TextSize = 32,
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
                        TextSize = 32,
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
            _defaultPositions.Add(
                new PosObj(
                    new GameCoord(obj.XYZ[0], obj.XYZ[1], obj.XYZ[2]),
                    obj.Sprite,
                    new SpriteText(obj.Title, Color.FromArgb(obj.TitleColor[0], obj.TitleColor[1], obj.TitleColor[2])),
                    filterType
                    )
                );
        }
        internal void AddDefaultPos(IEnumerable<IPos> objs, FilterType filterType)
        {
            foreach (var obj in objs)
                AddDefaultPos(obj, filterType);
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

        internal class MapBuilder(Map _map) : IMapBuilder
        {
            private readonly List<PosObj> _list = [];
            public async Task<IMap> Build(FilterType filterType)
            {
                return await _map.Build(_list, filterType);
            }

            public IMapBuilder WithPos(GameCoord pos, string sprite, FilterType filterType)
            {
                _list.Add(new PosObj(pos, sprite, null, filterType));
                return this;
            }
            public IMapBuilder WithPos(GameCoord pos, string sprite, FilterType filterType, SpriteText title)
            {
                _list.Add(new PosObj(pos, sprite, title, filterType));
                return this;
            }
        }
        internal class PosObj(GameCoord coord, string sprite, SpriteText? title, FilterType filterType)
        {
            public GameCoord Coord { get; } = coord;
            public string Sprite { get; } = sprite;
            public SpriteText? Title { get; } = title;
            public FilterType FilterType { get; } = filterType;
        }
    }
    public class SpriteText(string text, Color color)
    {
        public string Text { get; } = text;
        public Color Color { get; } = color;
    }
}
