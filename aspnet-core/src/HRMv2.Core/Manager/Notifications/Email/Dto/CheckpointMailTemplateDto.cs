using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMv2.Manager.Notifications.Email.Dto
{
    public class CheckpointMailTemplateDto
    {
        public string CheckpointName { get; set; }
        public string EmployeeFullName { get; set; }
        public string SendToEmail { get; set; }
        public string OldLevel { get; set; }
        public string NewLevel { get; set; }
        public string OldSalary { get; set; }
        public string NewSalary { get; set; }
        public string ApplyDate { get; set; }
        public string Subject => $"[NCC][{EmployeeFullName}] {CheckpointName} ";

    }

    public class GetSalaryChangeRequestForSendMailDto
    {
        public string CheckpointName { get; set; }
        public long requestId { get; set; }
        public string EmployeeFullName { get; set; }
        public string EmployeeEmail { get; set; }
        public long OldLevelId { get; set; }
        public long NewLevelId { get; set; }
        public double OldSalary { get; set; }
        public double NewSalary { get; set; }
        public DateTime ApplyDate { get; set; }

    }
}
