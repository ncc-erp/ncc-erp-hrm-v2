using Abp.AutoMapper;
using HRMv2.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HRMv2.Constants.Enum.HRMEnum;

namespace HRMv2.Manager.Talent.Dto
{
    [AutoMapTo(typeof(TempEmployeeTalent))]
    public class MapTempEmployeeTalentDto
    {
        public long? LevelId { get; set; }
        public string FullName { get; set; }
        public Sex? Gender { get; set; }
        public string PersonalEmail { get; set; }
        public string NCCEmail { get; set; }
        public string Phone { get; set; }
        public TalentOnboardStatus? Status { get; set; }
        public double? Salary { get; set; }
        public DateTime? OnboardDate { get; set; }
        public UserType? UserType { get; set; }
        public long? BranchId { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public long? JobPositionId { get; set; }
        public string Skills { get; set; }
        public int ProbationPercentage => UserType.HasValue && UserType == Constants.Enum.HRMEnum.UserType.ProbationaryStaff ? 85 : 100;

    }
}
