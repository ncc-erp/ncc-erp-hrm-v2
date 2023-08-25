using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HRMv2.Entities;
using HRMv2.Manager.Debts.PaidsManagger.Dto;
using HRMv2.NccCore;
using NccCore.Paging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abp.Domain.Entities;
using Abp.UI;
using HRMv2.Manager.Debts.PaidsManagger;
using Shouldly;
using Xunit;
using DocumentFormat.OpenXml.Bibliography;
using Abp.ObjectMapping;
using HRMv2.Manager.Payrolls;
using HRMv2.Manager.Salaries.Dto;
using Microsoft.AspNetCore.Http;
using System.Dynamic;
namespace HRMv2.Core.Tests.Managers.Debts.DebtPaids
{
    public class PaidManager_Tests : HRMv2CoreTestBase
    {
        private readonly PaidManager _paidManager;
        private readonly IWorkScope _workScope;
        public PaidManager_Tests()
        {
            _workScope = Resolve<IWorkScope>();
            _paidManager = new PaidManager(_workScope);
            _paidManager.ObjectMapper = LocalIocManager.Resolve<IObjectMapper>();
        }
        [Fact]
        public async Task GetAllPaging_Test1()
        {
            var expectedResult = new DebtPaidDto
            {
                Id = 574,
                DebtId = 89,
                PaymentType = Constants.Enum.HRMEnum.DebtPaymentType.Salary,
                UserSalaryId = 247484,
                Date = new DateTime(2022, 12, 01),
                Note = "Trừ khoản vay #89 vào lương tháng 12-2022",
                Money = 2500000.0,
                IsAllowEdit = false,
                CreatorUser = "admin",
                UpdatedUser = "admin"
            };
            var gridParam = new GridParam
            {
                SkipCount = 1,
            };
            await WithUnitOfWorkAsync(async () =>
            {
                var results = await _paidManager.GetAllPaging(gridParam);
                var result = results.Items[0];
                Assert.NotNull(result);
                Assert.Equal(expectedResult.Date, result.Date);
                Assert.Equal(expectedResult.Note, result.Note);
                Assert.Equal(expectedResult.UserSalaryId, result.UserSalaryId);
                Assert.Equal(expectedResult.DebtId, result.DebtId);
                Assert.Equal(expectedResult.Money, result.Money);
                Assert.Equal(10, results.Items.Count);
            });
        }
        [Fact]
        public async Task GetAllPaging_Test2()
        {
            var expectedResult = new DebtPaidDto
            {
                Id = 573,
                DebtId = 88,
                PaymentType = Constants.Enum.HRMEnum.DebtPaymentType.Salary,
                UserSalaryId = 247479,
                Date = new DateTime(2022, 12, 01),
                Note = "Trừ khoản vay #88 vào lương tháng 12-2022",
                Money = 2693333.0,
                IsAllowEdit = false,
                CreatorUser = "admin",
                UpdatedUser = "admin"
            };
            var gridParam = new GridParam
            {
                MaxResultCount = 6,
            };
            await WithUnitOfWorkAsync(async () =>
            {
                var results = await _paidManager.GetAllPaging(gridParam);
                Assert.Equal(12, results.TotalCount);
                var result = results.Items[0];
                Assert.NotNull(result);
                Assert.Equal(expectedResult.Date, result.Date);
                Assert.Equal(expectedResult.Note, result.Note);
                Assert.Equal(expectedResult.UserSalaryId, result.UserSalaryId);
                Assert.Equal(expectedResult.DebtId, result.DebtId);
                Assert.Equal(expectedResult.Money, result.Money);
            });
        }
        [Fact]
        public async Task GetAllPagging_Test3()
        {
            // paids.Result.TotoalCount >= skipCount + defaultMaxResultCount
            var expectedResult = new DebtPaidDto
            {
                Id = 580,
                DebtId = 94,
                PaymentType = Constants.Enum.HRMEnum.DebtPaymentType.Salary,
                UserSalaryId = 247837,
                Date = new DateTime(2023, 01, 01),
                Note = "Trừ khoản vay #94 vào lương tháng 01-2023",
                Money = 5000993.0,
                IsAllowEdit = false,
                CreatorUser = "admin",
                UpdatedUser = "admin"
            };
            var skipCount = 2;
            var defaultMaxResultCount = 10;
            var inputTest = new GridParam
            {
                SkipCount = skipCount,
            };
            await WithUnitOfWorkAsync(async () => {
                var paids = await _paidManager.GetAllPaging(inputTest);
                Assert.Equal(defaultMaxResultCount, paids.Items.Count);
                var result = paids.Items[0];
                Assert.NotNull(result);
                Assert.Equal(expectedResult.Date, result.Date);
                Assert.Equal(expectedResult.Note, result.Note);
                Assert.Equal(expectedResult.UserSalaryId, result.UserSalaryId);
                Assert.Equal(expectedResult.DebtId, result.DebtId);
                Assert.Equal(expectedResult.Money, result.Money);
                return Task.CompletedTask;
            });
        }
        [Fact]
        public async Task GetAllPagging_Test4()
        {
            // paids.Result.TotoalCount == skipCount
            var skipCount = 69;
            var inputTest = new GridParam
            {
                SkipCount = skipCount,
            };
            await WithUnitOfWorkAsync(() => {
                var paids = _paidManager.GetAllPaging(inputTest);
                Assert.Equal(0, paids.Result.Items.Count);
                return Task.CompletedTask;
            });
        }
        [Fact]
        public async Task GetAllPagging_Test5()
        {
            var expectedResult = new DebtPaidDto
            {
                Id = 580,
                DebtId = 94,
                PaymentType = Constants.Enum.HRMEnum.DebtPaymentType.Salary,
                UserSalaryId = 247837,
                Date = new DateTime(2023, 01, 01),
                Note = "Trừ khoản vay #94 vào lương tháng 01-2023",
                Money = 5000993.0,
                IsAllowEdit = false,
                CreatorUser = "admin",
                UpdatedUser = "admin"
            };
            // paids.Result.TotalCount >= maxResultCount + skipCount
            var maxResultCount = 3;
            var skipCount = 2;
            var inputTest = new GridParam
            {
                MaxResultCount = maxResultCount,
                SkipCount = skipCount,
            };
            await WithUnitOfWorkAsync(async () =>
            {
                var paids = await _paidManager.GetAllPaging(inputTest);
                Assert.Equal(maxResultCount, paids.Items.Count);
                var result = paids.Items[0];
                Assert.NotNull(result);
                Assert.Equal(expectedResult.Date, result.Date);
                Assert.Equal(expectedResult.Note, result.Note);
                Assert.Equal(expectedResult.UserSalaryId, result.UserSalaryId);
                Assert.Equal(expectedResult.DebtId, result.DebtId);
                Assert.Equal(expectedResult.Money, result.Money);
                return Task.CompletedTask;
            });
        }
        [Fact]
        public async Task GetAllPagging_Test6()
        {
            // paids.Result.TotalCount < maxResultCount && paids.Result.TotalCount > skipCount
            var expectedResult = new DebtPaidDto
            {
                Id = 580,
                DebtId = 94,
                PaymentType = Constants.Enum.HRMEnum.DebtPaymentType.Salary,
                UserSalaryId = 247837,
                Date = new DateTime(2023, 01, 01),
                Note = "Trừ khoản vay #94 vào lương tháng 01-2023",
                Money = 5000993.0,
                IsAllowEdit = false,
                CreatorUser = "admin",
                UpdatedUser = "admin"
            };
            var maxResultCount = 96;
            var skipCount = 2;
            var inputTest = new GridParam
            {
                MaxResultCount = maxResultCount,
                SkipCount = skipCount,
            };
            await WithUnitOfWorkAsync(async () =>
            {
                var paids = await _paidManager.GetAllPaging(inputTest);
                Assert.Equal(paids.TotalCount - skipCount, paids.Items.Count);
                var result = paids.Items[0];
                Assert.NotNull(result);
                Assert.Equal(expectedResult.Date, result.Date);
                Assert.Equal(expectedResult.Note, result.Note);
                Assert.Equal(expectedResult.UserSalaryId, result.UserSalaryId);
                Assert.Equal(expectedResult.DebtId, result.DebtId);
                Assert.Equal(expectedResult.Money, result.Money);
                return Task.CompletedTask;
            });
        }
        [Fact]
        public void GetAll_Test1()
        {
            var expectedResult = new DebtPaidDto
            {
                Id = 573,
                DebtId = 88,
                PaymentType = Constants.Enum.HRMEnum.DebtPaymentType.Salary,
                UserSalaryId = 247479,
                Date = new DateTime(2022, 12, 01),
                Note = "Trừ khoản vay #88 vào lương tháng 12-2022",
                Money = 2693333.0,
                IsAllowEdit = false,
                CreatorUser = "admin",
                UpdatedUser = "admin"
            };
            WithUnitOfWork(() =>
            {
                var results = _paidManager.GetAll();
                var result = results.First();
                Assert.NotNull(result);
                Assert.Equal(expectedResult.Date, result.Date);
                Assert.Equal(expectedResult.Note, result.Note);
                Assert.Equal(expectedResult.UserSalaryId, result.UserSalaryId);
                Assert.Equal(expectedResult.DebtId, result.DebtId);
                Assert.Equal(expectedResult.Money, result.Money);
                Assert.Equal(12, results.Count);
            });
        }
        [Fact]
        public void Get_Test1()
        {
            // Standard test case
            var expectedPaid = new DebtPaid
            {
                Id = 573,
                DebtId = 88,
                Money = 2693333.0,
                Date = new DateTime(2022, 12, 01),
                PaymentType = Constants.Enum.HRMEnum.DebtPaymentType.Salary,
                PayslipDetailId = 247479,
                CreatorUser = null,
                LastModifierUser = null,
                Note = "Trừ khoản vay #88 vào lương tháng 12-2022",
                CreationTime = new DateTime(2022, 12, 1),
                LastModificationTime = new DateTime(2022, 12, 1)
            };
            WithUnitOfWork(() =>
            {
                var result = _paidManager.Get(expectedPaid.Id);
                Assert.NotNull(result);
                Assert.Equal(expectedPaid.Date, result.Date);
                Assert.Equal(expectedPaid.Note, result.Note);
                Assert.Equal(expectedPaid.DebtId, result.DebtId);
                Assert.Equal(expectedPaid.Money, result.Money);
            });
        }
        [Fact]
        public void Get_Test2()
        {
            // Standard test case
            var notExistPaidID = 69420;
            WithUnitOfWork(() =>
            {
                var result = _paidManager.Get(notExistPaidID);
                Assert.Null(result);
            });
        }
        [Fact]
        public void GetPaidsByDebtId_Test1()
        {
            // Standard test case

            var debtId = 89;
            WithUnitOfWork(() =>
            {
                var results = _paidManager.GetPaidsByDebtId(debtId);
                Assert.NotNull(results);
                Assert.Equal(574, results.First().Id);
                Assert.Equal(582, results.Last().Id);
                Assert.Equal(2, results.Count);
            });
        }
        [Fact]
        public void GetPaidsByDebtId_Test2()
        {
            // Standard test case
            var notExistDebtID = 69420;
            var expectedResult = new List<DebtPaidDto>();
            WithUnitOfWork(() =>
            {
                var result = _paidManager.GetPaidsByDebtId(notExistDebtID);
                Assert.Equal(result, expectedResult);
            });
        }
        [Fact]
        public async Task Create_Test1()
        {
            // Standard test case
            var newPaid = new CreatePaidDto
            {
                DebtId = 590,
                Date = new DateTime(2022, 12, 15),
                UserSalaryId = 1,
                PaymentType = Constants.Enum.HRMEnum.DebtPaymentType.Salary,
                Money = 19001009.0,
                Note = "Trừ khoản vay #98 vào lương tháng 01-2024"
            };

            var expectedId = 590;

            await WithUnitOfWorkAsync(async () =>
            {
                var result = await _paidManager.Create(newPaid);
                var allDebtPaids = _workScope.GetAll<DebtPaid>();
                Assert.NotNull(result);
                Assert.Equal(newPaid.Note, result.Note);
                //result.DebtId.ShouldBeGreaterThan(allDebtPaids.Last().Id);
            });

            await WithUnitOfWorkAsync(async () =>
            {
                var allDebtPaids = _workScope.GetAll<DebtPaid>();
                var debtPaid = await _workScope.GetAsync<DebtPaid>(expectedId);
                allDebtPaids.Count().ShouldBe(13);
                debtPaid.Date.ShouldBe(newPaid.Date);
                debtPaid.Note.ShouldBe(newPaid.Note);
                debtPaid.PaymentType.ShouldBe(newPaid.PaymentType);
                debtPaid.Money.ShouldBe(newPaid.Money);
                allDebtPaids.Where(x => x.Id == 589).ShouldNotBeNull();
            });
        }

