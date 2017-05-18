using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Journal3.Models
{
    public class Record
    {
        public int Id { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateRecord { get; set; }
        public DateTime? DebtWorkDate { get; set; } 
        public TimeSpan TimeRecord { get; set; }
        public int Status { get; set; }
        public int Remark { get; set; }
        [StringLength(250, ErrorMessage = "Длина строки должна быть не больше 250 символов")]
        public string Comment { get; set; }
        public bool IsLate{ get; set; }
        public bool IsConfirmed { get; set; }
        public bool IsForgiven { get; set; }
        public bool IsSystem { get; set; }
        public bool WithoutTimebreak { get; set; }

        public string UserId { get; set; }
        public int WorkScheduleId { get; set; }

        public virtual ApplicationUser User { get; set; }
        public virtual WorkSchedule WorkSchedule { get; set; }
    }
}