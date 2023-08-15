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
    public class PayslipSalary : NccAuditEntity, IMayHaveTenant
    {
        public int? TenantId { get; set; }
        public long PayslipId { get; set; }
        [ForeignKey(nameof(PayslipId))]
        public Payslip Payslip { get; set; }
        public DateTime Date { get; set; }
        public double Salary { get; set; }
        public string Note { get; set; }
    }
}