        [Fact]
        public async Task Update_Test1()
        {
            // Standard test case
            var debtPaid = new UpdatePaidDto
            {
                Id = 583,
                DebtId = 93,
                Note = "Trừ khoản vay #93 vào lương tháng 02-2023"
            };
            var updateId = 583;
            await WithUnitOfWorkAsync(async () =>
            {
                var result = await _paidManager.Update(debtPaid);
                Assert.Null(result);
            });
            await WithUnitOfWorkAsync(async () =>
            {
                var updatePaid = await _workScope.GetAsync<DebtPaid>(updateId);
                updatePaid.Date.ShouldBe(debtPaid.Date);
                updatePaid.Note.ShouldBe(debtPaid.Note);
                updatePaid.PaymentType.ShouldBe(debtPaid.PaymentType);
                updatePaid.Money.ShouldBe(debtPaid.Money);
            });
        }

        [Fact]
        public async Task Delete_Test1()
        {
            // Standard test case
            var paidId = 587;
            await WithUnitOfWorkAsync(async () =>
            {
                var result = await _paidManager.Delete(paidId);
                Assert.Equal(paidId, result);
            });
            WithUnitOfWork(() =>
            {
                var allPaids = _workScope.GetAll<DebtPaid>();
                allPaids.Count().ShouldBe(11);
            });
        }
    }
}
