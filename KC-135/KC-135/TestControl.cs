using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace KC_135
{
    public partial class TestControl : Form
    {
        private List<Triangle> triangles;
        private List<GroupBox> sensorGroups;
        private Action onSensorModeChanged;

        public TestControl()
        {
            InitializeComponent();
        }

        public void SetTriangles(List<Triangle> triangleList, Action onModeChanged = null)
        {
            triangles = triangleList;
            onSensorModeChanged = onModeChanged;
            CreateSensorControls();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            
            // TestControl
            this.AutoScaleDimensions = new SizeF(6F, 13F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.ClientSize = new Size(500, 400);
            this.Name = "TestControl";
            this.Text = "Sensor Control";
            this.StartPosition = FormStartPosition.Manual;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            
            this.ResumeLayout(false);
        }

        private void CreateSensorControls()
        {
            if (triangles == null) return;

            sensorGroups = new List<GroupBox>();
            int yPosition = 10;

            for (int i = 0; i < triangles.Count; i++)
            {
                Triangle triangle = triangles[i];
                
                // Create group box for this sensor
                GroupBox sensorGroup = new GroupBox
                {
                    Text = $"Sensor {i + 1}",
                    Location = new Point(10, yPosition),
                    Size = new Size(460, 80),
                    Font = new Font("Arial", 9, FontStyle.Bold)
                };

                // Create radio buttons for each sensor mode
                RadioButton offRadio = new RadioButton
                {
                    Text = "Off",
                    Location = new Point(10, 20),
                    Size = new Size(80, 20),
                    Checked = triangle.CurrentMode == SensorMode.Off
                };
                offRadio.CheckedChanged += (sender, e) => {
                    if (offRadio.Checked) {
                        triangle.CurrentMode = SensorMode.Off;
                        onSensorModeChanged?.Invoke();
                    }
                };

                RadioButton initRadio = new RadioButton
                {
                    Text = "Initializing",
                    Location = new Point(100, 20),
                    Size = new Size(100, 20),
                    Checked = triangle.CurrentMode == SensorMode.Initializing
                };
                initRadio.CheckedChanged += (sender, e) => {
                    if (initRadio.Checked) {
                        triangle.CurrentMode = SensorMode.Initializing;
                        onSensorModeChanged?.Invoke();
                    }
                };

                RadioButton operateRadio = new RadioButton
                {
                    Text = "Operate",
                    Location = new Point(210, 20),
                    Size = new Size(80, 20),
                    Checked = triangle.CurrentMode == SensorMode.Operate
                };
                operateRadio.CheckedChanged += (sender, e) => {
                    if (operateRadio.Checked) {
                        triangle.CurrentMode = SensorMode.Operate;
                        onSensorModeChanged?.Invoke();
                    }
                };

                RadioButton degradedRadio = new RadioButton
                {
                    Text = "Degraded",
                    Location = new Point(300, 20),
                    Size = new Size(90, 20),
                    Checked = triangle.CurrentMode == SensorMode.Degraded
                };
                degradedRadio.CheckedChanged += (sender, e) => {
                    if (degradedRadio.Checked) {
                        triangle.CurrentMode = SensorMode.Degraded;
                        onSensorModeChanged?.Invoke();
                    }
                };

                // Add radio buttons to group box
                sensorGroup.Controls.Add(offRadio);
                sensorGroup.Controls.Add(initRadio);
                sensorGroup.Controls.Add(operateRadio);
                sensorGroup.Controls.Add(degradedRadio);

                // Add group box to form
                this.Controls.Add(sensorGroup);
                sensorGroups.Add(sensorGroup);

                yPosition += 90;
            }
        }
    }
}