using System.Collections.Generic;
using HRMv2.Utils;
using HRMv2.Validation;
using Newtonsoft.Json;
namespace HRMv2.Manager.Notifications.SendMezonDM.Dto
{
    [JsonObject]
    public class InputMezonDM
    {
        [JsonProperty(PropertyName = "content")]
        [JsonConverter(typeof(ContentJsonConverter))]
        public MezonDMContent Content { get; set; }

        [JsonProperty(PropertyName = "attachments")]
        public List<Attachment> Attachments { get; set; }
    }


    [JsonObject]
    public class MezonDMContent
    {
        [JsonProperty(PropertyName = "t")]
        public string Text { get; set; }

        [JsonProperty(PropertyName = "mk")]
        public List<MK_Link> MK => MezonDMUtil.GetListIndexOfLinks(Text);
    }

    public class MK_Link
    {
        [JsonProperty(PropertyName = "type")]
        public string Type { get; set; }

        [JsonProperty(PropertyName = "s")]
        public int Start { get; set; }
        [JsonProperty(PropertyName = "e")]
        public int End { get; set; }
    }

    public class Attachment
    {
        [JsonProperty(PropertyName = "url")]
        public string Url { get; set; }
        [JsonProperty(PropertyName = "filetype")]
        public string Filetype { get; set; }
    }

}
