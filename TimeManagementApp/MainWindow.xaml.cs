using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace TimeManagementApp
{
    public class Module
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public int Credits { get; set; }
        public int ClassHours { get; set; }
        public double SelfStudyHours { get; set; }
        public double HoursSpent { get; set; }
        public double RemainingHours { get; set; }
    }

    public partial class MainWindow : Window
    {
        public ObservableCollection<Module> Modules { get; set; } = new ObservableCollection<Module>();

        public MainWindow()
        {
            InitializeComponent();
            lvModules.ItemsSource = Modules;
            cmbModules.ItemsSource = Modules;
        }

        private void AddModule_Click(object sender, RoutedEventArgs e)
        {
            // Existing code remains the same...
            // Calculating remaining hours initially.
            var newModule = new Module
            {
                // ... (the rest of your properties)
                RemainingHours = (newModule.SelfStudyHours + newModule.ClassHours) * int.Parse(txtWeeks.Text) // Initial Remaining hours considering weeks.
            };
            Modules.Add(newModule);
            cmbModules.ItemsSource = null;
            cmbModules.ItemsSource = Modules; // Refresh the ComboBox items
        }

        private void RecordHours_Click(object sender, RoutedEventArgs e)
        {
            var selectedModule = cmbModules.SelectedItem as Module;
            if (selectedModule == null)
            {
                MessageBox.Show("Please select a module.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (!double.TryParse(txtHoursSpent.Text, out double hoursSpent) || hoursSpent < 0)
            {
                MessageBox.Show("Please enter valid and non-negative values for hours spent.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            selectedModule.HoursSpent += hoursSpent;
            selectedModule.RemainingHours -= hoursSpent; // Subtract the recorded hours from remaining hours.

            UpdateRemainingHoursDisplay();
        }

        private void UpdateRemainingHoursDisplay()
        {
            double totalRemainingHours = Modules.Sum(m => m.RemainingHours);
            lblRemainingHours.Content = $"Total Remaining Hours: {totalRemainingHours}";
        }
    }
}
