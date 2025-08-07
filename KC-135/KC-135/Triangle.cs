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

        public RectangleF GetCameraBody()
        {
            float radius = Math.Min(Base, Height) / 2;
            
            return new RectangleF(
                Location.X - radius,
                Location.Y - radius,
                radius * 2,
                radius * 2
            );
        }

        public RectangleF GetCameraLens()
        {
            float lensRadius = Math.Min(Base, Height) * 0.3f;
            float offsetDistance = Base * 0.25f;
            
            // Subtract 90 degrees to make 0° point up instead of right
            double radians = (Rotation - 90) * Math.PI / 180.0;
            float cos = (float)Math.Cos(radians);
            float sin = (float)Math.Sin(radians);
            
            float lensX = Location.X + (offsetDistance * cos);
            float lensY = Location.Y + (offsetDistance * sin);
            
            return new RectangleF(
                lensX - lensRadius / 2,
                lensY - lensRadius / 2,
                lensRadius,
                lensRadius
            );
        }

        public PointF[] GetFieldOfView()
        {
            float fovAngle = 60.0f;
            float fovDistance = Base * 1.5f;
            
            // Subtract 90 degrees to make 0° point up instead of right
            double leftRadians = (Rotation - 90 - fovAngle / 2) * Math.PI / 180.0;
            double rightRadians = (Rotation - 90 + fovAngle / 2) * Math.PI / 180.0;
            
            PointF[] fovPoints = new PointF[4];
            
            // Start from center of circle
            fovPoints[0] = Location;
            fovPoints[1] = new PointF(
                Location.X + (float)(fovDistance * Math.Cos(leftRadians)),
                Location.Y + (float)(fovDistance * Math.Sin(leftRadians))
            );
            fovPoints[2] = new PointF(
                Location.X + (float)(fovDistance * Math.Cos(rightRadians)),
                Location.Y + (float)(fovDistance * Math.Sin(rightRadians))
            );
            fovPoints[3] = Location;
            
            return fovPoints;
        }

        public PointF[] GetPoints()
        {
            RectangleF body = GetCameraBody();
            return new PointF[]
            {
                new PointF(body.Left, body.Top),
                new PointF(body.Right, body.Top),
                new PointF(body.Right, body.Bottom),
                new PointF(body.Left, body.Bottom)
            };
        }

        public RectangleF GetBounds()
        {
            RectangleF body = GetCameraBody();
            RectangleF lens = GetCameraLens();
            
            float left = Math.Min(body.Left, lens.Left);
            float top = Math.Min(body.Top, lens.Top);
            float right = Math.Max(body.Right, lens.Right);
            float bottom = Math.Max(body.Bottom, lens.Bottom);
            
            return new RectangleF(left, top, right - left, bottom - top);
        }

        public bool ContainsPoint(PointF point)
        {
            RectangleF bounds = GetBounds();
            return bounds.Contains(point);
        }
    }
}