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
    public class DebtPaymentPlan : NccAuditEntity, IMayHaveTenant
    {
        public int? TenantId { get; set; }
        public long DebtId { get; set; }
        [ForeignKey(nameof(DebtId))]
        public Debt Debt { get; set; }
        public DateTime Date { get; set; }
        public double Money { get; set; }
        public string Note { get; set; }
        public DebtPaymentType PaymentType { get; set; }
    }
}
