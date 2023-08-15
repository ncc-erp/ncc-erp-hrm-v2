using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMv2.Entities
{
    public class EmployeeSkill : FullAuditedEntity<long>
    {
        public long EmployeeId { get; set; }
        [ForeignKey(nameof(EmployeeId))]
        public Employee Employee { get; set; }
        public long SkillId { get; set; }
        [ForeignKey(nameof(SkillId))]
        public Skill Skill { get; set; }
    }
}
