using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMv2.Manager.MezonTokens.Dto
{
    public class MmnTransferTokenDto
    {
        public string sender_id { get; set; }
        public string sender_name { get; set;}
        public string receiver_id { get; set; }
        public double amount { get; set; }
        public string note { get; set; }
        public string JwtTokenBot { get; set; }

}

    public class InputSendMezonToken
    {
        public long MezonTokenId { get; set; }
        public string? JwtTokenBot { get; set; }
        public int? TenantId { get; set; }
        public long? CurrentUserLoginId { get; set; }
    }
}
