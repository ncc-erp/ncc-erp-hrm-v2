using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMv2.Manager.Timesheet.Dto
{
    public class InputConfirmPayslipMailDto
    {
        public string Email { get; set; }
        public long PayslipId { get; set; }
    }
}
