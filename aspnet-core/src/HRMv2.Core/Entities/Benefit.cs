using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HRMv2.Constants.Enum.HRMEnum;

namespace HRMv2.Entities
{
    public class Benefit : NccAuditEntity, IMayHaveTenant
    {
        public int? TenantId { get; set; }
        [Required]
        [StringLength(256)]
        public string Name { get; set; }
        public double Money { get; set; }
        public BenefitType Type { get; set; }
        public bool IsActive { get; set; }
        public bool IsBelongToAllEmployee { get; set; }
        public DateTime ApplyDate { get; set; }
    }
}
