using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMv2.Entities
{
    public class PunishmentEmployee : NccAuditEntity, IMayHaveTenant
    {
        public int? TenantId { get; set; }
        public long PunishmentId { get; set; }
        [ForeignKey(nameof(PunishmentId))]
        public Punishment Punishment { get; set; }
        public long EmployeeId { get; set; }
        [ForeignKey(nameof(EmployeeId))]
        public Employee Employee { get; set; }
        public double Money { get; set; }
        public string Note { get; set; }
    }
}
