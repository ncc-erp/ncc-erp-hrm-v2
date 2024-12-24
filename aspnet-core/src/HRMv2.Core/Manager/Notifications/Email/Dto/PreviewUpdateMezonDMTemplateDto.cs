using HRMv2.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HRMv2.Constants.Enum.HRMEnum;

namespace HRMv2.Manager.Notifications.Email.Dto
{
    public class PreviewUpdateMezonDMTemplateDto
    {
        public long Id {  get; set; }
        public string Name { get; set; }
        public string BodyMessage { get; set; }
        public string[] PropertiesSupport { get; set; }
        public NotifyTemplateEnum Type { get; set; }
        public TemplateType TemplateType => CommonUtil.GetTemplateType(Type);
    }
}
