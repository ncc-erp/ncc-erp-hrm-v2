using HRMv2.Manager.Notifications.Email.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMv2.Manager.Bonuses.Dto
{
    public class SendMailBonusDto
    {
        public long BonusEmployeeId { get; set; }
        public MailPreviewInfoDto MailContent { get; set; }
    }
}
