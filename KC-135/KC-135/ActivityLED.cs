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

        public ActivityLED()
        {
            InitializeComponent();
            InitializeLED();
        }

        private void InitializeLED()
        {
            // Set default appearance
            this.Size = new Size(20, 20);
            this.BackColor = InactiveColor;
            this.BorderStyle = BorderStyle.FixedSingle;
            
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
            
            this.BackColor = ledState ? ActiveColor : InactiveColor;
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