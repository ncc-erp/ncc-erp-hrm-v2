using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMv2.WebServices.Mezon.Dto
{
    public class AuthOauth2Mezon
    {
        public List<string> aud { get; set; } 
        public long auth_time { get; set; }
        public long iat { get; set; }
        public string iss {  get; set; }
        public long rat { get; set; }
        public string sub {  get; set; }    
    }

    public class MezonHashAuthDto
    {
        public string HashData { get; set; }
        public string TenancyName { get; set; }
    }

    public class BaseHashData
    {
        public string query_id { get; set; }
        public string user { get; set; }
        public long auth_date { get; set; }
        public string signature { get; set; }
    }
    public class HashData : BaseHashData
    {
        public string hash { get; set; }
    }
}
