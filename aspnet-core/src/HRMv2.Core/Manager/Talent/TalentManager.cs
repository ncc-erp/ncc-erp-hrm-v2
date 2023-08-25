using Abp.UI;
using HRMv2.Entities;
using HRMv2.Manager.Talent.Dto;
using HRMv2.NccCore;
using HRMv2.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HRMv2.Constants.Enum.HRMEnum;

namespace HRMv2.Manager.Talent
{
    public class TalentManager : BaseManager
    {

        public TalentManager(IWorkScope workScop) : base(workScop)
        {
        }

        public void CreateTempEmployeeFromTalent(InputCreateTempEmployeeTalentDto input)
        {

            var levelId = WorkScope.GetAll<Level>()
            .Where(x => x.Code == input.FinalLevel)
            .Select(x => x.Id)
            .FirstOrDefault();

            var positionId = WorkScope.GetAll<JobPosition>()
                .Where(x => x.Name.ToLower().Trim() == input.PositionName.ToLower().Trim()
                || x.Code.ToLower().Trim() == input.PositionName.ToLower().Trim())
                .Select(x => x.Id)
                .FirstOrDefault();

            var branchId = WorkScope.GetAll<Branch>()
                .Where(x => x.Name.ToLower().Trim() == input.BranchName.ToLower().Trim()
                || x.Code.ToLower().Trim() == input.BranchName.ToLower().Trim())
                .Select(x => x.Id)
                .FirstOrDefault();

            var existTempEmployees = WorkScope.GetAll<TempEmployeeTalent>()
                .Where(x => x.PersonalEmail.ToLower().Trim() == input.PersonalEmail.ToLower().Trim())
                .FirstOrDefault();

            if (existTempEmployees != default && existTempEmployees.Status == TalentOnboardStatus.Onboarded)
            {
                throw new UserFriendlyException($"Employee {existTempEmployees.PersonalEmail} already Onboard!");
            }

            var userType = CommonUtil.MaptalentUsertype(input.UserType) == UserType.Staff
                ? UserType.ProbationaryStaff : CommonUtil.MaptalentUsertype(input.UserType);

            var tempEmployee = new MapTempEmployeeTalentDto
            {
                LevelId = levelId != default ? levelId : null,
                FullName = input.FullName,
                Gender = input.IsFemale ? Sex.Female : Sex.Male,
                PersonalEmail = input.PersonalEmail,
                NCCEmail = input.NCCEmail,
                Phone = input.Phone,
                Status = input.Status,
                Salary = input.Salary,
                OnboardDate = input.OnboardDate,
                UserType = userType,
                BranchId = branchId != default ? branchId : null,
                DateOfBirth = input.DateOfBirth.HasValue ? input.DateOfBirth : null,
                JobPositionId = positionId != default ? positionId : null,
                Skills = input.SkillStr,
            };

            if (existTempEmployees == default)
            {
                var entity = ObjectMapper.Map<TempEmployeeTalent>(tempEmployee);

                WorkScope.InsertAsync(entity);
            }
            else
            {
                existTempEmployees = ObjectMapper.Map(tempEmployee, existTempEmployees);

                WorkScope.UpdateAsync(existTempEmployees);
            }
        }
    }
}
