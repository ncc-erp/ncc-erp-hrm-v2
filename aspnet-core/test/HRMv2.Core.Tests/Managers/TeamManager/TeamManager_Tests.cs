using Abp.Domain.Entities;
using Abp.ObjectMapping;
using Abp.UI;
using DocumentFormat.OpenXml.Office2010.Excel;
using HRMv2.Entities;
using HRMv2.Manager.Categories.Teams;
using HRMv2.Manager.Salaries.Dto;
using HRMv2.NccCore;
using NccCore.Paging;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using SortDirection = NccCore.Paging.SortDirection;

namespace HRMv2.Core.Tests.Managers.TeamManagerTest
{
    public class TeamManager_Tests : HRMv2CoreTestBase
    {
        private readonly TeamManager _teamManager;
        private readonly IWorkScope _work;
        public TeamManager_Tests()
        {
            _work = Resolve<IWorkScope>();
            _teamManager = new TeamManager(_work);
            _teamManager.ObjectMapper = Resolve<IObjectMapper>();
        }

        //Test Function GetAll
        [Fact]
        public async Task Should_Get_All()
        {
            await WithUnitOfWorkAsync(async () =>
            {
                var expectedResult = 9;
                var result = _teamManager.GetAll();
                Assert.Equal(expectedResult,result.Count());
                result.ShouldContain(x => x.Id == 39);
                result.ShouldContain(x => x.Name == "Sipdo");
                result.ShouldContain(x => x.Id == 45);
                result.ShouldContain(x => x.Name == "Supporter");
            });
        }

        //Test Function GetAllPaging
        [Fact]
        public async Task Should_Get_All_Paging_With_GridParam_No_Value()
        {
            await WithUnitOfWorkAsync(async () =>
            {
                var inputGridParam = new GridParam { };
                var expectedResult = 9;
                var result = await _teamManager.GetAllPaging(inputGridParam);
                Assert.Equal(expectedResult, result.TotalCount);
                result.Items.ShouldContain(x => x.Id == 39);
                result.Items.ShouldContain(x => x.Name == "Sipdo");
                result.Items.ShouldContain(x => x.Id == 45);
                result.Items.ShouldContain(x => x.Name == "Supporter");
            });
        }

        [Fact]
        public async Task Should_Get_All_Paging_With_GridParam_Using_Sort()
        {
            await WithUnitOfWorkAsync(async () =>
            {
                var inputGridParam = new GridParam
                {
                    Sort = "Name"
                };
                var expectedTotal = 9;
                var expectedSortFirst = "BackOffice";
                var expectedSortLast = "Trainer";
                var result = await _teamManager.GetAllPaging(inputGridParam);
                Assert.Equal(expectedTotal, result.TotalCount);
                Assert.Equal(expectedSortFirst, result.Items.First().Name);
                Assert.Equal(expectedSortLast, result.Items.Last().Name);
                result.Items.ShouldContain(x => x.Id == 39);
                result.Items.ShouldContain(x => x.Name == "Sipdo");
                result.Items.ShouldContain(x => x.Id == 45);
                result.Items.ShouldContain(x => x.Name == "Supporter");
            });
        }

        [Fact]
        public async Task Should_Get_All_Paging_With_GridParam_Using_Sort_And_SortDirection()
        {
            await WithUnitOfWorkAsync(async () =>
            {
                var inputGridParam = new GridParam
                {
                    Sort = "Name",
                    SortDirection = SortDirection.DESC,
                };
                var expectedTotal = 9;
                var expectedSortFirst = "Trainer";
                var expectedSortLast = "BackOffice";
                var result = await _teamManager.GetAllPaging(inputGridParam);
                Assert.Equal(expectedTotal, result.TotalCount);
                Assert.Equal(expectedSortFirst, result.Items.First().Name);
                Assert.Equal(expectedSortLast, result.Items.Last().Name);
                result.Items.ShouldContain(x => x.Id == 39);
                result.Items.ShouldContain(x => x.Name == "Sipdo");
                result.Items.ShouldContain(x => x.Id == 45);
                result.Items.ShouldContain(x => x.Name == "Supporter");
            });
        }

        [Fact]
        public async Task Should_Get_All_Paging_With_GridParam_Using_Search_Text()
        {
            await WithUnitOfWorkAsync(async () =>
            {
                var inputGridParam = new GridParam
                {
                    SearchText = "Sipdo",
                };
                var expectedTotal = 1;
                var result = await _teamManager.GetAllPaging(inputGridParam);
                Assert.Equal(expectedTotal, result.TotalCount);
                Assert.Equal(39, result.Items.First().Id);
                Assert.Equal("Sipdo", result.Items.First().Name);
            });
        }

        [Fact]
        public async Task Should_Get_All_Paging_With_GridParam_Using_Max_Result_Count()
        {
            await WithUnitOfWorkAsync(async () =>
            {
                var inputGridParam = new GridParam
                {
                    MaxResultCount = 5
                };
                var expectedTotal = 5;
                var result = await _teamManager.GetAllPaging(inputGridParam);
                Assert.Equal(expectedTotal, result.Items.Count());
                Assert.Equal(39, result.Items.First().Id);
                Assert.Equal("Sipdo", result.Items.First().Name);
                result.Items.ShouldNotContain(x => x.Id == 44);
                result.Items.ShouldNotContain(x => x.Name == "Trainer");
            });
        }

