using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMv2.Manager.Salaries.Payslips.Dto
{
    public class InputToUpdateRemainLeaveDaysDto
    {
        public IFormFile File { get; set; }
        public long PayrollId { get; set; }
    }

    public class ResponseFailedDto
    {
        public int Row { get; set; }
        public string Email { get; set; }
        public float RemainLeaveDays { get; set; }
        public string ReasonFail { get; set; }

    }

    public class EmployeeRemainLeaveDaysAfter
    {
        public string Email { get; set; }
        public float RemainLeaveDayAfter { get; set; }
    }

    public class EmailPayslipDto
    {
        public string Email { get; set; }
    }
}
