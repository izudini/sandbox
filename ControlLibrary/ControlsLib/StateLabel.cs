using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace ControlsLib
{
    public partial class StateLabel : UserControl
    {
        private List<Label> _labels = new List<Label>();
        private int _selectedIndex = -1;
        private string[] _states = Array.Empty<string>();
        private FlowLayoutPanel _flowPanel;
        private bool _readOnly = false;
        private GroupBox _groupBox;
        private string _frameName;

        public StateLabel()
        {
            InitializeComponent();
            InitializeFlowPanel();
        }

        public StateLabel(string frameName)
        {
            InitializeComponent();
            _frameName = frameName;
            InitializeWithFrame();
        }

        private void InitializeFlowPanel()
        {
            _flowPanel = new FlowLayoutPanel
            {
                FlowDirection = FlowDirection.LeftToRight,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                WrapContents = false,
                Margin = Padding.Empty,
                Padding = new Padding(5, 5, 5, 5)
            };

            if (_groupBox != null)
            {
                _flowPanel.Location = new Point(5, 25);
                _flowPanel.Dock = DockStyle.None;
                _groupBox.Controls.Add(_flowPanel);
            }
            else
            {
                this.Controls.Add(_flowPanel);
            }

            this.AutoSize = true;
            this.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        }

        private void InitializeWithFrame()
        {
            _groupBox = new GroupBox
            {
                Text = _frameName,
                Size = new Size(400, 80),
                Location = new Point(0, 0)
            };

            _flowPanel = new FlowLayoutPanel
            {
                FlowDirection = FlowDirection.LeftToRight,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                WrapContents = false,
                Location = new Point(10, 30),
                Size = new Size(380, 40)
            };

            _groupBox.Controls.Add(_flowPanel);
            this.Controls.Add(_groupBox);
            this.AutoSize = true;
            this.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        }

        public StateLabel(IEnumerable<string> states) : this()
        {
            States = states?.ToArray() ?? Array.Empty<string>();
        }

        public StateLabel(IEnumerable<string> states, bool readOnly) : this()
        {
            States = states?.ToArray() ?? Array.Empty<string>();
            ReadOnly = readOnly;
        }

        public StateLabel(IEnumerable<string> states, string frameName) : this(frameName)
        {
            States = states?.ToArray() ?? Array.Empty<string>();
        }

        public StateLabel(IEnumerable<string> states, string frameName, bool readOnly) : this(frameName)
        {
            States = states?.ToArray() ?? Array.Empty<string>();
            ReadOnly = readOnly;
        }

        [DefaultValue(null)]
        public string[] States
        {
            get { return _states; }
            set
            {
                _states = value ?? Array.Empty<string>();
                CreateLabels();
            }
        }

        [DefaultValue(0)]
        public int SelectedIndex
        {
            get { return _selectedIndex; }
            set
            {
                if (value >= 0 && value < _labels.Count)
                {
                    _selectedIndex = value;
                    UpdateSelection();
                }
                else if (value == -1)
                {
                    _selectedIndex = -1;
                    UpdateSelection();
                }
            }
        }

        [DefaultValue(null)]
        public string SelectedState
        {
            get
            {
                return _selectedIndex >= 0 && _selectedIndex < _states.Length
                    ? _states[_selectedIndex]
                    : string.Empty;
            }
            set
            {
                var index = Array.IndexOf(_states, value);
                SelectedIndex = index;
            }
        }

        [DefaultValue(false)]
        public bool ReadOnly
        {
            get { return _readOnly; }
            set
            {
                _readOnly = value;
                UpdateLabelsInteraction();
            }
        }

        public event EventHandler<int> StateSelected;

        private void CreateLabels()
        {
            _flowPanel.Controls.Clear();
            _labels.Clear();

            if (_states == null || _states.Length == 0)
                return;

            for (int i = 0; i < _states.Length; i++)
            {
                var label = new Label
                {
                    Text = _states[i],
                    AutoSize = true,
                    Padding = new Padding(8, 4, 8, 4),
                    BorderStyle = BorderStyle.FixedSingle,
                    BackColor = Color.LightGray,
                    Cursor = _readOnly ? Cursors.Default : Cursors.Hand,
                    Tag = i,
                    Margin = new Padding(0, 0, 2, 0)
                };

                if (!_readOnly)
                {
                    label.Click += Label_Click;
                }
                _labels.Add(label);
                _flowPanel.Controls.Add(label);
            }

            UpdateSelection();
        }

        private void Label_Click(object sender, EventArgs e)
        {
            if (_readOnly) return;
            
            if (sender is Label label && label.Tag is int index)
            {
                SelectedIndex = index;
                StateSelected?.Invoke(this, index);
            }
        }

        private void UpdateLabelsInteraction()
        {
            foreach (var label in _labels)
            {
                label.Cursor = _readOnly ? Cursors.Default : Cursors.Hand;
                
                if (_readOnly)
                {
                    label.Click -= Label_Click;
                }
                else
                {
                    label.Click -= Label_Click;
                    label.Click += Label_Click;
                }
            }
        }

        private void UpdateSelection()
        {
            for (int i = 0; i < _labels.Count; i++)
            {
                _labels[i].BackColor = i == _selectedIndex ? Color.LightGreen : Color.LightGray;
            }
        }
    }
}