using HRMv2.Manager.Notifications.Email.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMv2.Manager.Salaries.Payslips.Dto
{
    public class GetPayslipMailContentDto
    {
        public DateTime? Deadline { get; set; }
        public MailPreviewInfoDto MailInfo { get; set; }
    }
}
