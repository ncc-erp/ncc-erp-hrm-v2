using HRMv2.Manager.Notifications.Email.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HRMv2.Constants.Enum.HRMEnum;

namespace HRMv2.Manager.Salaries.Payslips.Dto
{
    public class GetPayslipMailContentDto
    {
        public DateTime? Deadline { get; set; }
        public MailPreviewInfoDto MailInfo { get; set; }
      
    }
    public class GetPayslipLinkMailContentDto
    {
        public DateTime? Deadline { get; set; }
        public MailPreviewInfoDto MailInfo { get; set; }
        public EmployeeStatus Status { get; set; }
        public string FullName { get; set; }
        public string CurrentUserLoginEmail { get; set; }
        public string? CurrentUserLoginFullName { get; set; }

        public bool Valid { get; set; }

    }
    

}
