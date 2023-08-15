using Abp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMv2.Entities
{
    public class PayslipTeam : NccAuditEntity, IMayHaveTenant
    {
        public int? TenantId { get; set; }
        public long PayslipId { get; set; }
        [ForeignKey(nameof(PayslipId))]
        public Payslip Payslip { get; set; }
        public long TeamId { get; set; }
    }
}
