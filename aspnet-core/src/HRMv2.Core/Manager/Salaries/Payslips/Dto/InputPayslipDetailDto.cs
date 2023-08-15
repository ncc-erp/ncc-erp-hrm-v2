using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMv2.Manager.Salaries.Payslips.Dto
{
    public class InputPayslipDetailDto
    {
        public long EmployeeId { get; set; }
        public string Note { get; set; }
        public double Money { get; set; }
        public long ReferenceId { get; set; }
    }
}
