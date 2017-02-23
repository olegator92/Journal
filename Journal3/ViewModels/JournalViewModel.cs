using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Journal3.ViewModels
{
    public class JournalViewModel
    {
        public string EmployeeName { get; set; }
        public TimeSpan ComeTime { get; set; }
        public TimeSpan GoneTime { get; set; }
    }
}