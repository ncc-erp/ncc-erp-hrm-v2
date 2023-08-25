using HRMv2.Manager.Notifications.Email.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMv2.Manager.SalaryRequests.Dto
{
    public class SendMailCheckpointDto
    {
        public long requestId { get; set; }
        public MailPreviewInfoDto MailContent { get; set; }
    }
}
