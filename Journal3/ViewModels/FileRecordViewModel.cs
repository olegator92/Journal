using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Journal3.ViewModels
{
    public class FileRecordViewModel
    {
        public DateTime Date { get; set; }
        public TimeSpan Time { get; set; }
        public string Key { get; set; }
    }
}