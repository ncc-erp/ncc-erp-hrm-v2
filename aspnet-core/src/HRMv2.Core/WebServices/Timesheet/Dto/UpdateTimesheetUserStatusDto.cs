using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMv2.WebServices.Timesheet
{
    public class UpdateTimesheetUserStatusDto
    {
        public string EmailAddress { get; set; }
        public bool IsStopWork { get; set; }
        public bool IsActive { get; set; }
        public DateTime? StopWorkingTime { get; set; }
    }
}
