using HRMv2.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HRMv2.Constants.Enum.HRMEnum;

namespace HRMv2.Manager.Talent.Dto
{
    public class InputCreateTempEmployeeTalentDto
    {
        public TalentOnboardStatus Status { get; set; }
        public string FinalLevel { get; set; }
        public DateTime? OnboardDate { get; set; }
        public double Salary { get; set; }
        public string SkillStr { get; set; }
        public string NCCEmail { get; set; }
        public string BranchName { get; set; }
        public UserType UserType { get; set; }
        public string PersonalEmail { get; set; }
        public string Phone { get; set; }
        public string FullName { get; set; }
        public bool IsFemale { get; set; }
        public string PositionName { get; set; }
        public DateTime? DateOfBirth { get; set; }
    }
}
