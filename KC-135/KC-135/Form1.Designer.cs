using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace KC_135
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.Panel planePanel;
        private System.Windows.Forms.Panel detailsPanel;
        // private System.Windows.Forms.CheckBox checkBoxSelection;
        private ActivityLED activityLED;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                CleanupResources();
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            planePanel = new Panel();
            detailsPanel = new Panel();
            activityLED = new ActivityLED();
            SuspendLayout();
            // 
            // planePanel
            // 
            planePanel.Location = new Point(12, 40);
            planePanel.Name = "planePanel";
            planePanel.Size = new Size(400, 400);
            planePanel.TabIndex = 0;
            planePanel.Paint += panel1_Paint;
            planePanel.MouseDown += panel1_MouseDown;
            planePanel.MouseMove += panel1_MouseMove;
            planePanel.MouseUp += panel1_MouseUp;
            // 
            // detailsPanel
            // 
            detailsPanel.BackColor = Color.LightGray;
            detailsPanel.BorderStyle = BorderStyle.FixedSingle;
            detailsPanel.Location = new Point(430, 40);
            detailsPanel.Name = "detailsPanel";
            detailsPanel.Size = new Size(250, 400);
            detailsPanel.TabIndex = 1;
            detailsPanel.Visible = false;
            // 
            // activityLED
            // 
            activityLED.ActiveColor = Color.Red;
            activityLED.InactiveColor = Color.Gray;
            activityLED.BlinkTimeoutSeconds = 2;
            activityLED.Location = new Point(12, 12);
            activityLED.Name = "activityLED";
            activityLED.Size = new Size(20, 20);
            activityLED.TabIndex = 2;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(700, 461);
            Controls.Add(activityLED);
            Controls.Add(detailsPanel);
            Controls.Add(planePanel);
            Name = "Form1";
            Text = "KC-135 Sensor Simulator";
            Load += Form1_Load;
            ResumeLayout(false);
        }
    }
}