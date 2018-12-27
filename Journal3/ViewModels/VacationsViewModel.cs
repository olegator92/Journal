using Journal3.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Journal3.ViewModels
{
    public class VacationsViewModel
    {
        public List<DateTime> Dates { get; set; }
        public ApplicationUser User { get; set; }
    }
}