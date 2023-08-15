using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMv2.Entities
{
    public class Punishment : NccAuditEntity, IMayHaveTenant
    {
        public int? TenantId { get; set; }
        public long? PunishmentTypeId { get; set; }
        [ForeignKey(nameof(PunishmentTypeId))]
        public PunishmentType PunishmentType { get; set; }
        [Required]
        [StringLength(256)]
        public string Name { get; set; }
        public DateTime Date { get; set; }
        public bool IsActive { get; set; }
    }
}
