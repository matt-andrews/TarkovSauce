using System.Drawing;
using System.Drawing.Drawing2D;

namespace TarkovSauce.MapTools
{
    public interface IMap
    {
        string Name { get; }
        string BaseImage { get; }
        MapCoord GetPos(GameCoord gameCoord);
    }
    internal class Map : IMap
    {
        public string Name { get; }
        public string BaseImage { get; }
        public Anchor[] Anchors { get; }

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
            if(anchors.Length % 2 != 0)
            {
                throw new ArgumentException("Must include at least two anchors per level, ensure that you've set anchors correctly");
            }

            Name = name;
            BaseImage = baseImage;
            Anchors = anchors;
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
    }
}
