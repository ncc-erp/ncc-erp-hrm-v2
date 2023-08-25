using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMv2.Manager.Categories.AuditLogs.Dto
{
    public class GetAllEmailAddressInAuditLogDto
    {
        public long UserId { get; set; }
        public string EmailAddress { get; set; }
    }
}
