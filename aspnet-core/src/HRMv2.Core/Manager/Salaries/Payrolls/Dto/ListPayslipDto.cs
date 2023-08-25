using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMv2.Manager.Salaries.Payrolls.Dto
{
    public class ListPayslipDto
    {
        public long EmployeeId { get; set; }
        public float LeaveDayAfter { get; set; }
        public double Salary { get; set; }
    }
}
