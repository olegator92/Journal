using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Journal3.ViewModels
{
    public class MonthHoursViewModel
    {
        public string Name { get; set; }
        public int LateHours { get; set; }
        public int LateForgivenHours { get; set; }
        public int EarlyGoneHours { get; set; }
        public int EarlyGoneForgivenHours { get; set; }
        public double OutForWorkHours { get; set; }
        public double ByPermissionHours { get; set; }
        public double ByPermissionForgivenHours { get; set; }
        public double DebtWorkHours { get; set; }
        public double OverWorkHours { get; set; }
        public double TotalHours { get; set; }

    }
}