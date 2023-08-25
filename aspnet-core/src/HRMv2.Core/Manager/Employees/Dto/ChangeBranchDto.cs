using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HRMv2.Constants.Enum.HRMEnum;

namespace HRMv2.Manager.Employees.Dto
{
    public class ChangeBranchDto
    {
        public int? TenantId { get; set; }
        public long? CurrentUserLoginId { get; set; }
        public long EmployeeId { get; set; }
        public long BranchId { get; set; }
        public DateTime Date { get; set; }

    }
}
