using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMv2.Manager.Notifications.Email.Dto
{
    public class PayrollMailTemplateDto
    {
        public DateTime PayrollMonth { get; set; }
        public string ApplyMonth => PayrollMonth.ToString("MM/yyyy");
        public string ConfirmUrl {get; set; }
        public string PayrollStatus { get; set; }
        public string Subject => $"[NCC] Bảng lương tháng {ApplyMonth} [{PayrollStatus}]";
        public string SendToEmail { get; set; }
        public string Deadline => $"05/{PayrollMonth.AddMonths(1).ToString("MM/yyyy")}";
    }
}
