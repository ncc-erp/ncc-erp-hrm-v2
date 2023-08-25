using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMv2.Manager.Salaries.Payslips.Dto
{
    public class SendMailAllEmployeeDto
    {
        public long PayrollId { get; set; }
        public DateTime Deadline { get; set; }
    }
}
