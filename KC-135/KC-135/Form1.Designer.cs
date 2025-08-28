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
        private System.Windows.Forms.Button startButton;
        private System.Windows.Forms.Button stopButton;
        private System.Windows.Forms.Label statusLabel;

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
            startButton = new Button();
            stopButton = new Button();
            statusLabel = new Label();
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
            // startButton
            // 
            startButton.BackColor = Color.Green;
            startButton.ForeColor = Color.White;
            startButton.Font = new Font("Arial", 9, FontStyle.Bold);
            startButton.Location = new Point(50, 12);
            startButton.Name = "startButton";
            startButton.Size = new Size(75, 25);
            startButton.TabIndex = 3;
            startButton.Text = "Start";
            startButton.UseVisualStyleBackColor = false;
            startButton.Click += StartButton_Click;
            // 
            // stopButton
            // 
            stopButton.BackColor = Color.Red;
            stopButton.ForeColor = Color.White;
            stopButton.Font = new Font("Arial", 9, FontStyle.Bold);
            stopButton.Location = new Point(135, 12);
            stopButton.Name = "stopButton";
            stopButton.Size = new Size(75, 25);
            stopButton.TabIndex = 4;
            stopButton.Text = "Stop";
            stopButton.UseVisualStyleBackColor = false;
            stopButton.Click += StopButton_Click;
            // 
            // statusLabel
            // 
            statusLabel.AutoSize = true;
            statusLabel.Font = new Font("Arial", 9, FontStyle.Bold);
            statusLabel.Location = new Point(220, 17);
            statusLabel.Name = "statusLabel";
            statusLabel.Size = new Size(52, 15);
            statusLabel.TabIndex = 5;
            statusLabel.Text = "Stopped";
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(700, 461);
            Controls.Add(statusLabel);
            Controls.Add(stopButton);
            Controls.Add(startButton);
            Controls.Add(activityLED);
            Controls.Add(detailsPanel);
            Controls.Add(planePanel);
            Name = "Form1";
            Text = "KC-135 Sensor Simulator";
            Load += Form1_Load;
            ResumeLayout(false);
            PerformLayout();
        }
    }
}