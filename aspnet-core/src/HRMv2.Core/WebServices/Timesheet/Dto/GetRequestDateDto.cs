using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMv2.WebServices.Timesheet.Dto
{
    public class GetRequestDateDto
    {
        public string NormalizedEmailAddress { get; set; }
        public List<OffDateDto> OffDates { get; set; }
        public HashSet<DateTime> WorkAtHomeOnlyDates { get; set; }
        public List<OffDateDto> OffDateLastMonth { get; set; }
    }
    public class OffDateDto
    {
        public DateTime DateAt { get; set; }
        public float DayValue { get; set; }
        public long DayOffTypeId { get; set; }
        public int LeaveDay { get; set; }
    }
}
