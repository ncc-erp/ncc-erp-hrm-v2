using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMv2.Manager.MezonTokens.Dto
{
    public class AddTokenToMultipleUserDto
    {
        public List<long> EmployeeIds { get; set; }
        public long Amount { get; set; }
        public string Note { get; set; }
        public long? PayrollId { get; set; }
        public long? ReferenceId { get; set; }
        public DateTime? SentToEmployeeAt { get; set; }
    }
}
