using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMv2.Manager.Notifications.Email.Dto
{
    public class BonusMailTemplateDto
    {
        public string EmployeeFullName { get; set; }
        public string SendToEmail { get; set; }
        public string BonusMoney { get; set; }
        public string ApplyMonth { get; set; }
        public string BonusName { get; set; }
        public string Subject => $"[NCC][{EmployeeFullName}] {BonusName} ";
    }

    public class GetBonusEmployeeForSendMailDto
    {
        public string BonusName { get; set; }
        public long BonusId { get; set; }
        public double Money { get; set; }
        public DateTime ApplyMonth { get; set; }
        public string EmployeeFullName { get; set; }
        public string EmployeeEmail { get; set; }

    }
}
