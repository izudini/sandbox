using System;
using System.Collections.Concurrent;
using System.Drawing;

namespace KC_135
{
    public class Triangle
    {
        public PointF Location { get; set; }
        public float WidthDegrees { get; set; }
        public float Rotation { get; set; }
        public Color Color { get; set; }
        public bool IsSelected { get; set; }
        public bool IsConsoleVisible { get; set; }
        public ConcurrentQueue<string> MessageQueue { get; set; }
        public SensorMode CurrentMode { get; set; }

        public Triangle(PointF location, float widthDegrees, float rotation, Color color)
        {
            Location = location;
            WidthDegrees = widthDegrees;
            Rotation = rotation;
            Color = color;
            IsConsoleVisible = false;
            MessageQueue = new ConcurrentQueue<string>();
            CurrentMode = SensorMode.Off;
        }

        public Color GetSensorModeColor()
        {
            switch (CurrentMode)
            {
                case SensorMode.Off:
                    return Color.White;
                case SensorMode.Initializing:
                    return Color.Yellow;
                case SensorMode.Operate:
                    return Color.LightGreen;
                case SensorMode.Degraded:
                    return Color.Black;
                case SensorMode.Declaring:
                    // Flash between dark red and bright red at 2 Hz (500ms cycle, 250ms per color)
                    long totalMilliseconds = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
                    int flashInterval = (int)(totalMilliseconds / 250); // Flash every 250ms for 2 Hz
                    return flashInterval % 2 == 0 ? Color.DarkRed : Color.Red;
                default:
                    return Color.White;
            }
        }

        public RectangleF GetCameraBody()
        {
            float radius = 15; // Fixed radius for camera body
            
            return new RectangleF(
                Location.X - radius,
                Location.Y - radius,
                radius * 2,
                radius * 2
            );
        }

        public RectangleF GetCameraLens()
        {
            return new RectangleF(
                this.Location.X - 10,
                this.Location.Y - 10,
                20,
                20
            );
        }

        public PointF[] GetFieldOfView()
        {
            float fovAngle = WidthDegrees;
            float fovDistance = 150; // Fixed distance for field of view
            int numSegments = 20; // Number of points to create smooth circular sector
            
            // Subtract 90 degrees to make 0° point up instead of right
            double startRadians = (Rotation - 90 - fovAngle / 2) * Math.PI / 180.0;
            double endRadians = (Rotation - 90 + fovAngle / 2) * Math.PI / 180.0;
            
            // Create array with center point + arc points + closing point
            PointF[] fovPoints = new PointF[numSegments + 2];
            
            // Start from center of circle
            fovPoints[0] = Location;
            
            // Create arc points
            for (int i = 0; i <= numSegments; i++)
            {
                double t = (double)i / numSegments;
                double currentRadians = startRadians + (endRadians - startRadians) * t;
                
                fovPoints[i + 1] = new PointF(
                    Location.X + (float)(fovDistance * Math.Cos(currentRadians)),
                    Location.Y + (float)(fovDistance * Math.Sin(currentRadians))
                );
            }
            
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
            // Check if point is within the sector (field of view)
            return IsPointInSector(point);
        }

        private bool IsPointInSector(PointF point)
        {
            float fovAngle = WidthDegrees;
            float fovDistance = 150; // Fixed distance for sector
            
            // Calculate distance from center to point
            float dx = point.X - Location.X;
            float dy = point.Y - Location.Y;
            float distance = (float)Math.Sqrt(dx * dx + dy * dy);
            
            // If point is too far from center, it's not in the sector
            if (distance > fovDistance)
                return false;
            
            // Calculate angle from center to point
            double pointAngle = Math.Atan2(dy, dx) * 180.0 / Math.PI;
            
            // Normalize angles to match our coordinate system (0° points up)
            double normalizedPointAngle = pointAngle + 90;
            double normalizedRotation = Rotation;
            
            // Normalize angles to [0, 360)
            while (normalizedPointAngle < 0) normalizedPointAngle += 360;
            while (normalizedPointAngle >= 360) normalizedPointAngle -= 360;
            while (normalizedRotation < 0) normalizedRotation += 360;
            while (normalizedRotation >= 360) normalizedRotation -= 360;
            
            // Calculate sector bounds
            double startAngle = normalizedRotation - fovAngle / 2;
            double endAngle = normalizedRotation + fovAngle / 2;
            
            // Handle angle wraparound
            if (startAngle < 0)
            {
                startAngle += 360;
                endAngle += 360;
                if (normalizedPointAngle < 180) normalizedPointAngle += 360;
            }
            else if (endAngle >= 360)
            {
                startAngle -= 360;
                endAngle -= 360;
                if (normalizedPointAngle > 180) normalizedPointAngle -= 360;
            }
            
            // Check if point angle is within sector bounds
            return normalizedPointAngle >= startAngle && normalizedPointAngle <= endAngle;
        }
    }
}