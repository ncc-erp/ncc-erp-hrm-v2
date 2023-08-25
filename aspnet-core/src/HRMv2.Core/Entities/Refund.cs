using Abp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMv2.Entities
{
    public class Refund : NccAuditEntity, IMayHaveTenant
    {
        public int? TenantId { get; set; }
        public string Name { get; set; }
        public DateTime Date { get; set; }
        public bool IsActive { get; set; }
    }
}
