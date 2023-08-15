using Castle.Core.Internal;
using HRMv2.Entities;
using HRMv2.Manager.Debts.PaymentPlansManager;
using HRMv2.Manager.Debts.PaymentPlansManager.Dto;
using HRMv2.NccCore;
using HRMv2.Utils;
using NccCore.Paging;
using NccCore.Uitls;
using Shouldly;
using Xunit;
using static HRMv2.Constants.Enum.HRMEnum;

namespace HRMv2.Core.Tests.Managers.Debts.DebtPaymentPlans
{
    public class PaymentPlanManager_Tests : HRMv2CoreTestBase
    {
        private readonly PaymentPlanManager _paymentPlan;
        private readonly IWorkScope _workScope;
        public PaymentPlanManager_Tests()
        {
            _paymentPlan = Resolve<PaymentPlanManager>();
            _workScope = Resolve<IWorkScope>();
        }

        [Fact]
        public async Task Should_Get_All_PaymentPlan()
        {
            await WithUnitOfWorkAsync(async () =>
            {
                var result = _paymentPlan.GetAll();

                result.ShouldNotBeNull();
                result.Count.ShouldBeGreaterThanOrEqualTo(20);
                result.ShouldContain(paymentPlan => paymentPlan.Id == 266);
                result.ShouldContain(paymentPlan => paymentPlan.DebtId == 89);
                result.ShouldContain(paymentPlan => paymentPlan.Money == 2500000);
                result.ShouldContain(paymentPlan => paymentPlan.Date == new DateTime(2023, 3, 5));
            });
        }

        [Fact]
        public async Task Should_Get_A_PaymentPlan()
        {
            var expectPaymentPlan = new PaymentPlanDto
            {
                Id = 266,
                DebtId = 89,
                Date = new DateTime(2022, 12, 5),
                Money = 2500000,
                PaymentType = DebtPaymentType.Salary,
                Note = "Trả lần 1",
            };
            await WithUnitOfWorkAsync(async () =>
            {
                var result = _paymentPlan.Get(266);

                result.Id.ShouldBe(expectPaymentPlan.Id);
                result.DebtId.ShouldBe(expectPaymentPlan.DebtId);
                result.Date.ShouldBe(expectPaymentPlan.Date);
                result.Money.ShouldBe(expectPaymentPlan.Money);
                result.Note.ShouldBe(expectPaymentPlan.Note);
                result.PaymentType.ShouldBe(expectPaymentPlan.PaymentType);
            });
        }

        [Fact]
        public async Task Should_Get_PaymentPlan_By_DebtId()
        {
            await WithUnitOfWorkAsync(async () =>
            {
                var result = _paymentPlan.GetPaymentPlansByDebId(89);

                result.Count.ShouldBeGreaterThanOrEqualTo(2);
                result.ShouldContain(paymentPlan => paymentPlan.Id == 266);
                result.ShouldContain(paymentPlan => paymentPlan.Id == 267);
                result.ShouldContain(paymentPlan => paymentPlan.DebtId == 89);
                result.ShouldContain(paymentPlan => paymentPlan.Money == 2500000);
            });
        }

        [Fact]
        public async Task Should_Get_All_PaymentPlan_Paging()
        {
            var expectTotalCount = 20;
            var expectItemsCount = 10;

            await WithUnitOfWorkAsync(async () =>
            {
                var result = await _paymentPlan.GetAllPaging(new GridParam
                {
                    MaxResultCount = 10,
                    SkipCount = 2,
                });

                result.TotalCount.ShouldBe(expectTotalCount);
                result.Items.Count.ShouldBe(expectItemsCount);
                result.Items.ShouldContain(paymentPlan => paymentPlan.Id == 268);
                result.Items.ShouldContain(paymentPlan => paymentPlan.Money == 3000000);
                result.Items.ShouldContain(paymentPlan => paymentPlan.Date == new DateTime(2022, 11, 5));
                result.Items.ShouldNotContain(paymentPlan => paymentPlan.Id == 267);
                result.Items.ShouldNotContain(paymentPlan => paymentPlan.Money == 2500000);
            });
        }

