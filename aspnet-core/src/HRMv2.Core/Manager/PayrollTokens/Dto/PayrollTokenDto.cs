using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMv2.Manager.PayrollTokens.Dto
{
    public class PayrollTokenDto
    {
        public long PayrollId {  get; set; }
        public DateTime ApplyMonth { get; set; }   
    }
}
