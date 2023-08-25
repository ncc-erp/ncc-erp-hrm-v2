using Abp.Domain.Entities;
using Abp.UI;
using DocumentFormat.OpenXml.Office2010.Excel;
using HRMv2.Core.Tests;
using HRMv2.Entities;
using HRMv2.Manager.Categories;
using HRMv2.Manager.Categories.JobPositions;
using HRMv2.Manager.Categories.JobPositions.Dto;
using HRMv2.Manager.Categories.PunishmentTypes;
using HRMv2.Manager.Employees;
using HRMv2.Manager.Salaries.CalculateSalary.Dto;
using HRMv2.Manager.Salaries.Dto;
using HRMv2.Manager.Salaries.Payslips.Dto;
using HRMv2.Manager.Salaries.SalaryCalculators;
using HRMv2.Manager.Salaries.SalaryCalculators.Dto;
using HRMv2.NccCore;
using HRMv2.WebServices.Timesheet.Dto;
using Microsoft.VisualStudio.TestPlatform.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting.Logging;
using NccCore.Paging;
using NccCore.Uitls;
using Newtonsoft.Json;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using static HRMv2.Constants.Enum.HRMEnum;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace HRMv2.Application.Tests.APIs.JobPositionManagerTest
{
    public class JobPosition_Tests : HRMv2CoreTestBase
    {
        private readonly JobPositionManager _JobPosition;
        private readonly IWorkScope _work;

        public JobPosition_Tests()
        {
            _JobPosition = Resolve<JobPositionManager>();
            _work = Resolve<IWorkScope>();
        }

        [Fact]
        public async Task GetAllTest()
        {

            await WithUnitOfWorkAsync(() =>
            {
                var JobPositions = _JobPosition.GetAll();
                Assert.Equal(15, JobPositions.Count);
                JobPositions.ShouldContain(JobPosition => JobPosition.Name == "HR");
                JobPositions.ShouldContain(JobPosition => JobPosition.ShortName == "HR");
                JobPositions.ShouldContain(JobPosition => JobPosition.Code == "hr");
                return Task.CompletedTask;
            });

        }

        [Fact]
        //get all paging with no begin, no take
        //if no MaxResultCount then take = 10
        public async Task GetAllPagingTest1()
        {
            var inputTest = new GridParam();

            await WithUnitOfWorkAsync(() =>
            {
                var JobPositions = _JobPosition.GetAllPaging(inputTest);

                Assert.Equal(10, JobPositions.Result.Items.Count);
                Assert.Equal(15, JobPositions.Result.TotalCount);
                Assert.NotEqual(JobPositions.Result.TotalCount, JobPositions.Result.Items.Count);
                Assert.Throws<ArgumentOutOfRangeException>(() => JobPositions.Result.Items[10].ShortName);

                JobPositions.Result.Items.ShouldContain(JobPosition => JobPosition.Name == "QA");
                JobPositions.Result.Items.ShouldContain(JobPosition => JobPosition.ShortName == "QA");
                JobPositions.Result.Items.ShouldContain(JobPosition => JobPosition.Code == "qa");
                JobPositions.Result.Items.ShouldNotContain(JobPosition => JobPosition.Name == "Finance");
                return Task.CompletedTask;
            });
        }

        [Fact]
        //get all paging with begin = 3, no take
        public async Task GetAllPagingTest2()
        {
            var skipCount = 3;
            var inputTest = new GridParam
            {
                SkipCount = skipCount,
            };

            await WithUnitOfWorkAsync(() =>
            {
                var JobPositions = _JobPosition.GetAllPaging(inputTest);

                Assert.Equal(10, JobPositions.Result.Items.Count);
                Assert.Equal(12, JobPositions.Result.TotalCount - skipCount);
                Assert.Equal("Art", JobPositions.Result.Items[0].Name);

                JobPositions.Result.Items.ShouldContain(JobPosition => JobPosition.Name == "Art");
                JobPositions.Result.Items.ShouldContain(JobPosition => JobPosition.ShortName == "Art");
                JobPositions.Result.Items.ShouldContain(JobPosition => JobPosition.Code == "art");
                JobPositions.Result.Items.ShouldNotContain(JobPosition => JobPosition.Name == "PM");
                return Task.CompletedTask;
            });
        }

        [Fact]
        //get all paging with begin > 15(max = 15), no take
        public async Task GetAllPagingTest3()
        {
            var skipCount = 16;
            var inputTest = new GridParam
            {
                SkipCount = skipCount,
            };

            await WithUnitOfWorkAsync(() =>
            {
                var JobPositions = _JobPosition.GetAllPaging(inputTest);

                Assert.Equal(0, JobPositions.Result.Items.Count);
                return Task.CompletedTask;
            });
        }

        [Fact]
        //get all paging with take = 3
        public async Task GetAllPagingTest4()
        {
            var takeCount = 3;
            var inputTest = new GridParam
            {
                MaxResultCount = takeCount,
            };

            await WithUnitOfWorkAsync(() =>
            {
                var JobPositions = _JobPosition.GetAllPaging(inputTest);

                Assert.Equal(takeCount, JobPositions.Result.Items.Count);
                JobPositions.Result.Items.ShouldContain(JobPosition => JobPosition.Name == "Dev");
                JobPositions.Result.Items.ShouldContain(JobPosition => JobPosition.ShortName == "Dev");
                JobPositions.Result.Items.ShouldContain(JobPosition => JobPosition.Code == "dev");
                JobPositions.Result.Items.ShouldNotContain(JobPosition => JobPosition.Name == "IT");
                return Task.CompletedTask;
            });
        }

        [Fact]
        //get all paging with begin = 2, take = 3
        public async Task GetAllPagingTest5()
        {
            var skipCount = 2;
            var takeCount = 3;
            var inputTest = new GridParam
            {
                MaxResultCount = takeCount,
                SkipCount = skipCount,
            };

            await WithUnitOfWorkAsync(() =>
            {
                var JobPositions = _JobPosition.GetAllPaging(inputTest);

                Assert.Equal(takeCount, JobPositions.Result.Items.Count);
                Assert.Matches(JobPositions.Result.Items[0].ShortName, "PM");
                JobPositions.Result.Items.ShouldContain(JobPosition => JobPosition.Name == "PM");
                JobPositions.Result.Items.ShouldContain(JobPosition => JobPosition.ShortName == "PM");
                JobPositions.Result.Items.ShouldContain(JobPosition => JobPosition.Code == "pm");
                JobPositions.Result.Items.ShouldNotContain(JobPosition => JobPosition.Name == "Tester");
                JobPositions.Result.Items.ShouldNotContain(JobPosition => JobPosition.Name == "IT");
                return Task.CompletedTask;
            });
        }

        [Fact]
        //delete JobPosition 
        public async Task DeleteTest1()
        {
            long idDel = 53;

            await WithUnitOfWorkAsync(() =>
            {
                var DelJobPosition = _JobPosition.Delete(idDel);
                Assert.Equal(idDel.ToString(), DelJobPosition.Result.ToString());

                return Task.CompletedTask;
            });

            WithUnitOfWork(() =>
            {
                var AllJobPosition = _work.GetAll<JobPosition>().Count();

                AllJobPosition.ShouldBe(14);
            });
        }

        [Fact]
        //delete JobPosition with exception not found ID
        public async Task DeleteTest2()
        {
            long idDel = 100;

            await WithUnitOfWorkAsync(async () =>
            {
                var exception = await Assert.ThrowsAsync<EntityNotFoundException>(() => _JobPosition.Delete(idDel));
            });
        }

        [Fact]
        //delete JobPosition with exception Had user in this Job Position
        public async Task DeleteTest3()
        {
            long idDel = 50;

            await WithUnitOfWorkAsync(async () =>
            {
                var exception = await Assert.ThrowsAsync<UserFriendlyException>(() => _JobPosition.Delete(idDel));
                Assert.Equal($"Job Position Id {idDel} has user", exception.Message);
            });
        }

        [Fact]
        //create JobPosition 
        public async Task CreateJobPositionTest1()
        {
            JobPositionDto JP = new()
            {
                Name = "Job Position 1000",
                ShortName = "JP1000",
                Code = "998877",
                Color = "lime",
                NameInContract = "JJPP1000"
            };

            await WithUnitOfWorkAsync(() =>
            {
                var newJobPosition = _JobPosition.Create(JP);
                var getNewJobPosition = _work.GetAsync<JobPosition>(7);
                var getAllJobPositions = _JobPosition.GetAll();

                Assert.NotNull(newJobPosition);
                Assert.Equal("JP1000", newJobPosition.Result.ShortName);
                newJobPosition.Result.Id.ShouldBeGreaterThan(getAllJobPositions.Last().Id);

                return Task.FromResult(Task.CompletedTask);
            });

            await WithUnitOfWorkAsync(async () =>
            {
                var allJobPosition = _work.GetAll<JobPosition>();
                var JobPosition = await _work.GetAsync<JobPosition>(62);

                allJobPosition.Count().ShouldBe(16);
                JobPosition.Name.ShouldBe(JP.Name);
                JobPosition.ShortName.ShouldBe(JP.ShortName);
                JobPosition.Code.ShouldBe(JP.Code);
                JobPosition.Color.ShouldBe(JP.Color);
                JobPosition.NameInContract.ShouldBe(JP.NameInContract);
                allJobPosition.Where(x => x.Id == 62).ShouldNotBeNull();
            });
        }

        [Fact]
        //create JobPosition with exception Name existed
        public async Task CreateJobPositionTest2()
        {
            await WithUnitOfWorkAsync(async () =>
            {
                JobPositionDto JP = new()
                {
                    Name = "QA",
                    ShortName = "QA",
                    Code = "998877",
                    Color = "lime",
                    NameInContract = "JJPP1000"
                };

                await WithUnitOfWorkAsync(async () =>
                {

                    var exception = await Assert.ThrowsAsync<UserFriendlyException>(async () =>
                    {
                        await _JobPosition.Create(JP);
                    });
                    Assert.Equal($"Name or Code is Already Exist", exception.Message);

                });

                return Task.FromResult(Task.CompletedTask);
            });
        }

        [Fact]
        //create JobPosition with exception Code existed
        public async Task CreateJobPositionTest3()
        {
            await WithUnitOfWorkAsync(async () =>
            {
                JobPositionDto JP = new()
                {
                    Name = "Job Position 8",
                    ShortName = "JP8",
                    Code = "pm",
                    Color = "black",
                    NameInContract = "JJPP1000"
                };

                await WithUnitOfWorkAsync(async () =>
                {
                    var exception = await Assert.ThrowsAsync<UserFriendlyException>(async () =>
                    {
                        await _JobPosition.Create(JP);
                    });
                    Assert.Equal($"Name or Code is Already Exist", exception.Message);

                });

                return Task.FromResult(Task.CompletedTask);
            });
        }

        [Fact]
        //update JobPosition
        public async Task UpdateJobPositionTest1()
        {
            JobPositionDto JP = new()
            {
                Id = 54,
                Name = "Job Position 10",
                ShortName = "JP10",
                Code = "7777",
                Color = "gray",
                NameInContract = "JJPP101"
            };

            await WithUnitOfWorkAsync(() =>
            {
                var updateJobPosition = _JobPosition.Update(JP);

                Assert.NotNull(updateJobPosition);

                return Task.FromResult(Task.CompletedTask);
            });

            await WithUnitOfWorkAsync(async () =>
            {
                var updateJobPosition = await _work.GetAsync<JobPosition>(54);

                updateJobPosition.Name.ShouldBe(JP.Name);
                updateJobPosition.ShortName.ShouldBe(JP.ShortName);
                updateJobPosition.Code.ShouldBe(JP.Code);
                updateJobPosition.Color.ShouldBe(JP.Color);
                updateJobPosition.NameInContract.ShouldBe(JP.NameInContract);
            });
        }

        [Fact]
        //update JobPosition with exception Code existed
        public async Task UpdateJobPositionTest2()
        {
            JobPositionDto JP = new()
            {
                Id = 52,
                Name = "Job Position 10",
                ShortName = "JP10",
                Code = "Brse",
                Color = "gray",
                NameInContract = "JJPP101"
            };

            await WithUnitOfWorkAsync(async () =>
            {

                var exception = await Assert.ThrowsAsync<UserFriendlyException>(async () =>
                {
                    await _JobPosition.Update(JP);
                });
                Assert.Equal($"Name or Code is Already Exist", exception.Message);

            });
        }

        [Fact]
        //update JobPosition with exception Name existed
        public async Task UpdateJobPositionTest3()
        {
            await WithUnitOfWorkAsync(async () =>
            {
                JobPositionDto JP = new()
                {
                    Id = 54,
                    Name = "PM",
                    ShortName = "JP10",
                    Code = "77784",
                    Color = "gray",
                    NameInContract = "JJPP101"
                };

                await WithUnitOfWorkAsync(async () =>
                {

                    var exception = await Assert.ThrowsAsync<UserFriendlyException>(async () =>
                    {
                        await _JobPosition.Update(JP);
                    });
                    Assert.Equal($"Name or Code is Already Exist", exception.Message);

                });
            });
        }

    }
}
