using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;

namespace KC_135
{
    public partial class Form1 : Form
    {
        private List<Triangle> triangles = new List<Triangle>();
        private Triangle draggedTriangle;
        // private Triangle selectedTriangle;
        private bool isDragging;
        // private bool isRotating;
        private PointF lastMousePosition;
        private PointF dragOffset;
        private Dictionary<Triangle, TextBox> consoleTextBoxes = new Dictionary<Triangle, TextBox>();
        private System.Windows.Forms.Timer messageUpdateTimer;

        public Form1()
        {
            InitializeComponent();
            InitializeForm();
        }
        
        private void InitializeForm()
        {
            if (DesignMode)
                return;
                
            SetStyle(ControlStyles.AllPaintingInWmPaint | 
                     ControlStyles.UserPaint | 
                     ControlStyles.DoubleBuffer | 
                     ControlStyles.ResizeRedraw, true);
            
            InitializeTimer();
            
            try
            {
                typeof(Panel).InvokeMember("DoubleBuffered",
                    BindingFlags.SetProperty | BindingFlags.Instance | BindingFlags.NonPublic,
                    null, panel1, new object[] { true });
            }
            catch
            {
                // Ignore reflection errors in designer
            }
            
            InitializeTriangles();
            InitializeBackgroundImage();
        }
        
        private void InitializeTimer()
        {
            messageUpdateTimer = new System.Windows.Forms.Timer();
            messageUpdateTimer.Interval = 100;
            messageUpdateTimer.Tick += MessageUpdateTimer_Tick;
            messageUpdateTimer.Start();
        }
        
        private void InitializeTriangles()
        {
            triangles = new List<Triangle>
            {
                new Triangle(new PointF(300, 150), 80, 60, 0, Color.Green),
                new Triangle(new PointF(400, 200), 60, 40, 45, Color.Purple),
                new Triangle(new PointF(500, 100), 100, 80, 180, Color.Orange)
            };
        }
        
        private void InitializeBackgroundImage()
        {
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
        }
        
        protected override void SetVisibleCore(bool value)
        {
            base.SetVisibleCore(value);
            if (value && !DesignMode && messageUpdateTimer != null && triangles != null)
            {
                StartMessageGenerator();
            }
        }
        
        private void StartMessageGenerator()
        {
            Thread messageThread = new Thread(() =>
            {
                Random random = new Random();
                string[] sampleMessages = {
                    "System status: OK",
                    "Temperature: 72°F",
                    "Fuel level: 85%",
                    "Altitude: 35000 ft",
                    "Speed: 450 mph",
                    "Navigation online",
                    "Radar clear",
                    "Communication active"
                };
                
                while (true)
                {
                    foreach (Triangle triangle in triangles)
                    {
                        if (random.Next(0, 10) < 3)
                        {
                            string message = $"[{DateTime.Now:HH:mm:ss}] {sampleMessages[random.Next(sampleMessages.Length)]}";
                            triangle.MessageQueue.Enqueue(message);
                        }
                    }
                    Thread.Sleep(2000);
                }
            })
            {
                IsBackground = true
            };
            messageThread.Start();
        }
        
        private void MessageUpdateTimer_Tick(object sender, EventArgs e)
        {
            foreach (var kvp in consoleTextBoxes)
            {
                Triangle triangle = kvp.Key;
                TextBox textBox = kvp.Value;
                
                List<string> messages = new List<string>();
                while (triangle.MessageQueue.TryDequeue(out string message) && messages.Count < 50)
                {
                    messages.Add(message);
                }
                
                if (messages.Count > 0)
                {
                    textBox.AppendText(string.Join(Environment.NewLine, messages) + Environment.NewLine);
                    textBox.SelectionStart = textBox.Text.Length;
                    textBox.ScrollToCaret();
                }
            }
        }
        
        private void ShowConsole(Triangle triangle)
        {
            if (consoleTextBoxes.ContainsKey(triangle))
                return;
                
            TextBox consoleTextBox = new TextBox
            {
                Multiline = true,
                ReadOnly = true,
                ScrollBars = ScrollBars.None,
                BackColor = Color.Black,
                ForeColor = Color.Lime,
                Font = new Font("Consolas", 6.4f),
                Width = 200,
                Height = 150,
                Left = (int)(triangle.Location.X + triangle.Base/2 + 5),
                Top = (int)(triangle.Location.Y - triangle.Height * 2/3)
            };
            
            panel1.Controls.Add(consoleTextBox);
            consoleTextBoxes[triangle] = consoleTextBox;
            triangle.IsConsoleVisible = true;
        }
        
