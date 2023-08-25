using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMv2.Entities
{
    public class EmployeeTeam : FullAuditedEntity<long>
    {
        public long EmployeeId { get; set; }
        [ForeignKey(nameof(EmployeeId))]
        public Employee Employee { get; set; }
        public long TeamId { get; set; }
        [ForeignKey(nameof(TeamId))]
        public Team Team { get; set; }
    }
}
