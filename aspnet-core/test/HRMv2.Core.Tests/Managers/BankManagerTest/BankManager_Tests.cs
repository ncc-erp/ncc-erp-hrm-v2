using Abp.Domain.Entities;
using Abp.UI;
using DocumentFormat.OpenXml.Office2010.Excel;
using HRMv2.Core.Tests;
using HRMv2.Entities;
using HRMv2.Manager.Categories;
using HRMv2.Manager.Categories.Banks;
using HRMv2.Manager.Categories.Banks.Dto;
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

namespace HRMv2.Application.Tests.APIs.BankManagerTest
{
    public class BankManager_Tests : HRMv2CoreTestBase
    {
        private readonly BankManager _Bank;
        private readonly IWorkScope _work;

        public BankManager_Tests()
        {
            _Bank = Resolve<BankManager>();
            _work = Resolve<IWorkScope>();
        }

        [Fact]
        public async Task GetAllTest()
        {
            await WithUnitOfWorkAsync(() =>
            {
                var Banks = _Bank.GetAll();
                Assert.Equal(8, Banks.Count);

                Banks.ShouldContain(Bank => Bank.Name == "Techcombank");
                Banks.ShouldContain(Bank => Bank.Code == "Techcombank");
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
                var Banks = _Bank.GetAllPaging(inputTest);

                Assert.Equal(8, Banks.Result.Items.Count);
                Assert.Equal(8, Banks.Result.TotalCount);

                Banks.Result.Items.ShouldContain(Bank => Bank.Name == "Techcombank");
                Banks.Result.Items.ShouldContain(Bank => Bank.Code == "Techcombank");
                return Task.CompletedTask;
            });
        }

        [Fact]
        //get all paging with begin = 2, no take
        public async Task GetAllPagingTest2()
        {
            var skipCount = 2;
            var inputTest = new GridParam
            {
                SkipCount = skipCount,
            };

            await WithUnitOfWorkAsync(() =>
            {
                var Banks = _Bank.GetAllPaging(inputTest);

                Assert.Equal(6, Banks.Result.Items.Count);
                Assert.Equal(6, Banks.Result.TotalCount - skipCount);
                Assert.Equal("Agribank", Banks.Result.Items[0].Name);

                Banks.Result.Items.ShouldContain(Bank => Bank.Name == "BIDV");
                Banks.Result.Items.ShouldContain(Bank => Bank.Code == "BIDV");
                Banks.Result.Items.ShouldNotContain(Bank => Bank.Code == "ACB");
                return Task.CompletedTask;
            });
        }

        [Fact]
        //get all paging with begin > 8(max = 8), no take
        public async Task GetAllPagingTest3()
        {
            var skipCount = 8;
            var inputTest = new GridParam
            {
                SkipCount = skipCount,
            };

            await WithUnitOfWorkAsync(() =>
            {
                var Banks = _Bank.GetAllPaging(inputTest);

                Banks.Result.Items.Count.ShouldBe(0);

                return Task.FromResult(Task.CompletedTask);
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
                var Banks = _Bank.GetAllPaging(inputTest);

                Assert.Equal(takeCount, Banks.Result.Items.Count);

                Banks.Result.Items.ShouldContain(Bank => Bank.Name == "Agribank");
                Banks.Result.Items.ShouldContain(Bank => Bank.Code == "Agribank");
                Banks.Result.Items.ShouldNotContain(Bank => Bank.Name == "BIDV");
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
                var Banks = _Bank.GetAllPaging(inputTest);

                Assert.Equal(takeCount, Banks.Result.Items.Count);
                Assert.Matches(Banks.Result.Items[0].Name, "Agribank");

                Banks.Result.Items.ShouldContain(Bank => Bank.Name == "DongA Bank");
                Banks.Result.Items.ShouldContain(Bank => Bank.Code == "DongA Bank");
                Banks.Result.Items.ShouldNotContain(Bank => Bank.Name == "ACB");
                Banks.Result.Items.ShouldNotContain(Bank => Bank.Name == "MBB");
                return Task.CompletedTask;
            });
        }

        [Fact]
        //delete Bank 
        public async Task DeleteTest1()
        {
            long idDel = 34;

            await WithUnitOfWorkAsync(() =>
            {
                var DelBank = _Bank.Delete(idDel);
                Assert.Equal(idDel.ToString(), DelBank.Result.ToString());

                return Task.CompletedTask;
            });

            WithUnitOfWork(() =>
            {
                var AllBanks = _work.GetAll<Bank>().Count();

                AllBanks.ShouldBe(7);
            });

        }

