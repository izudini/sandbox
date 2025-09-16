namespace ControlsLib
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            InitializeDaysOfWeek();
        }

        private void InitializeDaysOfWeek()
        {
            string[] daysOfWeek = { "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday" };
            
            // Interactive control with no frame
            daysOfWeekLabel.States = daysOfWeek;
            daysOfWeekLabel.SelectedIndex = 0;
            daysOfWeekLabel.StateSelected += DaysOfWeekLabel_StateSelected;
            
            // Replace the designer-created control with a framed readonly control
            this.Controls.Remove(daysOfWeekReadOnlyLabel);
            daysOfWeekReadOnlyLabel = new StateLabel(daysOfWeek, "ReadOnly", true);
            daysOfWeekReadOnlyLabel.Location = new System.Drawing.Point(20, 120);
            daysOfWeekReadOnlyLabel.SelectedIndex = 2; // Select Wednesday
            this.Controls.Add(daysOfWeekReadOnlyLabel);
        }

        private void DaysOfWeekLabel_StateSelected(object sender, int selectedIndex)
        {
            string selectedDay = daysOfWeekLabel.SelectedState;
            this.Text = $"StateLabel Demo - Selected: {selectedDay}";
        }
    }
}
