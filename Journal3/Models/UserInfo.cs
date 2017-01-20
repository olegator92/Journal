using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Journal3.Models
{
    public class UserInfo
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Key { get; set; }
        public TimeSpan StartWork { get; set; }
        public TimeSpan EndWork { get; set; }

        public virtual ApplicationUser User { get; set; }
    }
}