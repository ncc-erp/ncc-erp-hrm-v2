using Abp.ObjectMapping;
using Abp.UI;
using HRMv2.Entities;
using HRMv2.Manager.Salaries.Dto;
using HRMv2.Manager.Talent;
using HRMv2.Manager.Talent.Dto;
using HRMv2.NccCore;
using Microsoft.AspNetCore.Components.Forms;
using NSubstitute;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using static HRMv2.Constants.Enum.HRMEnum;
namespace HRMv2.Core.Tests.Managers.Talent
{
    public class TalentManager_Tests : HRMv2CoreTestBase
    {
        private readonly TalentManager _talentManager;
        private readonly IWorkScope _workScope;
        public TalentManager_Tests()
        {
            _workScope = Resolve<IWorkScope>();
            _talentManager = new TalentManager(_workScope);
            _talentManager.ObjectMapper = LocalIocManager.Resolve<IObjectMapper>();
        }
        [Fact]
        public async Task CreateTempEmployeeFromTalent_Test1()
        {
            // Standard test case
            var newTalent = new InputCreateTempEmployeeTalentDto
            {
                FinalLevel = "313",
                FullName = "Nguyen Van Vai",
                IsFemale = true,
                PersonalEmail = "vanvaiii@gam.cccom",
                NCCEmail = "vai.nguyenvan@ncc.comcc",
                Phone = "0351665412",
                Status = TalentOnboardStatus.AcceptedOffer,
                Salary = 50000000,
                OnboardDate = DateTime.Now,
                UserType = UserType.Internship,
                BranchName = "HN2",
                DateOfBirth = new DateTime(2001, 11, 15),
                PositionName = "Marketing",
                SkillStr = "e o co gi",
            };
            var expectedId = 6;
            var expectedTotal = 6;
            await WithUnitOfWorkAsync(async () =>
            {
                Task waitIt = Task.Run(() => _talentManager.CreateTempEmployeeFromTalent(newTalent));
                var res = await Task.WhenAny(waitIt);
                var allTalentEmployees = _workScope.GetAll<TempEmployeeTalent>();
                Assert.NotNull(allTalentEmployees);
                
            });
            await WithUnitOfWorkAsync(async () =>
            {
                var allTalentEmployees = _workScope.GetAll<TempEmployeeTalent>();
                var talentEmployee = await _workScope.GetAsync<TempEmployeeTalent>(expectedId);
                allTalentEmployees.Count().ShouldBe(expectedTotal);
                talentEmployee.Phone.ShouldBe(newTalent.Phone);
                talentEmployee.FullName.ShouldBe(newTalent.FullName);
                talentEmployee.NCCEmail.ShouldBe(newTalent.NCCEmail);
                talentEmployee.PersonalEmail.ShouldBe(newTalent.PersonalEmail);
                talentEmployee.DateOfBirth.ShouldBe(newTalent.DateOfBirth);
                talentEmployee.OnboardDate.ShouldBe(newTalent.OnboardDate);
                talentEmployee.Salary.ShouldBe(newTalent.Salary);
                allTalentEmployees.Where(x => x.Id == expectedId).ShouldNotBeNull();
            });
        }
        [Fact]
        public async Task CreateTempEmployeeFromTalent_Update_IF_Exist_PersonalEmail_Test2()
        {
            // Standard test case
            var updatedTalent = new InputCreateTempEmployeeTalentDto
            {
                FinalLevel = "312",
                FullName = "Nguyen Van Banh",
                IsFemale = false,
                PersonalEmail = "vanbanh@gam.cccom",
                NCCEmail = "Banh.nguyenvan@ncc.comcc",
                Phone = "0353365412",
                Status = TalentOnboardStatus.AcceptedOffer,
                Salary = 60000000,
                OnboardDate = DateTime.Now,
                UserType = UserType.Internship,
                BranchName = "HN2",
                DateOfBirth = new DateTime(2001, 11, 15),
                PositionName = "UI",
                SkillStr = "e o co gi",
            };
            var existId = 2;
            var expectedTotal = 5;
            await WithUnitOfWorkAsync(async () =>
            {
                Task waitIt = Task.Run(() => _talentManager.CreateTempEmployeeFromTalent(updatedTalent));
                var res = await Task.WhenAny(waitIt);
                var allTalentEmployees = _workScope.GetAll<TempEmployeeTalent>();
                Assert.NotNull(allTalentEmployees);
                allTalentEmployees.Count().ShouldBe(expectedTotal);
            });
            await WithUnitOfWorkAsync(async () =>
            {
                var allTalentEmployees = _workScope.GetAll<TempEmployeeTalent>();
                var talentEmployee = await _workScope.GetAsync<TempEmployeeTalent>(existId);
                talentEmployee.Phone.ShouldBe(updatedTalent.Phone);
                talentEmployee.NCCEmail.ShouldBe(updatedTalent.NCCEmail);
                talentEmployee.FullName.ShouldBe(updatedTalent.FullName);
                talentEmployee.DateOfBirth.ShouldBe(updatedTalent.DateOfBirth);
                talentEmployee.OnboardDate.ShouldBe(updatedTalent.OnboardDate);
                talentEmployee.Salary.ShouldBe(updatedTalent.Salary);
                allTalentEmployees.Where(x => x.Id == existId).ShouldNotBeNull();
            });
        }

        [Fact]
        public async Task CreateTempEmployeeFromTalent_Should_Not_Create_Onboard_Talent()
        {
            // Standard test case
            var newTalent = new InputCreateTempEmployeeTalentDto
            {
                FinalLevel = "1",
                FullName = "Nguyen Van Kha",
                IsFemale = true,
                PersonalEmail = "vankha@gam.cccom",
                NCCEmail = "kha.nguyenvan@ncc.comcc",
                Phone = "0353154122",
                Status = TalentOnboardStatus.Onboarded,
                Salary = 60020000,
                OnboardDate = new DateTime(2022, 12, 27), // DateTime.Now,
                UserType = UserType.Internship,
                BranchName = "HN2",
                DateOfBirth = new DateTime(2001, 11, 15),
                PositionName = "Dev",
                SkillStr = "e o co gi",
            };

            await WithUnitOfWorkAsync(() =>
            {

                var exception = Assert.ThrowsAsync<UserFriendlyException>(() =>
                {
                    _talentManager.CreateTempEmployeeFromTalent(newTalent);
                    return Task.CompletedTask;
            });
                Assert.Equal($"Employee {newTalent.PersonalEmail} already Onboard!", exception.Result.Message);


                return Task.FromResult(Task.CompletedTask);
            });
        }
    }
}