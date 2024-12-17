using HRMv2.Manager.Notifications.SendDMToMezon.Dto.HRMv2.Manager.Notifications.SendDMToMezon.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
namespace HRMv2.Manager.Notifications.SendDMToMezon.Dto
{
    [JsonObject]
    public class InputMezonDM
    {
        [JsonProperty(PropertyName = "content")]
        public ContentMezonDM Content { get; set; }
        [JsonProperty(PropertyName = "attachments")]
        public List<Attachment> Attachments { get; set; }
    }
}
