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
    public class Debt : NccAuditEntity, IMayHaveTenant
    {
        public int? TenantId { get; set; }
        public long EmployeeId { get; set; }
        [ForeignKey(nameof(EmployeeId))]
        public Employee Employee { get; set; }
        public double InterestRate { get; set; }
        public double Money { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Note { get; set; }
        [Required]
        public DebtPaymentType PaymentType { get; set; }
        [Required]
        public DebtStatus Status { get; set; }
    }
}