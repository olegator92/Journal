using Journal3.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Journal3.ViewModels
{
    public class RecordViewModel
    {
        public int Id { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateRecord { get; set; }
        public DateTime? DebtWorkDate { get; set; }
        public TimeSpan TimeRecord { get; set; }
        public int Status { get; set; }
        public string StatusName { get; set; }
        public int Remark { get; set; }
        public string RemarkName { get; set; }
        public string Comment { get; set; }
        public bool IsLate { get; set; }
        public bool IsConfirmed { get; set; }
        public bool IsForgiven { get; set; }
        public bool IsSystem { get; set; }

        public  ApplicationUser User { get; set; }
        public  WorkSchedule WorkSchedule { get; set; }
    }
}