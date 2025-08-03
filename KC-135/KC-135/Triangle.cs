using System;
using System.Collections.Concurrent;
using System.Drawing;

namespace KC_135
{
    public class Triangle
    {
        public PointF Location { get; set; }
        public float Base { get; set; }
        public float Height { get; set; }
        public float Rotation { get; set; }
        public Color Color { get; set; }
        public bool IsSelected { get; set; }
        public bool IsConsoleVisible { get; set; }
        public ConcurrentQueue<string> MessageQueue { get; set; }

        public Triangle(PointF location, float baseWidth, float height, float rotation, Color color)
        {
            Location = location;
            Base = baseWidth;
            Height = height;
            Rotation = rotation;
            Color = color;
            IsConsoleVisible = false;
            MessageQueue = new ConcurrentQueue<string>();
        }

        public PointF[] GetPoints()
        {
            float halfBase = Base / 2;
            PointF[] points = new PointF[]
            {
                new PointF(0, -Height * 2/3),
                new PointF(-halfBase, Height * 1/3),
                new PointF(halfBase, Height * 1/3)
            };

            double radians = Rotation * Math.PI / 180.0;
            float cos = (float)Math.Cos(radians);
            float sin = (float)Math.Sin(radians);

            for (int i = 0; i < points.Length; i++)
            {
                float x = points[i].X;
                float y = points[i].Y;
                points[i] = new PointF(
                    Location.X + (x * cos - y * sin),
                    Location.Y + (x * sin + y * cos)
                );
            }

            return points;
        }

        public bool ContainsPoint(PointF point)
        {
            PointF[] points = GetPoints();
            
            int j = points.Length - 1;
            bool inside = false;
            
            for (int i = 0; i < points.Length; j = i++)
            {
                if (((points[i].Y > point.Y) != (points[j].Y > point.Y)) &&
                    (point.X < (points[j].X - points[i].X) * (point.Y - points[i].Y) / (points[j].Y - points[i].Y) + points[i].X))
                {
                    inside = !inside;
                }
            }
            
            return inside;
        }
    }
}