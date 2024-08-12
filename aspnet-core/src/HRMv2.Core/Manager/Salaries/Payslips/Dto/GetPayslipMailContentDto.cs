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
    public class GetPayslipToConfirmDto: GetPayslipMailContentDto
    {      
        public CheckValidType CheckValidType { get; set; }
        public string Message { get; set; }
     
    }

    public enum CheckValidType
    {
        Valid = 1,
        InvalidBecauseEmployeePauseOrQuit = -1,
        InvalidBecauseEmployeeViewOther = -2,
    }


}
