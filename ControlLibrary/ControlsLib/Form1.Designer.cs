namespace ControlsLib
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.daysOfWeekLabel = new ControlsLib.StateLabel();
            this.daysOfWeekReadOnlyLabel = new ControlsLib.StateLabel();
            this.SuspendLayout();
            // 
            // daysOfWeekLabel
            // 
            this.daysOfWeekLabel.Location = new System.Drawing.Point(20, 20);
            this.daysOfWeekLabel.Name = "daysOfWeekLabel";
            this.daysOfWeekLabel.SelectedIndex = -1;
            this.daysOfWeekLabel.Size = new System.Drawing.Size(0, 0);
            this.daysOfWeekLabel.TabIndex = 0;
            // 
            // daysOfWeekReadOnlyLabel
            // 
            this.daysOfWeekReadOnlyLabel.Location = new System.Drawing.Point(20, 120);
            this.daysOfWeekReadOnlyLabel.Name = "daysOfWeekReadOnlyLabel";
            this.daysOfWeekReadOnlyLabel.SelectedIndex = -1;
            this.daysOfWeekReadOnlyLabel.Size = new System.Drawing.Size(0, 0);
            this.daysOfWeekReadOnlyLabel.TabIndex = 1;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.daysOfWeekLabel);
            this.Controls.Add(this.daysOfWeekReadOnlyLabel);
            this.Name = "Form1";
            this.Text = "StateLabel Demo";
            this.ResumeLayout(false);
        }

        #endregion

        private StateLabel daysOfWeekLabel;
        private StateLabel daysOfWeekReadOnlyLabel;
    }
}
