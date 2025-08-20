using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace KC_135
{
    [DesignerCategory("UserControl")]
    public partial class ActivityLED : UserControl
    {
        private System.Windows.Forms.Timer ledTimer;
        private bool ledState = false;
        private bool ledBlinking = false;
        private DateTime lastEventTime = DateTime.MinValue;
        private const int LED_BLINK_INTERVAL = 250; // 250ms = 4Hz toggle rate = 2Hz blink rate
        
        // Properties for customization
        public Color ActiveColor { get; set; } = Color.Red;
        public Color InactiveColor { get; set; } = Color.Gray;
        public int BlinkTimeoutSeconds { get; set; } = 2;
        public string LabelText { get; set; } = string.Empty;

        public ActivityLED()
        {
            InitializeComponent();
            InitializeLED();
        }

        public ActivityLED(Color activeColor, string labelText = "")
        {
            InitializeComponent();
            ActiveColor = activeColor;
            LabelText = labelText;
            InitializeLED();
        }

        private void InitializeLED()
        {
            // Set default appearance - adjust size to accommodate label
            this.Size = string.IsNullOrEmpty(LabelText) ? new Size(20, 20) : new Size(80, 20);
            this.BackColor = Color.Transparent;
            this.BorderStyle = BorderStyle.None;
            
            // Enable double buffering for smooth rendering
            this.SetStyle(ControlStyles.AllPaintingInWmPaint | 
                         ControlStyles.UserPaint | 
                         ControlStyles.DoubleBuffer | 
                         ControlStyles.ResizeRedraw, true);
            
            // Initialize LED timer
            ledTimer = new System.Windows.Forms.Timer();
            ledTimer.Interval = LED_BLINK_INTERVAL;
            ledTimer.Tick += LedTimer_Tick;
        }

        private void LedTimer_Tick(object sender, EventArgs e)
        {
            // Toggle LED state if blinking
            if (ledBlinking)
            {
                ledState = !ledState;
                UpdateLedVisual();
                
                // Check if we should stop blinking
                if (DateTime.Now - lastEventTime > TimeSpan.FromSeconds(BlinkTimeoutSeconds))
                {
                    StopBlinking();
                }
            }
        }

        public void TriggerActivity()
        {
            lastEventTime = DateTime.Now;
            
            // Only start blinking if not already blinking (ensures max 2 Hz)
            if (!ledBlinking)
            {
                StartBlinking();
            }
        }

        private void StartBlinking()
        {
            ledBlinking = true;
            ledTimer.Start();
        }

        private void StopBlinking()
        {
            ledBlinking = false;
            ledState = false;
            ledTimer.Stop();
            UpdateLedVisual();
        }

        private void UpdateLedVisual()
        {
            if (InvokeRequired)
            {
                Invoke(new Action(UpdateLedVisual));
                return;
            }
            
            this.Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            
            Graphics g = e.Graphics;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
            
            // Calculate LED circle bounds
            int ledSize = Math.Min(this.Height - 4, 16); // Max 16px LED size
            int padding = 2;
            Rectangle ledBounds = new Rectangle(padding, 
                                              (this.Height - ledSize) / 2, 
                                              ledSize, ledSize);
            
            // Draw the LED circle
            Color fillColor = ledState ? ActiveColor : InactiveColor;
            using (SolidBrush brush = new SolidBrush(fillColor))
            {
                g.FillEllipse(brush, ledBounds);
            }
            
            // Draw LED border
            using (Pen pen = new Pen(Color.Black, 1))
            {
                g.DrawEllipse(pen, ledBounds);
            }
            
            // Draw label if provided
            if (!string.IsNullOrEmpty(LabelText))
            {
                Rectangle textBounds = new Rectangle(
                    ledBounds.Right + 5, 
                    0, 
                    this.Width - ledBounds.Right - 5, 
                    this.Height);
                
                using (SolidBrush textBrush = new SolidBrush(this.ForeColor))
                {
                    StringFormat format = new StringFormat
                    {
                        Alignment = StringAlignment.Near,
                        LineAlignment = StringAlignment.Center
                    };
                    
                    g.DrawString(LabelText, this.Font, textBrush, textBounds, format);
                }
            }
        }
        
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (ledTimer != null)
                {
                    ledTimer.Stop();
                    ledTimer.Dispose();
                    ledTimer = null;
                }
                if (components != null)
                {
                    components.Dispose();
                }
            }
            base.Dispose(disposing);
        }
    }
}