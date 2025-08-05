namespace KC_135
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.Panel planePanel;
        // private System.Windows.Forms.CheckBox checkBoxSelection;

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
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 638);
            Controls.Add(planePanel);
            Name = "Form1";
            Text = "Form1";
            ResumeLayout(false);
        }
    }
}