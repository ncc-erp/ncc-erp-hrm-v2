using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HRMv2.WebServices.Timesheet
{
    public class TimesheetConfig
    {
        public string Uri { get; set; }
        public string BaseAddress { get; set; }
        public string SecurityCode { get; set; } = "";
    }
}
