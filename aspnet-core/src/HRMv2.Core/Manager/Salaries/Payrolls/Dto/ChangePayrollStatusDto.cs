using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HRMv2.Constants.Enum.HRMEnum;

namespace HRMv2.Manager.Payrolls.Dto
{
    public class ChangePayrollStatusDto
    {
        public long PayrollId { get; set; }
        public PayrollStatus Status { get; set; }
    }
}
