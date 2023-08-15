using Abp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMv2.Entities
{
    public class EmployeeBranchHistory : NccAuditEntity, IMayHaveTenant
    {
        public int? TenantId { get; set; }
        public long EmployeeId { get; set; }
        public long BranchId { get; set; }
        [ForeignKey(nameof(BranchId))]
        public Branch Branch { get; set; }
        public string Note { get; set; }
        public DateTime DateAt { get; set; }
    }
}
