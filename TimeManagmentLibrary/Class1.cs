using System;
using System.Collections.ObjectModel;

namespace TimeManagmentLibrary
{
    public class Module
    {
        //code attribution - GeeksForGeeks
        // Properties of the Module class.
        public string Code { get; set; }
        public string Name { get; set; }
        public int Credits { get; set; }
        public int ClassHours { get; set; }
        public double SelfStudyHours { get; set; }
        public double HoursSpent { get; set; }
        public double RemainingHours { get; set; }

        // Collection to hold recorded hours for each module.
        public ObservableCollection<RecordedHour> RecordedHours { get; set; } = new ObservableCollection<RecordedHour>();
    }

    public class RecordedHour
    {
        public DateTime Date { get; set; }
        public double Hours { get; set; }
    }
}
