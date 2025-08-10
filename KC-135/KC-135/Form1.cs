using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;

namespace KC_135
{
    public partial class Form1 : Form
    {
        private List<Sensor> triangles = new List<Sensor>();
        private Dictionary<Sensor, TextBox> consoleTextBoxes = new Dictionary<Sensor, TextBox>();
        private Dictionary<Sensor, Button> consoleCloseButtons = new Dictionary<Sensor, Button>();
        private Dictionary<Sensor, Label> consoleNameLabels = new Dictionary<Sensor, Label>();
        private Dictionary<Sensor, Label> triangleLabels = new Dictionary<Sensor, Label>();
        private System.Windows.Forms.Timer messageUpdateTimer;
        private TestControl testControlForm;

        public Form1()
        {
            InitializeComponent();
            InitializeForm();
        }

        private void InitializeForm()
        {
            //if (DesignMode)
            //return;

            SetStyle(ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.UserPaint |
                     ControlStyles.DoubleBuffer |
                     ControlStyles.ResizeRedraw, true);

            InitializeTimer();

            try
            {
                typeof(Panel).InvokeMember("DoubleBuffered",
                    BindingFlags.SetProperty | BindingFlags.Instance | BindingFlags.NonPublic,
                    null, planePanel, new object[] { true });
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
            triangles = new List<Sensor>
            {
                new Sensor("A", new PointF(200, 170), 60, 0, Color.Green),
                new Sensor("B", new PointF(200, 220), 60, 90, Color.Yellow),
                new Sensor("C", new PointF(200, 220), 60, 270, Color.Yellow),
                new Sensor("Sensor 4", new PointF(200, 240), 50, 180, Color.Orange)
            };

            // Create labels for each triangle
            foreach (Sensor triangle in triangles)
            {
                Label operateLabel = new Label
                {
                    Text = triangle.CurrentMode.ToString(),
                    ForeColor = Color.White,
                    BackColor = Color.Black,
                    Font = new Font("Arial", 8, FontStyle.Bold),
                    AutoSize = true,
                    TextAlign = ContentAlignment.MiddleCenter,
                    BorderStyle = BorderStyle.FixedSingle
                };

                // Position the label in the center of the triangle (halfway along rotation direction)
                float centerDistance = 75; // Fixed distance for label positioning
                double centerRadians = (triangle.Rotation - 90) * Math.PI / 180.0; // Convert to radians, subtract 90 to make 0° point up
                float centerX = triangle.Location.X + (float)(centerDistance * Math.Cos(centerRadians));
                float centerY = triangle.Location.Y + (float)(centerDistance * Math.Sin(centerRadians));
                
                var labelSize = TextRenderer.MeasureText(operateLabel.Text, operateLabel.Font);
                operateLabel.Left = (int)(centerX - labelSize.Width / 2);
                operateLabel.Top = (int)(centerY - labelSize.Height / 2);

                // Add right click event handler to label
                operateLabel.MouseDown += (sender, e) => {
                    if (e.Button == MouseButtons.Right)
                    {
                        if (triangle.IsConsoleVisible)
                        {
                            HideConsole(triangle);
                        }
                        else
                        {
                            ShowConsole(triangle);
                        }
                    }
                };

                planePanel.Controls.Add(operateLabel);
                triangleLabels[triangle] = operateLabel;

                // Bring label to front so it appears above the triangle
                operateLabel.BringToFront();
            }
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
                        planePanel.BackgroundImage = Image.FromStream(stream);
                        planePanel.BackgroundImageLayout = ImageLayout.Zoom;
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
                            planePanel.BackgroundImage = Image.FromFile(imagePath);
                            planePanel.BackgroundImageLayout = ImageLayout.Zoom;
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
                OpenTestControlForm();
            }
        }

