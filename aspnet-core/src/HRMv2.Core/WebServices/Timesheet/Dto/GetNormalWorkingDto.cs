using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMv2.WebServices.Timesheet
{
    public class GetNormalWorkingDto
    {
        public string NormalizedEmailAddress { get; set; }
        public double TotalWorkingHour { get; set; }
        public double TotalWorkingHourOfMonth { get; set; }
        public double TotalOpenTalk { get; set; }
        public IEnumerable<UserNormalWorkingDto> ListWorkingHour { get; set; }
    }

    public class UserNormalWorkingDto
    {
        public int Day { get; set; }
        public string DayName { get; set; }
        public double WorkingHour { get; set; }
        public bool IsOpenTalk { get; set; }
    }
}
