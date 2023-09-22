using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeManagementAppLibrary
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
}