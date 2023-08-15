using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMv2.Manager.Salaries.Payslips.Dto
{
    public class GetPayslipInfoBeforeUpdateDto:UpdatePayslipInfoDto
    {
        public double Salary { get; set; }  //payslip
    }
}
