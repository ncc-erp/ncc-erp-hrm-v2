using Abp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMv2.Entities
{
    public class RefundEmployee : NccAuditEntity, IMayHaveTenant
    {
        public int? TenantId { get; set; }
        public long RefundId { get; set; }
        [ForeignKey(nameof(RefundId))]
        public Refund Refund { get; set; }
        public long EmployeeId { get; set; }
        [ForeignKey(nameof(EmployeeId))]
        public Employee Employee { get; set; }
        public double Money { get; set; }
        public string Note { get; set; }
    }
}
