using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Journal3.Models
{
    public class WorkSchedule
    {
        [Key, ForeignKey("UserInfo")]
        public int UserInfoId { get; set; }
        //public int Id { get; set; }
        public string Name { get; set; }
        public TimeSpan StartWork { get; set; }
        public TimeSpan EndWork { get; set; }

        public virtual UserInfo UserInfo { get; set; }
    }
}