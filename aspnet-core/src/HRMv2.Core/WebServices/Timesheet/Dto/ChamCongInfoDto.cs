using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMv2.WebServices.Timesheet.Dto
{
    public class ChamCongInfoDto
    {
        public string NormalizeEmailAddress { get; set; }
        public List<DateTime> OpenTalkDates { get; set; }
        public List<DateTime> NormalWorkingDates { get; set; }
        public int TotalOpentalkCount
        {
            get
            {
                return OpenTalkDates.Count;
            }
        }
    }
}
