using Journal3.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Journal3.ViewModels
{
    public class JournalViewModel
    {
        public ApplicationUser User { get; set; }
        public WorkSchedule WorkSchedule { get; set; }
        public ComeViewModel Come { get; set; }
        public GoneViewModel Gone { get; set; }
        public bool WithoutTimebreak { get; set; }
        public bool IsSystem { get; set; }
        public TimeSpan OutForWorkTime { get; set; }
        public TimeSpan ByPermissionTime { get; set; }
        public TimeSpan ByPermissionForgivenTime { get; set; }
        public TimeSpan PlusDebtWorkTime { get; set; }
        public TimeSpan MinusDebtWorkTime { get; set; }
        public TimeSpan OverWorkTime { get; set; }
        public TimeSpan TotalTime { get; set; }
    }

    public class ComeViewModel
    {
        public TimeSpan Time { get; set; }
        public bool IsForgiven { get; set; }
        public bool IsProblem { get; set; }
        public bool IsLate { get; set; }
        public string Comment { get; set; }
    }

    public class GoneViewModel
    {
        public TimeSpan Time { get; set; }
        public bool IsForgiven { get; set; }
        public bool IsProblem { get; set; }
        public bool IsEarlyGone { get; set; }
        public string Comment { get; set; }
    }

}