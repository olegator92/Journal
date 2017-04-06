using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Journal3.ViewModels
{
    public class StatsViewModel
    {
        public DateTime Date { get; set; } 
        public List<JournalViewModel> DateStats { get; set; }
    }
}