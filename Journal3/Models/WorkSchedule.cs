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
            SpecialSchedules = new List<SpecialSchedule>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        [RegularExpression(@"^([0-9]|0[0-9]|1[0-9]|2[0-3]):[0-5][0-9]:[0-5][0-9]$", ErrorMessage = "Неверный формат времени")]
        public TimeSpan StartWork { get; set; }
        [RegularExpression(@"^([0-9]|0[0-9]|1[0-9]|2[0-3]):[0-5][0-9]:[0-5][0-9]$", ErrorMessage = "Неверный формат времени")]
        public TimeSpan EndWork { get; set; }
        public bool IsSpecial { get; set; }

        public virtual ICollection<SpecialSchedule> SpecialSchedules { get; set; }
        public virtual ICollection<Record> Records { get; set; }
        public virtual ICollection<UserInfo> UserInfos { get; set; }

    }
}