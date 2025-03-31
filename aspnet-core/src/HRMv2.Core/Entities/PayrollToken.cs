using Abp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HRMv2.Constants.Enum.HRMEnum;

namespace HRMv2.Entities
{
    public class PayrollToken : NccAuditEntity, IMayHaveTenant
    {
        public int? TenantId { get; set; }
        public string EmailAddress { get; set; }
        public string Note { get; set; }
        public int TokenMezon { get; set; }
        public StatusSendToken Status { get; set; }
        public string Month {  get; set; }
        public long PayrollId { get; set; }

    }
}
