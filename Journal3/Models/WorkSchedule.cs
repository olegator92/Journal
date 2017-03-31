using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Journal3.Models
{
    public class WorkSchedule
    {
        public int Id { get; set; }
        //public int UserInfoId { get; set; }
        public string Name { get; set; }
        public TimeSpan StartWork { get; set; }
        public TimeSpan EndWork { get; set; }

        public virtual UserInfo UserInfo { get; set; }
    }
}