        [Fact]
        public async Task Should_Generate_Plan_Without_Having_Diff_Amount()
        {
            var expectPaymentPlan = new GeneratePlanDto
            {
                DebtId = 1,
                Money = 120000,
                StartDate = new DateTime(2022, 1, 10),
                EndDate = new DateTime(2022, 12, 10),
            };

            await WithUnitOfWorkAsync(async () =>
            {
                var result = await _paymentPlan.GeneratePlan(new GeneratePlanDto
                {
                    DebtId = 1,
                    Money = 120000,
                    StartDate = new DateTime(2022, 1, 10),
                    EndDate = new DateTime(2022, 12, 10),
                });

                result.DebtId.ShouldBe(expectPaymentPlan.DebtId);
                result.Money.ShouldBe(expectPaymentPlan.Money);
                DateTimeUtils.ToStringStandardDateTime(result.StartDate).ShouldBe("10/01/2022 00:00");
                DateTimeUtils.ToStringStandardDateTime(result.EndDate).ShouldBe("10/12/2022 00:00");
            });

            await WithUnitOfWorkAsync(async () =>
            {
                var result = _paymentPlan.GetPaymentPlansByDebId(1);

                // DateTimeUtils has been tested in DateTimeUtilTest file 
                List<DateTime> listMonth = DateTimeUtils.GetListMonthForDebtPaymentPlan(expectPaymentPlan.StartDate, expectPaymentPlan.EndDate);

                double money = expectPaymentPlan.Money / listMonth.Count;

                double roundMoney = CommonUtil.RoundMoneyVND(money);

                foreach (var debtPaymentPlan in result.Select((value, index) => (value, index)))
                {
                    debtPaymentPlan.value.DebtId.ShouldBe(expectPaymentPlan.DebtId);
                    debtPaymentPlan.value.Money.ShouldBe(roundMoney);
                    debtPaymentPlan.value.Note.ShouldBe($"Trả lần {debtPaymentPlan.index + 1}");
                    debtPaymentPlan.value.PaymentType.ShouldBe(DebtPaymentType.Salary);
                }
            });
        }

        [Fact]
        public async Task Should_Generate_Plan_With_Having_Diff_Amount()
        {
            var expectPaymentPlan = new GeneratePlanDto
            {
                DebtId = 2,
                Money = 200000,
                StartDate = new DateTime(2022, 1, 10),
                EndDate = new DateTime(2022, 12, 10),
            };

            await WithUnitOfWorkAsync(async () =>
            {
                var result = await _paymentPlan.GeneratePlan(new GeneratePlanDto
                {
                    DebtId = 2,
                    Money = 200000,
                    StartDate = new DateTime(2022, 1, 10),
                    EndDate = new DateTime(2022, 12, 10),
                });

                result.DebtId.ShouldBe(expectPaymentPlan.DebtId);
                result.Money.ShouldBe(expectPaymentPlan.Money);
                DateTimeUtils.ToStringStandardDateTime(result.StartDate).ShouldBe("10/01/2022 00:00");
                DateTimeUtils.ToStringStandardDateTime(result.EndDate).ShouldBe("10/12/2022 00:00");
            });

            await WithUnitOfWorkAsync(async () =>
            {
                var result = _paymentPlan.GetPaymentPlansByDebId(2);

                // DateTimeUtils has been tested in DateTimeUtilTest file 
                List<DateTime> listMonth = DateTimeUtils.GetListMonthForDebtPaymentPlan(expectPaymentPlan.StartDate, expectPaymentPlan.EndDate);

                double money = expectPaymentPlan.Money / listMonth.Count;

                double roundMoney = CommonUtil.RoundMoneyVND(money);

                var diffAmount = expectPaymentPlan.Money - roundMoney * listMonth.Count;

                var lastMoney = roundMoney + diffAmount;

                result[listMonth.Count - 1].Money.ShouldBe(lastMoney);

                result.RemoveAt(result.Count - 1);

                foreach (var debtPaymentPlan in result.Select((value, index) => (value, index)))
                {
                    debtPaymentPlan.value.DebtId.ShouldBe(expectPaymentPlan.DebtId);
                    debtPaymentPlan.value.Money.ShouldBe(roundMoney);
                    debtPaymentPlan.value.Note.ShouldBe($"Trả lần {debtPaymentPlan.index + 1}");
                    debtPaymentPlan.value.PaymentType.ShouldBe(DebtPaymentType.Salary);
                }
            });
        }

