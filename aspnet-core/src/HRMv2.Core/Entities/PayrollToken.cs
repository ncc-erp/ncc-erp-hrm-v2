using Abp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HRMv2.Constants.Enum.HRMEnum;

namespace HRMv2.Entities
{
    public class PayrollToken : NccAuditEntity, IMayHaveTenant
    {
        public int? TenantId { get; set; }
        public long EmployeeId { get; set; }
        public string Note { get; set; }
        public int Amount { get; set; }
        public StatusSendToken Status { get; set; } 
        /// <summary>
        /// Benefit ăn trưa thì refereceId = payslipDetail.Id
        /// </summary>
        public long ReferenceId { get; set; }

    }
}
