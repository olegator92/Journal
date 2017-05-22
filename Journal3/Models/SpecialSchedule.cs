using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Journal3.Models
{
    public class SpecialSchedule
    {
        public SpecialSchedule()
        {
        }

        public int Id { get; set; }
        public int DayOfWeek { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public bool WithoutTimeBreak { get; set; }

        public int WorkScheduleId { get; set; }

        public virtual WorkSchedule WorkSchedule { get; set; }

    }
}