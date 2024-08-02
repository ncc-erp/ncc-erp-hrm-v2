using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMv2.Manager.Notifications.Email.Dto
{
    public class InputPayslipLinkMailTemplate
    {
        public string EmployeeFullName { get; set; }
        public string PayrollYear { get; set; }
        public string PayrollMonth { get; set; }
        public string SendToEmail { get; set; }
        public string SalaryLink { get; set; }
        public string ComplainDeadline { get; set; }
        public string Subject => $"[NCC][{EmployeeFullName}] THÔNG BÁO CHI TIẾT LƯƠNG THÁNG {PayrollMonth}/{PayrollYear}";
    }
}
