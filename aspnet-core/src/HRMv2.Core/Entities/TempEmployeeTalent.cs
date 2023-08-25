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
    public class TempEmployeeTalent : NccAuditEntity, IMayHaveTenant
    {
        public int? TenantId { get; set; }
        public string NCCEmail { get; set; }
        public string PersonalEmail { get; set; }
        public string FullName { get; set; }
        public Sex? Gender { get; set; }
        public UserType? UserType { get; set; }
        public long? BranchId { get; set; }
        [ForeignKey(nameof(BranchId))]
        public Branch Branch { get; set; }
        public long? JobPositionId { get; set; }
        [ForeignKey(nameof(JobPositionId))]
        public JobPosition JobPosition { get; set; }
        public long? LevelId { get; set; }
        [ForeignKey(nameof(LevelId))]
        public Level Level { get; set; }
        public string Phone { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public DateTime? OnboardDate { get; set; }
        public TalentOnboardStatus? Status { get; set; }
        public double? Salary { get; set; }
        public int? ProbationPercentage { get; set; }
        public string Skills { get; set; }
    }
}