        [Fact]
        public async Task Should_Get_All_Paging_With_GridParam_Using_Skip_Count()
        {
            await WithUnitOfWorkAsync(async () =>
            {
                var inputGridParam = new GridParam
                {
                    SkipCount = 5
                };
                var expectedTotal = 4;
                var result = await _teamManager.GetAllPaging(inputGridParam);
                Assert.Equal(expectedTotal, result.Items.Count());
                Assert.Equal(44, result.Items.First().Id);
                Assert.Equal("Trainer", result.Items.First().Name);
                result.Items.ShouldNotContain(x => x.Id == 43);
                result.Items.ShouldNotContain(x => x.Name == "BackOffice");
            });
        }

        //Test Function Create
        [Fact]
        public async Task Should_Not_Allow_Create_Team_With_Name_Already_Exist()
        {
            var input = new TeamDto
            {
                Id = 100,
                Name = "Sipdo",
            };
            await WithUnitOfWorkAsync(async () =>
            {
                var expectedMsg = $"Name is Already Exist";
                var exception = await Assert.ThrowsAsync<UserFriendlyException>(async () =>
                {
                    await _teamManager.Create(input);
                });
                Assert.Equal(expectedMsg, exception.Message);
            });

        }

        [Fact]
        public async Task Should_Allow_Create_Team_With_Input_Valid()
        {
            var input = new TeamDto
            {
                Id = 100,
                Name = "New Team",
            };
            await WithUnitOfWorkAsync(async () =>
            {
                var result = await _teamManager.Create(input);
                Assert.Equal(input.Id, result.Id);
                Assert.Equal(input.Name, result.Name);
            });

            await WithUnitOfWorkAsync(async () =>
            {
                var teams = _work.GetAll<Team>();
                var team = await _work.GetAsync<Team>(100);
                Assert.Equal(10, teams.Count());
                Assert.Equal(input.Id, team.Id);
                Assert.Equal(input.Name, team.Name);
            });
        }

        //Test Function Update
        // code chings thiếu case Id not exist and name not exist
        [Fact]
        public async Task Should_Allow_Update_With_Team_Valid()

        {
            var input = new TeamDto
            {
                Id = 39,
                Name = "New Name Team",
            };
            await WithUnitOfWorkAsync(async () =>
            {
                var team = await _teamManager.Update(input);
                Assert.Equal(input.Name, team.Name);
            });
            await WithUnitOfWorkAsync(async () =>
            {
                var team = await _work.GetAsync<Team>(39);
                Assert.Equal(input.Name, team.Name);
            });
        }

        [Fact]
        public async Task Should_Not_Allow_Update_With_Id_Not_Exist_And_Name_Already_Axist()
        {
            var input = new TeamDto
            {
                Id = 100,
                Name = "Sipdo",
            };
            await WithUnitOfWorkAsync(async () =>
            {
                var expectedMsg = $"Name is Already Exist";
                var exception = await Assert.ThrowsAsync<UserFriendlyException>(async () =>
                {
                    await _teamManager.Update(input);
                });
                Assert.Equal(expectedMsg, exception.Message);
            });
        }

        //Test Function Delete
        //Chưa test đc Exception EntityNotFoundException

        [Fact]
        public async Task Should_Not_Allow_Delete_With_Team_Not_Exist()
        {
            var input = 100;
            await WithUnitOfWorkAsync(async () =>
            {
                var exception = await Assert.ThrowsAsync<EntityNotFoundException>(async () =>
                {
                    await _teamManager.Delete(input);
                });
            });
        }

        [Fact]
        public async Task Should_Not_Allow_Delete_With_Team_Already_Has_PayslipTeam()
        {
            var input = 46;
            await WithUnitOfWorkAsync(async () =>
            { 
                var expectedMsg = $"This team already has payslip";
                var exception = await Assert.ThrowsAsync<UserFriendlyException>(async () =>
                {
                    await _teamManager.Delete(input);
                });
                Assert.Equal(expectedMsg, exception.Message);
            });
        }

        [Fact]
        public async Task Should_Not_Allow_Delete_With_Team_Already_Has_Employee()
        {
            var input = 39;
            await WithUnitOfWorkAsync(async () =>
            {
                var expectedMsg = $"This team already has employee";
                var exception = await Assert.ThrowsAsync<UserFriendlyException>(async () =>
                {
                    await _teamManager.Delete(input);
                });
                Assert.Equal(expectedMsg, exception.Message);
            });
        }

        [Fact]
        public async Task Should_Allow_Delete_With_Team_Valid()
        {
            var input = 47;
            await WithUnitOfWorkAsync(async () =>
            {            
                var id = await _teamManager.Delete(input);
                Assert.Equal(input, id);
            });

            await WithUnitOfWorkAsync(async () =>
            {
                var teams = _work.GetAll<Team>();
                Assert.Equal(8, teams.Count());
            });
        }
    }
}