        [Fact]
        //delete Bank with exception not found ID
        public async Task DeleteTest2()
        {
            long idDel = 100;

            await WithUnitOfWorkAsync(async () =>
            {
                await Assert.ThrowsAsync<EntityNotFoundException>(() => _Bank.Delete(idDel));
            });
        }

        [Fact]
        //delete Bank with exception Had user in this Bank
        public async Task DeleteTest3()
        {
            long idDel = 32;

            await WithUnitOfWorkAsync(async () =>
            {
                var exception = await Assert.ThrowsAsync<UserFriendlyException>(() => _Bank.Delete(idDel));
                Assert.Equal($"Bank Id {idDel} has user", exception.Message);
            });
        }

        [Fact]
        //create Bank 
        public async Task CreateTest1()
        {
            BankDto Banks = new()
            {
                Name = "XamXiBank",
                Code = "XamXiBank",
            };

            await WithUnitOfWorkAsync(() =>
            {
                var newBank = _Bank.Create(Banks);

                Assert.NotNull(newBank);

                return Task.FromResult(Task.CompletedTask);
            });

            await WithUnitOfWorkAsync(async () =>
            {
                var allBank = _work.GetAll<Bank>();
                var Bank = await _work.GetAsync<Bank>(40);

                allBank.Count().ShouldBe(9);
                Bank.Name.ShouldBe(Banks.Name);
                Bank.Code.ShouldBe(Banks.Code);
                allBank.Where(x => x.Id == 40).ShouldNotBeNull();
            });
        }

        [Fact]
        //create Bank with exception Name existed
        public async Task CreateTest2()
        {
            BankDto Banks = new()
            {
                Name = "ACB",
                Code = "XamXiBank",
            };

            await WithUnitOfWorkAsync(async () =>
            {

                var exception = await Assert.ThrowsAsync<UserFriendlyException>(async () =>
                {
                    await _Bank.Create(Banks);
                });
                Assert.Equal($"Name or Code is Already Exist", exception.Message);

                return Task.FromResult(Task.CompletedTask);
            });
        
        }

        [Fact]
        //create Bank with exception Code existed
        public async Task CreateTest3()
        {
            await WithUnitOfWorkAsync(async () =>
            {
                BankDto Banks = new()
                {
                    Name = "XamXiBank",
                    Code = "ACB",
                };
                var exception = await Assert.ThrowsAsync<UserFriendlyException>(async () =>
                {
                    await _Bank.Create(Banks);
                });
                Assert.Equal($"Name or Code is Already Exist", exception.Message);

                return Task.FromResult(Task.CompletedTask);
            });
        }

        [Fact]
        //update Bank
        public async Task UpdateTest1()
        {
            BankDto Banks = new()
            {
                Id = 32,
                Name = "SanSiBank",
                Code = "SanSiBank",
            };

            await WithUnitOfWorkAsync(() =>
            {
                var updateBank = _Bank.Update(Banks);

                Assert.NotNull(updateBank);

                return Task.FromResult(Task.CompletedTask);
            });

            await WithUnitOfWorkAsync(async () =>
            {
                var updateBank = await _work.GetAsync<Bank>(32);

                updateBank.Name.ShouldBe(Banks.Name);
                updateBank.Code.ShouldBe(Banks.Code);
            });
        }

        [Fact]
        //update Bank with exception Code existed
        public async Task UpdateTest2()
        {
            await WithUnitOfWorkAsync(async () =>
            {
                BankDto Banks = new()
                {
                    Id = 32,
                    Name = "SanSiBank",
                    Code = "ACB",
                };

                await WithUnitOfWorkAsync(async () =>
                {

                    var exception = await Assert.ThrowsAsync<UserFriendlyException>(async () =>
                    {
                        await _Bank.Update(Banks);
                    });
                    Assert.Equal($"Name or Code is Already Exist", exception.Message);

                });
            });
        }

        [Fact]
        //update Bank with exception Name existed
        public async Task UpdateTest3()
        {
            await WithUnitOfWorkAsync(async () =>
            {
                BankDto Banks = new()
                {
                    Id = 32,
                    Name = "ACB",
                    Code = "SanSiBank",
                };

                await WithUnitOfWorkAsync(async () =>
                {

                    var exception = await Assert.ThrowsAsync<UserFriendlyException>(async () =>
                    {
                        await _Bank.Update(Banks);
                    });
                    Assert.Equal($"Name or Code is Already Exist", exception.Message);

                });
            });
        }

    }
}
