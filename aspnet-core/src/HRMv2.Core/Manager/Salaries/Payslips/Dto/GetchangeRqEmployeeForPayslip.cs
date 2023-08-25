using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HRMv2.Constants.Enum.HRMEnum;

namespace HRMv2.Manager.Salaries.Payslips.Dto
{
    public class GetchangeRqEmployeeForPayslip
    {
        public long EmployeeId { get; set; }
        public DateTime ApplyDate { get; set; }
        public double Salary { get; set; }
        public SalaryRequestType Type { get; set; }
        public UserType UserType { get; set; }
    }
}
