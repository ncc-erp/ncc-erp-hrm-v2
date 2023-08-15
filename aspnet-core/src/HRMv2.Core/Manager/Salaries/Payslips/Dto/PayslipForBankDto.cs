using HRMv2.Manager.Common.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HRMv2.Constants.Enum.HRMEnum;

namespace HRMv2.Manager.Salaries.Payslips.Dto
{
    public class PayslipForBankDto
    {
        public EmployeeStatus Status { get; set; }
        public string EmployeeName { get; set; }
        public double Salary { get; set; }
        public string BankAccountNumber { get; set; }
        public string BankName { get; set; }
        public string Note { get; set; }
    }
}
