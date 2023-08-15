using Abp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HRMv2.Constants.Enum.HRMEnum;

namespace HRMv2.Entities
{
    public class Payroll : NccAuditEntity, IMayHaveTenant
    {
        public int? TenantId { get; set; }
        public DateTime ApplyMonth { get; set; }
        public PayrollStatus Status { get; set; }
        public float NormalWorkingDay { get; set; }
        public int OpenTalk { get; set; }
        
    }
}