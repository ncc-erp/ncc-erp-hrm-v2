using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMv2.Manager.MezonTokens.Dto
{
    public class ExportMezonTokenDto
    {
        public string EmailAddress { get; set; }    
        public long Amount {  get; set; }
        public string Note { get; set; }
        public string Status {  get; set; }
        public DateTime? SentAt { get; set; }
    }
}
