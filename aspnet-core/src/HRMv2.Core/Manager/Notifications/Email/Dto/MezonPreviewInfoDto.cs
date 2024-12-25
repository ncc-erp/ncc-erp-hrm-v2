using HRMv2.Manager.Notifications.SendMezonDM.Dto;
using HRMv2.Utils;
using Newtonsoft.Json;
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
        public string Name { get; set; }
        public string BodyMessage { get; set; }
        public long? CurrentUserLoginId { get; set; }
        public string MezonUsername { get; set; }
        public int? TenantId { get; set; }
        public InputMezonDM InputMezonDM => JsonConvert.DeserializeObject<InputMezonDM>(BodyMessage);

    }
}
