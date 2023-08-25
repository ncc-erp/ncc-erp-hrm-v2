using HRMv2.Manager.Notifications.Email.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMv2.Manager.Salaries.Payslips.Dto
{
    public class SendMailOneemployeeDto
    {
        public MailPreviewInfoDto MailContent { get; set; }
        public DateTime Deadline { get; set; }
        public long PayslipId { get; set; }
    }
}
