using Abp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMv2.Manager.Salaries.Payslips.Dto
{
    public class CollectPayslipDto:IMayHaveTenant
    {
        public long PayrollId { get; set; }
        public List<long> EmployeeIds { get; set; }
        public int? TenantId { get ; set ; }
        public long? CurrentUserLoginId { get; set; }
    }
}
