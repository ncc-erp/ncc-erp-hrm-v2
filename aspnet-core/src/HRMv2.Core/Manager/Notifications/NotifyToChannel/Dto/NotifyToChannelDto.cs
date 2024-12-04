using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMv2.Manager.Notifications.NotifyToChannel.Dto
{
    public class NotifyToChannelDto
    {
        public string NotifyPlatform { get; set; }
        public string ITChannel { get; set; }
        public string PayrollChannel { get; set; }
        public string SendDMToMezon { get; set; }
    }

    public class NotifyToPlatformDto
    {
        public string ToPlatform { get; set; }
    }
}
