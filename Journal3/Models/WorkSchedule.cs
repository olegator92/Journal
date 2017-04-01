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

        public WorkSchedule()
        {
            Records = new List<Record>();
            UserInfos = new List<UserInfo>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public TimeSpan StartWork { get; set; }
        public TimeSpan EndWork { get; set; }

        public virtual ICollection<Record> Records { get; set; }
        public virtual ICollection<UserInfo> UserInfos { get; set; }
    }
}