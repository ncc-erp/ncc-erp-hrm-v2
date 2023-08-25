using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HRMv2.Constants.Enum.HRMEnum;

namespace HRMv2.Manager.Salaries.Payslips.Dto
{
    public class EmployeeWorkingStatusDto
    {
        public long EmployeeId { get; set; }
        public DateTime DateAt { get; set; }
        public EmployeeStatus Status { get; set; }
    }
}
