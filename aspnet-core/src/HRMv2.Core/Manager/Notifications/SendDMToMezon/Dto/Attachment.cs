using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
namespace HRMv2.Manager.Notifications.SendDMToMezon.Dto
{
    [JsonObject]
    public class Attachment
    {
        [JsonProperty(PropertyName = "url")]
        public string Url { get; set; }
        [JsonProperty(PropertyName = "filetype")]
        public string Filetype { get; set; }
    }
}
