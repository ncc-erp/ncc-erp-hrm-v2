using HRMv2.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HRMv2.Constants.Enum.HRMEnum;

namespace HRMv2.Manager.Notifications.Email.Dto
{
    public class MezonPreviewInfoDto
    {
        public long TemplateId { get; set; }
        public string Name { get; set; }
        public MailFuncEnum MailFuncType { get; set; }
        public string BodyMessage { get; set; }
        public string Subject { get; set; }
        public string[] PropertiesSupport { get; set; }
        public long? CurrentUserLoginId { get; set; }
        public string SendToUser { get; set; }
        public int? TenantId { get; set; }
        public List<string> ListCC { get; set; }
        public TemplateType TemplateType => CommonUtil.GetTemplateType(MailFuncType);
    }
}
