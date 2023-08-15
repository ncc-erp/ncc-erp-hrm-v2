using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMv2.WebServices.Timesheet.Dto
{
    public class InputCollectDataForPayslipDto
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public List<string> UpperEmails { get; set; }
    }
}
