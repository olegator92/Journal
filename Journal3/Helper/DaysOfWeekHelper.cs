using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Journal3.Helper
{
    public static class DaysOfWeekHelper
    {
        public static string GetDayName(Int32 number)
        {
            switch (number)
            {
                case 1: return "Понедельник";break;
                case 2: return "Вторник"; break;
                case 3: return "Среда"; break;
                case 4: return "Четверг"; break;
                case 5: return "Пятница"; break;
                case 6: return "Суббота"; break;
                case 0: return "Воскресенье"; break;
            }
            return "";
        }
    }
}