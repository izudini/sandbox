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
            this.ClientSize = new Size(320, 200);
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
            int yPosition = 3;

            for (int i = 0; i < triangles.Count; i++)
            {
                Triangle triangle = triangles[i];
                
                // Create group box for this sensor
                GroupBox sensorGroup = new GroupBox
                {
                    Text = $"S{i + 1}",
                    Location = new Point(3, yPosition),
                    Size = new Size(314, 42),
                    Font = new Font("Arial", 7, FontStyle.Bold)
                };

                // Create radio buttons for each sensor mode
                RadioButton offRadio = new RadioButton
                {
                    Text = "Off",
                    Location = new Point(3, 12),
                    Size = new Size(35, 15),
                    Checked = triangle.CurrentMode == SensorMode.Off,
                    Font = new Font("Arial", 6.5f)
                };
                offRadio.CheckedChanged += (sender, e) => {
                    if (offRadio.Checked) {
                        triangle.CurrentMode = SensorMode.Off;
                        onSensorModeChanged?.Invoke();
                    }
                };

                RadioButton initRadio = new RadioButton
                {
                    Text = "Init",
                    Location = new Point(40, 12),
                    Size = new Size(38, 15),
                    Checked = triangle.CurrentMode == SensorMode.Initializing,
                    Font = new Font("Arial", 6.5f)
                };
                initRadio.CheckedChanged += (sender, e) => {
                    if (initRadio.Checked) {
                        triangle.CurrentMode = SensorMode.Initializing;
                        onSensorModeChanged?.Invoke();
                    }
                };

                RadioButton operateRadio = new RadioButton
                {
                    Text = "Op",
                    Location = new Point(80, 12),
                    Size = new Size(30, 15),
                    Checked = triangle.CurrentMode == SensorMode.Operate,
                    Font = new Font("Arial", 6.5f)
                };
                operateRadio.CheckedChanged += (sender, e) => {
                    if (operateRadio.Checked) {
                        triangle.CurrentMode = SensorMode.Operate;
                        onSensorModeChanged?.Invoke();
                    }
                };

                RadioButton degradedRadio = new RadioButton
                {
                    Text = "Deg",
                    Location = new Point(112, 12),
                    Size = new Size(35, 15),
                    Checked = triangle.CurrentMode == SensorMode.Degraded,
                    Font = new Font("Arial", 6.5f)
                };
                degradedRadio.CheckedChanged += (sender, e) => {
                    if (degradedRadio.Checked) {
                        triangle.CurrentMode = SensorMode.Degraded;
                        onSensorModeChanged?.Invoke();
                    }
                };

                RadioButton declaringRadio = new RadioButton
                {
                    Text = "Decl",
                    Location = new Point(149, 12),
                    Size = new Size(40, 15),
                    Checked = triangle.CurrentMode == SensorMode.Declaring,
                    Font = new Font("Arial", 6.5f)
                };
                declaringRadio.CheckedChanged += (sender, e) => {
                    if (declaringRadio.Checked) {
                        triangle.CurrentMode = SensorMode.Declaring;
                        onSensorModeChanged?.Invoke();
                    }
                };

                // Add radio buttons to group box
                sensorGroup.Controls.Add(offRadio);
                sensorGroup.Controls.Add(initRadio);
                sensorGroup.Controls.Add(operateRadio);
                sensorGroup.Controls.Add(degradedRadio);
                sensorGroup.Controls.Add(declaringRadio);

                // Add group box to form
                this.Controls.Add(sensorGroup);
                sensorGroups.Add(sensorGroup);

                yPosition += 45;
            }
        }
    }
}