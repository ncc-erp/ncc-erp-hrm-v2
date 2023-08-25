using Abp.Domain.Entities;
using System.ComponentModel.DataAnnotations;

namespace HRMv2.Entities
{
    public class IssuedBy : NccAuditEntity, IMayHaveTenant
    {
        public int? TenantId { get; set; }
        [Required]
        [StringLength(256)]
        public string Name { get; set; }
    }
}