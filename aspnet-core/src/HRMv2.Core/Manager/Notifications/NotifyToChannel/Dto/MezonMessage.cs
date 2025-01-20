using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMv2.Manager.Notifications.NotifyToChannel.Dto
{
    public class MezonMessage
    {
        public string username { get; set; }
        public string t { set; get; }
        public List<MezonMessageMention> mentions { set; get; }
    }

    public class MezonMessageMention
    {
        public string username { set; get; }
        public int s { set; get; }
        public int e => s + username.Length +1;
    }
}
