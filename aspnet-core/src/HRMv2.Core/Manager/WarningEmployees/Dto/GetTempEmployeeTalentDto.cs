using HRMv2.Manager.Common.Dto;
using HRMv2.Utils;
using NccCore.Anotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HRMv2.Constants.Enum.HRMEnum;

namespace HRMv2.Manager.WarningEmployees.Dto
{
    public class GetTempEmployeeTalentDto
    {
        public long Id { get; set; }
        [ApplySearch]
        public string NCCEmail { get; set; }
        [ApplySearch]
        public string FullName { get; set; }
        [ApplySearch]
        public string Email { get; set; }
        public Sex? Sex { get; set; }
        public string Phone { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public DateTime? OnboardDate { get; set; }
        public TalentOnboardStatus? OnboardStatus { get; set; }
        public double? Salary { get; set; }
        public int? ProbationPercentage { get; set; }
        public string SkillStr { get; set; }
        public string LevelName { get; set; }
        public string PositionName { get; set; }
        public string BranchName { get; set; }
        public string StatusName => OnboardStatus.HasValue ? Enum.GetName(typeof(TalentOnboardStatus), OnboardStatus) : "";
        public UserType? UserType { get; set; }
        public long? BranchId { get; set; }
        public long? JobPositionId { get; set; }
        public long? LevelId { get; set; }
        public BadgeInfoDto JobPositionInfo { get; set; }
        public BadgeInfoDto BranchInfo { get; set; }
        public BadgeInfoDto LevelInfo { get; set; }
        public BadgeInfoDto UserTypeInfo { get; set; }
        public string UpdatedUser { get; set; }
        public DateTime? UpdatedTime { get; set; }
        public DateTime? CreationTime { get; set; }
    }
}
