using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMv2.Authorization.Users
{
    public class MezonUser
    {
        public string id { get; set; }
        public string username { get; set; }
        public string display_name { get; set; }
        public string avatar_url { get; set; }
        public string mezon_id { get; set; }
        public string sub { get; set; }
    }
}
