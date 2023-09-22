using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using TimeManagmentLibrary;

namespace TimeManagementApp
{

    // Main window class handling the UI interaction.
    public partial class MainWindow : Window
    {
        // Collection to hold the modules.
        public ObservableCollection<Module> Modules { get; set; } = new ObservableCollection<Module>();

        //Code attribution - W3Schools
        // Constructor initializing the main window and setting up initial bindings.
        public MainWindow()
        {
            InitializeComponent();
            lvModules.ItemsSource = Modules;
            cmbModules.ItemsSource = Modules;

            // Initial population of the remaining hours list.
            UpdateRemainingHoursDisplay();
        }

        // Event handler for adding a new module.
        private void AddModule_Click(object sender, RoutedEventArgs e)
        {
            // Validation for numeric inputs
            if (!int.TryParse(txtCredits.Text, out int credits) ||
                !int.TryParse(txtClassHours.Text, out int classHours) ||
                !int.TryParse(txtWeeks.Text, out int weeks))
            {
                MessageBox.Show("Please enter valid numeric values.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Validation for weeks input
            if (weeks <= 0)
            {
                MessageBox.Show("Number of weeks must be greater than zero.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            //Code attribution - W3Schools
            // Creation and initialization of a new Module object.
            var newModule = new Module
            {
                Code = txtModuleCode.Text,
                Name = txtModuleName.Text,
                Credits = credits,
                ClassHours = classHours,
                SelfStudyHours = ((double)credits * 10 / weeks) - classHours
            };

            // Validation for SelfStudyHours
            if (newModule.SelfStudyHours < 0)
            {
                MessageBox.Show("Self-study hours cannot be negative. Please check the input values.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Calculating and initializing RemainingHours for the new Module object.
            newModule.RemainingHours = (newModule.SelfStudyHours + newModule.ClassHours) * weeks;
            Modules.Add(newModule);

            // Refresh the combo box items source.
            cmbModules.ItemsSource = null;
            cmbModules.ItemsSource = Modules;
        }

        // Event handler for recording spent hours.
        private void RecordHours_Click(object sender, RoutedEventArgs e)
        {
            //Code attribution - W3Schools
            // Validation for module selection.
            var selectedModule = cmbModules.SelectedItem as Module;
            if (selectedModule == null)
            {
                MessageBox.Show("Please select a module.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Validation for hours spent input.
            if (!double.TryParse(txtHoursSpent.Text, out double hoursSpent) || hoursSpent < 0)
            {
                MessageBox.Show("Please enter valid and non-negative values for hours spent.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            //Code attribution - W3Schools
            // Updating hours properties for the selected module.
            selectedModule.HoursSpent += hoursSpent;
            selectedModule.SelfStudyHours -= hoursSpent;

            // Validation for negative SelfStudyHours.
            if (selectedModule.SelfStudyHours < 0)
            {
                MessageBox.Show("SelfStudyHours cannot be negative. Please enter a lower value.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Error);
                // Reverting the subtraction as it caused a negative value.
                selectedModule.SelfStudyHours += hoursSpent;
                return;
            }

            selectedModule.RemainingHours -= hoursSpent;

            // Adding a new RecordedHour object to the selected module.
            var recordedDate = dpRecordDate.SelectedDate ?? DateTime.Now;
            selectedModule.RecordedHours.Add(new RecordedHour
            {
                Date = recordedDate,
                Hours = hoursSpent
            });

            // Updating the remaining hours display.
            UpdateRemainingHoursDisplay();
        }

        //Code attribution - W3Schools
        // Method to update the list displaying the remaining hours for the current week.
        private void UpdateRemainingHoursDisplay()
        {
            var remainingHoursList = new ObservableCollection<Module>();
            foreach (var module in Modules)
            {
                var currentWeekNumber = GetIso8601WeekOfYear(DateTime.Now);
                var recordedHoursThisWeek = module.RecordedHours
                    .Where(rh => GetIso8601WeekOfYear(rh.Date) == currentWeekNumber)
                    .Sum(rh => rh.Hours);

                var totalSelfStudyHoursForTheWeek = module.SelfStudyHours;
                var remainingSelfStudyHoursThisWeek = totalSelfStudyHoursForTheWeek - recordedHoursThisWeek;

                if (remainingSelfStudyHoursThisWeek < 0) remainingSelfStudyHoursThisWeek = 0;

                remainingHoursList.Add(new Module { Name = module.Name, RemainingHours = remainingSelfStudyHoursThisWeek });
            }

            lvRemainingHours.ItemsSource = remainingHoursList;

            // Optionally update label for selected module if any.
            var selectedModule = cmbModules.SelectedItem as Module;
            if (selectedModule != null && dpRecordDate.SelectedDate != null)
            {
                string selectedDate = dpRecordDate.SelectedDate.Value.ToString("yyyy-MM-dd");
                lblRemainingHours.Content = $"Module: {selectedModule.Name}, Hours: {selectedModule.HoursSpent}, Date: {selectedDate}";
            }
        }
        //Code attribution - GeeksForGeeks 
        // Method to get the ISO 8601 week number of a given date.
        public static int GetIso8601WeekOfYear(DateTime time)
        {
            DayOfWeek day = CultureInfo.InvariantCulture.Calendar.GetDayOfWeek(time);
            if (day >= DayOfWeek.Monday && day <= DayOfWeek.Wednesday)
                time = time.AddDays(3);

            return CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(time, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
        }
        //Code attribution - GeeksForGeeks 
        // Event handler for Clear button click.
        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            // Clear the input fields.
            txtModuleCode.Text = "";
            txtModuleName.Text = "";
            txtCredits.Text = "";
            txtClassHours.Text = "";
            txtWeeks.Text = "";
            dpStartDate.SelectedDate = null;
            dpRecordDate.SelectedDate = null;
            txtHoursSpent.Text = "";
            cmbModules.SelectedIndex = -1; // Deselect any selected module
        }

        private void cmbModules_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e) { }
        private void txtHoursSpent_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e) { }
        private void txtModuleName_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e) { }
        private void lvRemainingHours_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e) { }
    }
}
