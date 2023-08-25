using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HRMv2.Constants.Enum.HRMEnum;

namespace HRMv2.Manager.SalaryRequests.Dto
{
    public class ChangeRequestStatusDto
    {
        public long SalaryRequestId { get; set; }
        public SalaryRequestStatus Status { get; set; }
    }
}
