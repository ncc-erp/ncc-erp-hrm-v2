using Abp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HRMv2.Constants.Enum.HRMEnum;

namespace HRMv2.Entities
{
    public class SalaryChangeRequestEmployee : NccAuditEntity, IMayHaveTenant
    {
        public int? TenantId { get; set; }
        public long? SalaryChangeRequestId { get; set; }
        [ForeignKey(nameof(SalaryChangeRequestId))]
        public SalaryChangeRequest SalaryChangeRequest { get; set; }
        public long EmployeeId { get; set; }
        [ForeignKey(nameof(EmployeeId))]
        public Employee Employee { get; set; }
        public long LevelId { get; set; }
        public long ToLevelId { get; set; }
        public UserType FromUserType { get; set; }
        public UserType ToUserType { get; set; }
        public long JobPositionId { get; set; }
        public long ToJobPositionId { get; set; }
        public double Salary { get; set; }
        public double ToSalary { get; set; }
        public DateTime ApplyDate { get; set; }
        public string Note { get; set; }
        public SalaryRequestType Type { get; set; }
        public bool HasContract { get; set; }
    }
}
