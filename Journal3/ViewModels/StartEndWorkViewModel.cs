using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Journal3.ViewModels
{
    public class StartEndWorkViewModel
    {
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public bool WithoutTimeBreak { get; set; }
        public bool IsWorkDay { get; set; }
    }
}