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
        public ContentMezonDM Content { get; set; }

        [JsonProperty(PropertyName = "attachments")]
        public List<Attachment> Attachments { get; set; }
    }

    [JsonObject]
    public class ContentMezonDM
    {
        [JsonProperty(PropertyName = "t")]
        public string Text { get; set; }
        [JsonProperty(PropertyName = "lk")]
        public List<StartEnd> Link => MezonDMUtil.GetListIndexOfLinks(Text);
    }

    public class StartEnd
    {
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
