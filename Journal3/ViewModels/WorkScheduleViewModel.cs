using Journal3.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Journal3.ViewModels
{
    public class WorkScheduleViewModel
    {
        public WorkSchedule WorkSchedule { get; set; }
        public List<SpecialSchedule> SpecialSchedules { get; set; }
    }
}