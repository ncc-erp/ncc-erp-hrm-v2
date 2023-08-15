using Abp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HRMv2.Constants.Enum.HRMEnum;

namespace HRMv2.Entities
{
    public class SalaryChangeRequest : NccAuditEntity, IMayHaveTenant
    {
        public int? TenantId { get; set; }
        [Required]
        [StringLength(500)]
        public string Name { get; set; }
        public DateTime ApplyMonth { get; set; }
        public SalaryRequestStatus Status { get; set; }
    }
}
