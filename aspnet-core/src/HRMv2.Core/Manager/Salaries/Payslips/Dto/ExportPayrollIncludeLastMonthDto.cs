using HRMv2.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HRMv2.Constants.Enum.HRMEnum;

namespace HRMv2.Manager.Salaries.Payslips.Dto
{
    public class ExportPayrollIncludeLastMonthDto : GetPayslipEmployeeDto
    {

        public double RealSalaryLastMonth { get; set; }
        public double NormalSalaryLastMonth { get; set; }
        public double LeaveDayLastMonth { get; set; }
        public string BankName { get; set; }
        public string BankAccountNumber { get; set; }
        public PayslipStatusForExport ExportStatus { get; set; }
        public List<InputsalaryDto> ListInputSalaryLastMonth { get; set; }

    }
}
