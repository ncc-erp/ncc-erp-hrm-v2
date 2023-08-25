using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMv2.Manager.Notifications.Templates.Dto
{
    public class MailInfoDto
    {
        public string Name { get; set; }
        public string BodyMessage { get; set; }
        public string Subject { get; set; }
        public string Description { get; set; }
    }
}
