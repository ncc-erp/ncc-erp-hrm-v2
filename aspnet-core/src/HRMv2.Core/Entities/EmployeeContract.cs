using Abp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HRMv2.Constants.Enum.HRMEnum;

namespace HRMv2.Entities
{
    public class EmployeeContract : NccAuditEntity, IMayHaveTenant
    {
        public int? TenantId { get; set; }
        public long EmployeeId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string File { get; set; }
        [Required]
        [StringLength(100)]
        public string Code { get; set; }
        public UserType UserType { get; set; }
        public long JobPositionId { get; set; }
        [ForeignKey(nameof(JobPositionId))]
        public JobPosition JobPosition { get; set; }
        public long LevelId { get; set; }
        [ForeignKey(nameof(LevelId))]
        public Level Level { get; set; }
        public long SalaryRequestEmployeeId { get; set; }
        [ForeignKey(nameof(SalaryRequestEmployeeId))]
        public SalaryChangeRequestEmployee SalaryChangeRequestEmployee { get; set; }
        [StringLength(1000)]
        public string FilePath { get; set; }
        public double BasicSalary { get; set; }
        public double RealSalary { get; set; }
        public double ProbationPercentage { get; set; }
        public string Note { get; set; }
    }
}