using System.Drawing;

namespace TarkovSauce.MapTools
{
    public readonly struct GameCoord(float x, float y, float z)
    {
        public float X { get; } = x;
        public float Y { get; } = y;
        public float Z { get; } = z;
        public PointF ToPointF()
        {
            return new PointF(X, Z);
        }
        public override string ToString()
        {
            return $"({X}, {Y}, {Z})";
        }
        public GameCoord Invert()
        {
            return new(Z, Y, X);
        }
    }
    public readonly struct MapCoord(int x, int y, int z)
    {
        public int X { get; } = x;
        public int Y { get; } = y;
        public int Z { get; } = z;
        public PointF ToPointF()
        {
            return new PointF(X, Y);
        }
        public override string ToString()
        {
            return $"({X}, {Y}, {Z})";
        }
        public MapCoord GetCenter(int width, int height)
        {
            return new(X - (width / 2), Y + (height / 2), 0);
        }
    }
    internal static class CoordExtensions
    {
        public static MapCoord ToMapCoord(this PointF point)
        {
            return new MapCoord((int)Math.Round(point.X), (int)Math.Round(point.Y), 0);
        }
        public static GameCoord ToGameCoord(this PointF point)
        {
            return new GameCoord(point.X, 0, point.Y);
        }
    }
}
