using Abp.Domain.Entities.Auditing;
using HRMv2.Authorization.Users;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMv2.Entities
{
    public abstract class NccAuditEntity : FullAuditedEntity<long>
    {
        [ForeignKey(nameof(CreatorUserId))]
        public User CreatorUser { get; set; }
        [ForeignKey(nameof(LastModifierUserId))]
        public User LastModifierUser { get; set; }
    }
}
