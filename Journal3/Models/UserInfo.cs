using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Journal3.Models
{
    public class UserInfo
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Key { get; set; }
        
        public virtual ApplicationUser User { get; set; }
        public virtual WorkSchedule WorkSchedule { get; set; }
    }
}