using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMv2.Manager.Notifications.SendDMToMezon.Dto
{
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    namespace HRMv2.Manager.Notifications.SendDMToMezon.Dto
    {
        [JsonObject]
        public class ContentMezonDM
        {
            [JsonProperty(PropertyName = "t")]
            public string Text { get; set; }
            [JsonProperty(PropertyName = "lk")]
            public List<StartEnd> Link { get; set; }
        }

        public class StartEnd
        {
            [JsonProperty(PropertyName = "s")]
            public int Start { get; set; }
            [JsonProperty(PropertyName = "e")]
            public int End { get; set; }
        }
    }

}
