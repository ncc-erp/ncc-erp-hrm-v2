using HRMv2.Manager.Common.Dto;
using HRMv2.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HRMv2.Constants.Enum.HRMEnum;

namespace HRMv2.Manager.Salaries.Payslips.Dto
{
    public class GetNotPayslipEmployeeDto: BaseEmployeeDto
    {
        public double RealSalary { get; set; }
        public long EmployeeId { get; set; }
    }
}
