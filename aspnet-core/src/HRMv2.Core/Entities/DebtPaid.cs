using Abp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HRMv2.Constants.Enum.HRMEnum;

namespace HRMv2.Entities
{
    public class DebtPaid : NccAuditEntity, IMayHaveTenant
    {
        public int? TenantId { get; set; }
        public long DebtId { get; set; }
        [ForeignKey(nameof(DebtId))]
        public Debt Debt { get; set; }
        public double Money { get; set; }
        public DateTime Date { get; set; }
        [Required]
        public DebtPaymentType PaymentType { get; set; }
        public long? PayslipDetailId { get; set; }
        public string Note { get; set; }
    }
}
