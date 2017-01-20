using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Journal3.Models
{
    public class Record
    {
        public int Id { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateRecord { get; set; }
        public int Status { get; set; }
        public int Remark { get; set; }
        public string Comment { get; set; }
        public bool IsConfirmed { get; set; }
        public bool IsForgiven { get; set; }
        public bool IsSystem { get; set; }

        public virtual ApplicationUser User { get; set; }
    }
}