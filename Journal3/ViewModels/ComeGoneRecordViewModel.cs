using Journal3.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Journal3.ViewModels
{
    public class ComeGoneRecordViewModel
    {
        public int Id { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateRecord { get; set; }
        public DateTime? DebtWorkDate { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public int Remark { get; set; }
        public string Comment { get; set; }
        public bool WithoutTimebreak { get; set; }


        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
        public WorkSchedule WorkSchedule { get; set; }
        
    }
}