        private void HideConsole(Triangle triangle)
        {
            if (!consoleTextBoxes.ContainsKey(triangle))
                return;
                
            TextBox textBox = consoleTextBoxes[triangle];
            panel1.Controls.Remove(textBox);
            textBox.Dispose();
            consoleTextBoxes.Remove(triangle);
            triangle.IsConsoleVisible = false;
        }
        
        private void UpdateConsolePosition(Triangle triangle)
        {
            if (consoleTextBoxes.ContainsKey(triangle))
            {
                TextBox textBox = consoleTextBoxes[triangle];
                textBox.Left = (int)(triangle.Location.X + triangle.Base/2 + 5);
                textBox.Top = (int)(triangle.Location.Y - triangle.Height * 2/3);
            }
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            if (DesignMode || triangles == null)
                return;
                
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
            
            using (Pen pen = new Pen(Color.Black, 1))
            {
                g.DrawPolygon(pen, points);
            }
            
            // if (triangle.IsSelected)
            // {
            //     using (Brush centerBrush = new SolidBrush(Color.Red))
            //     {
            //         g.FillEllipse(centerBrush, triangle.Location.X - 3, triangle.Location.Y - 3, 6, 6);
            //     }
            // }
        }

        private void panel1_MouseDown(object sender, MouseEventArgs e)
        {
            if (DesignMode || triangles == null)
                return;
                
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
                    // if (checkBoxSelection.Checked)
                    // {
                    //     if (selectedTriangle != null)
                    //         selectedTriangle.IsSelected = false;
                    //     
                    //     selectedTriangle = clickedTriangle;
                    //     selectedTriangle.IsSelected = true;
                    //     
                    //     triangles.Remove(clickedTriangle);
                    //     triangles.Add(clickedTriangle);
                    // }
                    // else
                    // {
                        if (clickedTriangle.IsConsoleVisible)
                        {
                            HideConsole(clickedTriangle);
                        }
                        else
                        {
                            ShowConsole(clickedTriangle);
                        }
                        
                        draggedTriangle = clickedTriangle;
                        isDragging = true;
                        lastMousePosition = mousePoint;
                        dragOffset = new PointF(
                            mousePoint.X - draggedTriangle.Location.X,
                            mousePoint.Y - draggedTriangle.Location.Y
                        );
                        
                        triangles.Remove(clickedTriangle);
                        triangles.Add(clickedTriangle);
                    // }
                }
                // else if (checkBoxSelection.Checked)
                // {
                //     if (selectedTriangle != null)
                //     {
                //         selectedTriangle.IsSelected = false;
                //         selectedTriangle = null;
                //     }
                // }
                
                panel1.Invalidate();
            }
            // else if (e.Button == MouseButtons.Right && selectedTriangle != null && checkBoxSelection.Checked)
            // {
            //     isRotating = true;
            //     lastMousePosition = mousePoint;
            // }
        }

        private void panel1_MouseMove(object sender, MouseEventArgs e)
        {
            if (DesignMode)
                return;
                
            PointF mousePoint = new PointF(e.X, e.Y);
            
            if (isDragging && draggedTriangle != null)
            {
                draggedTriangle.Location = new PointF(
                    mousePoint.X - dragOffset.X,
                    mousePoint.Y - dragOffset.Y
                );
                
                UpdateConsolePosition(draggedTriangle);
                panel1.Invalidate();
            }
            // else if (isRotating && selectedTriangle != null)
            // {
            //     float deltaX = mousePoint.X - selectedTriangle.Location.X;
            //     float deltaY = mousePoint.Y - selectedTriangle.Location.Y;
            //     float angle = (float)(Math.Atan2(deltaY, deltaX) * 180.0 / Math.PI);
            //     
            //     selectedTriangle.Rotation = angle + 90;
            //     
            //     panel1.Invalidate();
            // }
        }

        private void panel1_MouseUp(object sender, MouseEventArgs e)
        {
            if (DesignMode)
                return;
                
            if (isDragging)
            {
                isDragging = false;
                draggedTriangle = null;
            }
            
            // if (isRotating)
            // {
            //     isRotating = false;
            // }
        }

        // private void checkBoxSelection_CheckedChanged(object sender, EventArgs e)
        // {
        //     if (!checkBoxSelection.Checked)
        //     {
        //         if (selectedTriangle != null)
        //         {
        //             selectedTriangle.IsSelected = false;
        //             selectedTriangle = null;
        //         }
        //         isRotating = false;
        //         panel1.Invalidate();
        //     }
        // }
        
        private void CleanupResources()
        {
            if (messageUpdateTimer != null)
            {
                messageUpdateTimer.Stop();
                messageUpdateTimer.Dispose();
                messageUpdateTimer = null;
            }
            
            if (consoleTextBoxes != null)
            {
                foreach (var textBox in consoleTextBoxes.Values)
                {
                    textBox.Dispose();
                }
                consoleTextBoxes.Clear();
            }
        }
    }
}