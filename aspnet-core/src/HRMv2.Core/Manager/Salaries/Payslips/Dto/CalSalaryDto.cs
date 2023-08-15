using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMv2.Manager.Salaries.Payslips.Dto
{
    public class CalSalaryDto
    {
        public long PayslipId { get; set; }
        public double TotalBonus { get; set; }
        public double TotalBenefit { get; set; }
        public double TotalPunishment { get; set; }
        public double TotalDebt { get; set; }
        public double SalaryNormal { get; set; }
        public double SalaryOT { get; set; }
        public double SalaryMaternityLeave { get; set; }
    }
}
