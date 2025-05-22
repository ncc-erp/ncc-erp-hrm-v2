using Abp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HRMv2.Constants.Enum.HRMEnum;

namespace HRMv2.Entities
{
    public class MezonToken : NccAuditEntity, IMayHaveTenant
    {
        public int? TenantId { get; set; }
        public long EmployeeId { get; set; }
        [ForeignKey(nameof(EmployeeId))]
        public Employee Employee { get; set; }
        public string Note { get; set; }
        public long Amount { get; set; }
        public DateTime? SentToEmployeeAt { get; set; }
        public StatusSendToken Status { get; set; } 
        /// <summary>
        /// Tạo từ PayslipDetail, Benefit ăn trưa thì refereceId = payslipDetail.Id
        /// Tạo từ MezonToken , referenceId = -1
        /// </summary>
        public long ReferenceId { get; set; }
        public long? PayrollId { get; set; }

        [ForeignKey(nameof(PayrollId))]
        public Payroll Payroll { get; set; }

    }
}