        [Fact]
        public async Task Should_Create_A_Payment_Plan()
        {
            var expectPaymentPlan = new DebtPaymentPlan
            {
                Id = 289,
                DebtId = 1,
                Date = new DateTime(2022, 12, 19),
                Money = 100000,
                PaymentType = DebtPaymentType.Salary,
                Note = "Trả bù",
                CreatorUserId = 53,
                LastModifierUserId = 53,
                IsDeleted = false,
            };
            await WithUnitOfWorkAsync(async () =>
            {
                var result = await _paymentPlan.Create(new CreatePaymentPlanDto
                {
                    DebtId = 1,
                    Date = new DateTime(2022, 12, 19),
                    Money = 100000,
                    PaymentType = DebtPaymentType.Salary,
                    Note = "Trả bù",
                });

                var allPaymentPlan = _workScope.GetAll<DebtPaymentPlan>();

                allPaymentPlan.Count().ShouldBe(20);
                result.DebtId.ShouldBe(expectPaymentPlan.DebtId);
                result.Money.ShouldBe(expectPaymentPlan.Money);
                result.Date.ShouldBe(expectPaymentPlan.Date);
                result.PaymentType.ShouldBe(expectPaymentPlan.PaymentType);
                result.Note.ShouldBe(expectPaymentPlan.Note);
            });

            await WithUnitOfWorkAsync(async () =>
            {
                var allPaymentPlan = _workScope.GetAll<DebtPaymentPlan>();
                var paymentPlan = await _workScope.GetAsync<DebtPaymentPlan>(expectPaymentPlan.Id);

                allPaymentPlan.Count().ShouldBe(21);
                allPaymentPlan.ToArray().Find(paymentPlan => paymentPlan.Id == expectPaymentPlan.Id).ShouldNotBeNull();

                paymentPlan.Id.ShouldBe(expectPaymentPlan.Id);
                paymentPlan.DebtId.ShouldBe(expectPaymentPlan.DebtId);
                paymentPlan.Money.ShouldBe(expectPaymentPlan.Money);
                paymentPlan.Note.ShouldBe(expectPaymentPlan.Note);
                paymentPlan.Date.ShouldBe(expectPaymentPlan.Date);
                paymentPlan.PaymentType.ShouldBe(expectPaymentPlan.PaymentType);
            });
        }

        [Fact]
        public async Task Should_Update_A_Payment_Plan()
        {
            var expectPaymentPlan = new DebtPaymentPlan
            {
                Id = 266,
                DebtId = 1,
                Date = new DateTime(2022, 12, 19),
                Money = 1000000,
                PaymentType = DebtPaymentType.Salary,
                Note = "Trả lần 1 update",
                CreatorUserId = 53,
                LastModifierUserId = 53,
                IsDeleted = false,
            };
            await WithUnitOfWorkAsync(async () =>
            {
                var result = await _paymentPlan.Update(new UpdatePaymentPlanDto
                {
                    Id = 266,
                    DebtId = 1,
                    Date = new DateTime(2022, 12, 19),
                    Money = 1000000,
                    PaymentType = DebtPaymentType.Salary,
                    Note = "Trả lần 1 update",
                });

                result.Id.ShouldBe(expectPaymentPlan.Id);
                result.DebtId.ShouldBe(expectPaymentPlan.DebtId);
                result.Money.ShouldBe(expectPaymentPlan.Money);
                result.Note.ShouldBe(expectPaymentPlan.Note);
                result.PaymentType.ShouldBe(expectPaymentPlan.PaymentType);
            });

            await WithUnitOfWorkAsync(async () =>
            {
                var paymentPlan = await _workScope.GetAsync<DebtPaymentPlan>(expectPaymentPlan.Id);

                paymentPlan.Id.ShouldBe(expectPaymentPlan.Id);
                paymentPlan.DebtId.ShouldBe(expectPaymentPlan.DebtId);
                paymentPlan.Date.ShouldBe(expectPaymentPlan.Date);
                paymentPlan.Note.ShouldBe(expectPaymentPlan.Note);
                paymentPlan.CreatorUser.ShouldBe(expectPaymentPlan.CreatorUser);
                paymentPlan.IsDeleted.ShouldBe(expectPaymentPlan.IsDeleted);
                paymentPlan.PaymentType.ShouldBe(expectPaymentPlan.PaymentType);
            });
        }

        [Fact]
        public async Task Should_Delete_A_Payment_Plan()
        {
            await WithUnitOfWorkAsync(async () =>
            {
                var result = await _paymentPlan.Delete(266);

                var allPaymentPlan = _workScope.GetAll<DebtPaymentPlan>();

                result.ShouldBe(266);
                allPaymentPlan.Count().ShouldBe(20);
            });

            await WithUnitOfWorkAsync(async () =>
            {
                var allPaymentPlan = _workScope.GetAll<DebtPaymentPlan>();

                allPaymentPlan.Count().ShouldBe(19);

                allPaymentPlan.ToArray().Find(paymentPlan => paymentPlan.Id == 266).ShouldBeNull();
            });
        }
    }
}
