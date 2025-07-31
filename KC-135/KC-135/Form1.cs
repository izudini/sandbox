using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

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

        public Triangle(PointF location, float baseWidth, float height, float rotation, Color color)
        {
            Location = location;
            Base = baseWidth;
            Height = height;
            Rotation = rotation;
            Color = color;
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

    public partial class Form1 : Form
    {
        private List<Triangle> triangles;
        private Triangle draggedTriangle;
        private Triangle selectedTriangle;
        private bool isDragging;
        private bool isRotating;
        private PointF lastMousePosition;
        private PointF dragOffset;

        public Form1()
        {
            InitializeComponent();
            
            SetStyle(ControlStyles.AllPaintingInWmPaint | 
                     ControlStyles.UserPaint | 
                     ControlStyles.DoubleBuffer | 
                     ControlStyles.ResizeRedraw, true);
            
            typeof(Panel).InvokeMember("DoubleBuffered",
                BindingFlags.SetProperty | BindingFlags.Instance | BindingFlags.NonPublic,
                null, panel1, new object[] { true });
            
            try
            {
                // Try loading from embedded resources first
                var assembly = System.Reflection.Assembly.GetExecutingAssembly();
                var resourceName = "KC_135.kc135.png";
                
                using (Stream stream = assembly.GetManifestResourceStream(resourceName))
                {
                    if (stream != null)
                    {
                        panel1.BackgroundImage = Image.FromStream(stream);
                        panel1.BackgroundImageLayout = ImageLayout.Zoom;
                    }
                    else
                    {
                        // Fallback to file system
                        string imagePath = Path.Combine(Application.StartupPath, "kc135.png");
                        if (!File.Exists(imagePath))
                        {
                            imagePath = Path.Combine(Directory.GetCurrentDirectory(), "kc135.png");
                        }
                        if (!File.Exists(imagePath))
                        {
                            imagePath = @"/mnt/c/Users/izudin/Desktop/Projects/KC-135/KC-135/kc135.png";
                        }
                        
                        if (File.Exists(imagePath))
                        {
                            panel1.BackgroundImage = Image.FromFile(imagePath);
                            panel1.BackgroundImageLayout = ImageLayout.Zoom;
                        }
                        else
                        {
                            MessageBox.Show("Background image 'kc135.png' not found in any expected location.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Could not load background image: {ex.Message}");
            }
            
            triangles = new List<Triangle>
            {
                new Triangle(new PointF(300, 150), 80, 60, 0, Color.Green),
                new Triangle(new PointF(400, 200), 60, 40, 45, Color.Purple),
                new Triangle(new PointF(500, 100), 100, 80, 180, Color.Orange)
            };
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            
            foreach (Triangle triangle in triangles)
            {
                DrawTriangle(g, triangle);
            }
        }

        private void DrawTriangle(Graphics g, Triangle triangle)
        {
            PointF[] points = triangle.GetPoints();
            
            using (Brush brush = new SolidBrush(triangle.Color))
            {
                g.FillPolygon(brush, points);
            }
            
            using (Pen pen = new Pen(triangle.IsSelected ? Color.Red : Color.Black, triangle.IsSelected ? 3 : 1))
            {
                g.DrawPolygon(pen, points);
            }
            
            if (triangle.IsSelected)
            {
                using (Brush centerBrush = new SolidBrush(Color.Red))
                {
                    g.FillEllipse(centerBrush, triangle.Location.X - 3, triangle.Location.Y - 3, 6, 6);
                }
            }
        }

        private void panel1_MouseDown(object sender, MouseEventArgs e)
        {
            PointF mousePoint = new PointF(e.X, e.Y);
            
            if (e.Button == MouseButtons.Left)
            {
                Triangle clickedTriangle = null;
                
                for (int i = triangles.Count - 1; i >= 0; i--)
                {
                    if (triangles[i].ContainsPoint(mousePoint))
                    {
                        clickedTriangle = triangles[i];
                        break;
                    }
                }
                
                if (clickedTriangle != null)
                {
                    if (checkBoxSelection.Checked)
                    {
                        if (selectedTriangle != null)
                            selectedTriangle.IsSelected = false;
                        
                        selectedTriangle = clickedTriangle;
                        selectedTriangle.IsSelected = true;
                        
                        triangles.Remove(clickedTriangle);
                        triangles.Add(clickedTriangle);
                    }
                    else
                    {
                        draggedTriangle = clickedTriangle;
                        isDragging = true;
                        lastMousePosition = mousePoint;
                        dragOffset = new PointF(
                            mousePoint.X - draggedTriangle.Location.X,
                            mousePoint.Y - draggedTriangle.Location.Y
                        );
                        
                        triangles.Remove(clickedTriangle);
                        triangles.Add(clickedTriangle);
                    }
                }
                else if (checkBoxSelection.Checked)
                {
                    if (selectedTriangle != null)
                    {
                        selectedTriangle.IsSelected = false;
                        selectedTriangle = null;
                    }
                }
                
                panel1.Invalidate();
            }
            else if (e.Button == MouseButtons.Right && selectedTriangle != null && checkBoxSelection.Checked)
            {
                isRotating = true;
                lastMousePosition = mousePoint;
            }
        }

        private void panel1_MouseMove(object sender, MouseEventArgs e)
        {
            PointF mousePoint = new PointF(e.X, e.Y);
            
            if (isDragging && draggedTriangle != null)
            {
                draggedTriangle.Location = new PointF(
                    mousePoint.X - dragOffset.X,
                    mousePoint.Y - dragOffset.Y
                );
                
                panel1.Invalidate();
            }
            else if (isRotating && selectedTriangle != null)
            {
                float deltaX = mousePoint.X - selectedTriangle.Location.X;
                float deltaY = mousePoint.Y - selectedTriangle.Location.Y;
                float angle = (float)(Math.Atan2(deltaY, deltaX) * 180.0 / Math.PI);
                
                selectedTriangle.Rotation = angle + 90;
                
                panel1.Invalidate();
            }
        }

        private void panel1_MouseUp(object sender, MouseEventArgs e)
        {
            if (isDragging)
            {
                isDragging = false;
                draggedTriangle = null;
            }
            
            if (isRotating)
            {
                isRotating = false;
            }
        }

        private void checkBoxSelection_CheckedChanged(object sender, EventArgs e)
        {
            if (!checkBoxSelection.Checked)
            {
                if (selectedTriangle != null)
                {
                    selectedTriangle.IsSelected = false;
                    selectedTriangle = null;
                }
                isRotating = false;
                panel1.Invalidate();
            }
        }
    }
}