        private void OpenTestControlForm()
        {
            if (testControlForm == null || testControlForm.IsDisposed)
            {
                testControlForm = new TestControl();
                testControlForm.SetTriangles(triangles, () => planePanel.Invalidate());
                testControlForm.Location = new Point(this.Location.X + this.Width + 10, this.Location.Y);
                testControlForm.Show();
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
                    foreach (Sensor triangle in triangles)
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
                Sensor triangle = kvp.Key;
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

            // Update labels for all triangles to ensure angle is current
            foreach (Sensor triangle in triangles)
            {
                UpdateTriangleLabel(triangle);
            }

            // Check if any triangles are in declaring mode to trigger redraw for flashing
            bool needsRedraw = triangles.Any(t => t.CurrentMode == SensorMode.Declaring);
            if (needsRedraw)
            {
                planePanel.Invalidate();
            }
        }

        private void UpdateTriangleLabel(Sensor triangle)
        {
            if (triangleLabels.ContainsKey(triangle))
            {
                triangleLabels[triangle].Text = triangle.CurrentMode.ToString();

                // Reposition the label in the center of the triangle (halfway along rotation direction)
                float centerDistance = 75; // Fixed distance for label positioning
                double centerRadians = (triangle.Rotation - 90) * Math.PI / 180.0; // Convert to radians, subtract 90 to make 0° point up
                float centerX = triangle.Location.X + (float)(centerDistance * Math.Cos(centerRadians));
                float centerY = triangle.Location.Y + (float)(centerDistance * Math.Sin(centerRadians));
                
                var labelSize = TextRenderer.MeasureText(triangleLabels[triangle].Text, triangleLabels[triangle].Font);
                triangleLabels[triangle].Left = (int)(centerX - labelSize.Width / 2);
                triangleLabels[triangle].Top = (int)(centerY - labelSize.Height / 2);
            }
        }

        private void ShowConsole(Sensor triangle)
        {
            if (consoleTextBoxes.ContainsKey(triangle))
                return;

            // Close any other open consoles first
            var trianglesToClose = new List<Sensor>(consoleTextBoxes.Keys);
            foreach (Sensor otherTriangle in trianglesToClose)
            {
                if (otherTriangle != triangle)
                {
                    HideConsole(otherTriangle);
                }
            }

            // Calculate sector center point (same as label positioning)
            float centerDistance = 75; // Fixed distance for console positioning
            double centerRadians = (triangle.Rotation - 90) * Math.PI / 180.0; // Convert to radians, subtract 90 to make 0° point up
            float centerX = triangle.Location.X + (float)(centerDistance * Math.Cos(centerRadians));
            float centerY = triangle.Location.Y + (float)(centerDistance * Math.Sin(centerRadians));

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
                Left = (int)(centerX - 100), // Center horizontally (width/2 = 200/2 = 100)
                Top = (int)(centerY - 75)    // Center vertically (height/2 = 150/2 = 75)
            };

            // Create context menu for console
            ContextMenuStrip contextMenu = new ContextMenuStrip();

            ToolStripMenuItem clearItem = new ToolStripMenuItem("Clear");
            clearItem.Click += (sender, e) => consoleTextBox.Clear();

            ToolStripMenuItem copyItem = new ToolStripMenuItem("Copy");
            copyItem.Click += (sender, e) =>
            {
                if (!string.IsNullOrEmpty(consoleTextBox.Text))
                {
                    Clipboard.SetText(consoleTextBox.Text);
                }
            };

            contextMenu.Items.Add(clearItem);
            contextMenu.Items.Add(copyItem);
            consoleTextBox.ContextMenuStrip = contextMenu;

            // Create name label for console (positioned above console window)
            Label nameLabel = new Label
            {
                Text = triangle.Name,
                Width = 100,
                Height = 15,
                Left = (int)(centerX - 100), // Align with left edge of console
                Top = (int)(centerY - 75) - 10, // Position above console window (20 pixels above)
                BackColor = Color.DarkGray,
                ForeColor = Color.Black,
                Font = new Font("Consolas", 8f, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleCenter
            };

            // Create close button
            Button closeButton = new Button
            {
                Text = "×",
                Width = 20,
                Height = 20,
                Left = (int)(centerX - 100) + 180, // Position in upper right corner of console
                Top = (int)(centerY - 75),
                BackColor = Color.DarkRed,
                ForeColor = Color.White,
                Font = new Font("Arial", 10, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                FlatAppearance = { BorderSize = 0 }
            };
            closeButton.Click += (sender, e) => HideConsole(triangle);

            planePanel.Controls.Add(consoleTextBox);
            planePanel.Controls.Add(nameLabel);
            planePanel.Controls.Add(closeButton);
            consoleTextBoxes[triangle] = consoleTextBox;
            consoleNameLabels[triangle] = nameLabel;
            consoleCloseButtons[triangle] = closeButton;
            triangle.IsConsoleVisible = true;
            
            // Bring console, name label, and close button to front so they appear above the operate label
            consoleTextBox.BringToFront();
            nameLabel.BringToFront();
            closeButton.BringToFront();
        }

        private void HideConsole(Sensor triangle)
        {
            if (!consoleTextBoxes.ContainsKey(triangle))
                return;

            TextBox textBox = consoleTextBoxes[triangle];
            planePanel.Controls.Remove(textBox);
            textBox.Dispose();
            consoleTextBoxes.Remove(triangle);

            if (consoleNameLabels.ContainsKey(triangle))
            {
                Label nameLabel = consoleNameLabels[triangle];
                planePanel.Controls.Remove(nameLabel);
                nameLabel.Dispose();
                consoleNameLabels.Remove(triangle);
            }

            if (consoleCloseButtons.ContainsKey(triangle))
            {
                Button closeButton = consoleCloseButtons[triangle];
                planePanel.Controls.Remove(closeButton);
                closeButton.Dispose();
                consoleCloseButtons.Remove(triangle);
            }

            triangle.IsConsoleVisible = false;
        }

        // private void UpdateConsolePosition(Triangle triangle)
        // {
        //     if (consoleTextBoxes.ContainsKey(triangle))
        //     {
        //         TextBox textBox = consoleTextBoxes[triangle];
        //         textBox.Left = (int)(triangle.Location.X + 75 + 5);
        //         textBox.Top = (int)(triangle.Location.Y - triangle.Height * 2/3);
        //     }
        // }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            if (DesignMode || triangles == null)
                return;

            Graphics g = e.Graphics;

            foreach (Sensor triangle in triangles)
            {
                DrawTriangle(g, triangle);
            }
        }

        private void DrawTriangle(Graphics g, Sensor triangle)
        {
            RectangleF cameraLens = triangle.GetCameraLens();
            PointF[] fieldOfView = triangle.GetFieldOfView();

            // Draw camera lens
            using (Brush lensBrush = new SolidBrush(Color.Black))
            {
                g.FillEllipse(lensBrush, cameraLens);
            }

            using (Pen lensPen = new Pen(Color.Black, 1))
            {
                g.DrawEllipse(lensPen, cameraLens);
            }

            using (Brush centerBrush = new SolidBrush(Color.Black))
            {
                float centerSize = cameraLens.Width * 0.3f;
                RectangleF center = new RectangleF(
                    cameraLens.X + (cameraLens.Width - centerSize) / 2,
                    cameraLens.Y + (cameraLens.Height - centerSize) / 2,
                    centerSize,
                    centerSize
                );
                g.FillEllipse(centerBrush, center);
                
            }

            // Draw field of view triangle last (front layer)
            using (Brush fovBrush = new SolidBrush(Color.FromArgb(180, triangle.GetSensorModeColor())))
            {
                g.FillPolygon(fovBrush, fieldOfView);
            }

            using (Pen fovPen = new Pen(Color.Black, 1))
            {
                g.DrawPolygon(fovPen, fieldOfView);
            }

            using (Brush lensBrush = new SolidBrush(Color.Black))
            {
                g.FillEllipse(lensBrush, cameraLens);
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

            if (e.Button == MouseButtons.Right)
            {
                Sensor clickedTriangle = null;

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

                    // draggedTriangle = clickedTriangle;
                    // isDragging = true;
                    // lastMousePosition = mousePoint;
                    // dragOffset = new PointF(
                    //     mousePoint.X - draggedTriangle.Location.X,
                    //     mousePoint.Y - draggedTriangle.Location.Y
                    // );

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

                planePanel.Invalidate();
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

            // if (isDragging && draggedTriangle != null)
            // {
            //     draggedTriangle.Location = new PointF(
            //         mousePoint.X - dragOffset.X,
            //         mousePoint.Y - dragOffset.Y
            //     );
            //     
            //     UpdateConsolePosition(draggedTriangle);
            //     panel1.Invalidate();
            // }
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

            // if (isDragging)
            // {
            //     isDragging = false;
            //     draggedTriangle = null;
            // }

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

            if (consoleNameLabels != null)
            {
                foreach (var label in consoleNameLabels.Values)
                {
                    label.Dispose();
                }
                consoleNameLabels.Clear();
            }

            if (consoleCloseButtons != null)
            {
                foreach (var button in consoleCloseButtons.Values)
                {
                    button.Dispose();
                }
                consoleCloseButtons.Clear();
            }

            if (triangleLabels != null)
            {
                foreach (var label in triangleLabels.Values)
                {
                    label.Dispose();
                }
                triangleLabels.Clear();
            }

            if (testControlForm != null && !testControlForm.IsDisposed)
            {
                testControlForm.Close();
                testControlForm.Dispose();
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}