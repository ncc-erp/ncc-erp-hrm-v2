using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMv2.WebServices.Mezon.Dto
{
    public class AuthData
    {
        public string token { get; set; }
        public string refresh_token { get; set; }
        public string user_id { get; set; }

    }  

    public class AuthResponse
    {
        public int code { get; set; }
        public string message { get; set; }
    }

}
