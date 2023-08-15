using Abp.Application.Features;
using Abp.BackgroundJobs;
using Abp.Configuration;
using Abp.Dependency;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.MultiTenancy;
using Abp.Net.Mail;
using Abp.Runtime.Session;
using Abp.Threading.Timers;
using HRMv2.Editions;
using HRMv2.Entities;
using HRMv2.Manager.Categories.Benefits;
using HRMv2.Manager.Categories.JobPositions;
using HRMv2.Manager.Categories.Levels;
using HRMv2.Manager.Categories.UserTypes;
using HRMv2.Manager.Categories;
using HRMv2.Manager.ChangeEmployeeWorkingStatuses;
using HRMv2.Manager.Debts;
using HRMv2.Manager.Debts.PaidsManagger;
using HRMv2.Manager.Debts.PaymentPlansManager;
using HRMv2.Manager.EmployeeContracts;
using HRMv2.Manager.Employees;
using HRMv2.Manager.Histories;
using HRMv2.Manager.Notifications.Email;
using HRMv2.Manager.Payrolls;
using HRMv2.Manager.SalaryRequests;
using HRMv2.Manager.Timesheet;
using HRMv2.MultiTenancy;
using HRMv2.NccCore;
using HRMv2.UploadFileServices;
using HRMv2.WebServices.Finfast;
using HRMv2.WebServices.IMS;
using HRMv2.WebServices.Komu;
using HRMv2.WebServices.Project;
using HRMv2.WebServices.Talent;
using HRMv2.WebServices.Timesheet;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using NSubstitute;
using Xunit;
using Abp.Application.Editions;
using HRMv2.Manager.Payrolls.Dto;
using Shouldly;
using HRMv2.Manager.Salaries.Payslips;
using HRMv2.Hubs;
using Abp.UI;
using static HRMv2.Constants.Enum.HRMEnum;
using Abp.Domain.Entities;
using Abp.ObjectMapping;
using NccCore.Paging;
using Moq;

namespace HRMv2.Core.Tests.Managers.Salaries.Payrolls
{
    public class PayrollManager_Tests : HRMv2CoreTestBase
    {
        private readonly PayrollManager _payrollManager;

        private readonly IWorkScope _workScope;
        private readonly TimesheetWebService _timesheetWebService;
        private readonly TimesheetManager _timesheetManager;
        private readonly FinfastWebService _finfastWebService;
        private readonly PaidManager _paidManager;
        private readonly PaymentPlanManager _paymentPlanManager;
        private readonly EmployeeManager _employeeManager;
        private readonly DebtManager _debtManager;
        private readonly KomuService _komuService;
        private readonly EmailManager _emailManager;
        private readonly CalculateSalaryHub _calculateSalaryHub;
        private readonly PayslipManager _payslipManager;

        public PayrollManager_Tests()
        {
            _workScope = Resolve<IWorkScope>();

            var _httpClient = Resolve<HttpClient>();
            var _iAbpSession = Resolve<IAbpSession>();
            var _iocResovler = Resolve<IIocResolver>();

            //var timesheetServiceMock = new Mock<TimesheetWebService>(_httpClient, _iAbpSession, _iocResovler);
            //timesheetServiceMock
            //    .Setup(p => p.GetSettingOffDates(It.IsAny<int>(), It.IsAny<int>()))
            //    .Returns(new HashSet<DateTime> { new DateTime(2022, 12, 12) });
            //_timesheetWebService = timesheetServiceMock.Object;

            var timesheetManagerMock = new Mock<TimesheetManager>(_timesheetWebService, _workScope);
            _timesheetManager = timesheetManagerMock.Object;

            _finfastWebService = Substitute.For<FinfastWebService>(_httpClient, _iAbpSession, _iocResovler);

            _paidManager = Substitute.For<PaidManager>(_workScope);
            _paymentPlanManager = Substitute.For<PaymentPlanManager>(_workScope);

            var configOptions = new Dictionary<string, string>
            {
                {"KomuService:ChannelIdDevMode", ""},
                {"KomuService:EnableKomuNotification", "true"}
            };
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(configOptions)
                .Build();

            /* == 1. UPLOAD FILE SERVICE == */
            // 1.1. EditionManager
            var _edition = Resolve<IRepository<Edition>>();
            var _featureValueStore = Resolve<IAbpZeroFeatureValueStore>();
            var _unitOfWorkManager = Resolve<IUnitOfWorkManager>();

            var _editionManager = Substitute.For<EditionManager>(_edition, _featureValueStore, _unitOfWorkManager); ;

            // 1.2. TenantManager
            var _tenant = Resolve<IRepository<Tenant>>();
            var _tenantFeatureSetting = Resolve<IRepository<TenantFeatureSetting, long>>();

            var _tenantManager = Substitute.For<TenantManager>(_tenant, _tenantFeatureSetting, _editionManager, _featureValueStore);

            // 1.3. UploadFileService
            var _uploadFile = Substitute.For<IUploadFileService>();

            var _uploadFileService = Substitute.For<UploadFileService>(_uploadFile, _tenantManager, _iAbpSession);

            /* == 2. CONTRACT MANAGER == */
            // 2.1. EmailManager
            var _emailSender = Resolve<IEmailSender>();
            var _timesheetConfig = Resolve<IOptions<TimesheetConfig>>();

            _emailManager = new EmailManager(_workScope, _emailSender, _timesheetConfig);

            // 2.2. ContractManager
            var _contractManager = Substitute.For<ContractManager>(_workScope, _uploadFileService, _emailManager);

            /* == 3. HISTORY MANAGER */
            // 3.1. BranchManager
            var moqBranchManager = Substitute.For<BranchManager>(_workScope);

            // 3.2. LevelManager
            var moqLevelManager = Substitute.For<LevelManager>(_workScope);

            // 3.3. ChangeEmployeeWorkingStatusManager 
            var _benefitManager = Substitute.For<BenefitManager>(_workScope);
            var _IBackgroundJobManager = Resolve<IBackgroundJobManager>();
            var _employee = Resolve<IRepository<Employee, long>>();
            var _employeeWorkingHistory = Resolve<IRepository<EmployeeWorkingHistory, long>>();
            var _salaryChangeRequestEmployee = Resolve<IRepository<SalaryChangeRequestEmployee, long>>();
            var _benefitEmployee = Resolve<IRepository<BenefitEmployee, long>>();
            var _backgroundJobInfo = Resolve<IRepository<BackgroundJobInfo, long>>();
            var _employeeContract = Resolve<IRepository<EmployeeContract, long>>();
            var _projectService = Substitute.For<ProjectService>(_httpClient, _iAbpSession, _iocResovler);
            var _IMSWebService = Substitute.For<IMSWebService>(_httpClient, _iAbpSession, _iocResovler);
            var _talentWebService = Substitute.For<TalentWebService>(_httpClient, _iAbpSession, _iocResovler);
            _komuService = Substitute.For<KomuService>(_httpClient, _iAbpSession, configuration, _iocResovler);
            var _settingManager = Resolve<ISettingManager>();

            var _changeEmployeeWorkingStatusManager
                = Substitute.For<ChangeEmployeeWorkingStatusManager>(_workScope, _benefitManager, _contractManager,
                _IBackgroundJobManager, _employee, _employeeWorkingHistory, _salaryChangeRequestEmployee,
                _benefitEmployee, _backgroundJobInfo, _employeeContract, _projectService, _timesheetWebService,
                _IMSWebService, _talentWebService, _komuService, _settingManager);

            // 3.4. HistoryManager
            var _historyManager = Substitute.For<HistoryManager>(_workScope, moqBranchManager, moqLevelManager, _changeEmployeeWorkingStatusManager);

            /* == 4. SALARY REQUEST MANAGER */
            var _jobPositionManager = Substitute.For<JobPositionManager>(_workScope);

            var _IBackgroundJobStore = Resolve<IBackgroundJobStore>();
            var _AbpAsyncTimer = Resolve<AbpAsyncTimer>();
            var _backgroundJobManager = Substitute.For<BackgroundJobManager>(_iocResovler, _IBackgroundJobStore, _AbpAsyncTimer);

            var _salaryRequestManager = Substitute.For<SalaryRequestManager>(moqLevelManager, _jobPositionManager, _contractManager, _workScope, _emailManager, _backgroundJobManager);

            /* == 5. USER TYPE MANAGER */
            var moqUserTypeManager = Substitute.For<UserTypeManager>(_workScope);

            /* == 6. EMPLOYEE MANAGER */
            _employeeManager = Substitute.For<EmployeeManager>(_uploadFileService, _contractManager, _historyManager, _projectService, _timesheetWebService, _IMSWebService, _talentWebService, _workScope, _salaryRequestManager, _benefitManager, moqUserTypeManager, _changeEmployeeWorkingStatusManager, _backgroundJobManager, _backgroundJobInfo);

            /* == 7. DEBT MANAGER */
            _debtManager = Substitute.For<DebtManager>(_paidManager, _paymentPlanManager, _workScope, _employeeManager, _emailManager, _backgroundJobManager);

            /* == 8. PAYSLIP MANAGER */
            _calculateSalaryHub = Resolve<CalculateSalaryHub>();
            
            _payslipManager = Substitute.For<PayslipManager>(_timesheetWebService, _emailManager, _backgroundJobManager, _calculateSalaryHub, _timesheetConfig, _workScope);
            _payslipManager.UnitOfWorkManager = Resolve<IUnitOfWorkManager>();

            /* == 9. PAYROLL MANAGER */
            _payrollManager = new PayrollManager(_timesheetManager, _finfastWebService, _debtManager, _komuService, _settingManager, _payslipManager, _workScope, _emailManager)
            {
                ObjectMapper = Resolve<IObjectMapper>(),
                UnitOfWorkManager = Resolve<IUnitOfWorkManager>(),
                AbpSession = Resolve<IAbpSession>()
            };
        }

        [Fact]
        public void GetAll_Test1()
        {
            var expectedTotalCount = 4;
            WithUnitOfWork(() =>
            {
                var result = _payrollManager.GetAll();

                Assert.Equal(expectedTotalCount, result.Count);
                result.ShouldContain(payroll => payroll.Id == 150);
                result.ShouldContain(payroll => payroll.ApplyMonth == new DateTime(2023, 1, 1));
                result.ShouldContain(payroll => payroll.Status == PayrollStatus.Executed);
                result.ShouldContain(payroll => payroll.StandardWorkingDay == 22);
                result.ShouldContain(payroll => payroll.StandardOpentalk == 2);
            });
        }

        [Fact]
        public void Get_Test1()
        {
            // Standard test case
            var expectedPayroll = new Payroll
            {
                Id = 150,
                ApplyMonth = new DateTime(2022, 12, 1),
                NormalWorkingDay = 22,
                OpenTalk = 2,
                Status = PayrollStatus.Executed
            };

            WithUnitOfWork(() =>
            {
                var result = _payrollManager.Get(expectedPayroll.Id);

                Assert.NotNull(result);
                Assert.Equal(expectedPayroll.ApplyMonth, result.ApplyMonth);
                Assert.Equal(expectedPayroll.NormalWorkingDay, result.StandardWorkingDay);
                Assert.Equal(expectedPayroll.OpenTalk, result.StandardOpentalk);
                Assert.Equal(expectedPayroll.Status, result.Status);
            });
        }

        [Fact]
        public void Get_Test2()
        {
            // Non-existent payroll
            var expectedId = 5;

            WithUnitOfWork(() =>
            {
                var result = _payrollManager.Get(expectedId);

                Assert.Null(result);
            });
        }

        [Fact]
        public async Task GetAllPaging_Test1()
        {
            var gridParam = new GridParam
            {
                SkipCount = 1,
            };
            var expectedTotalCount = 4;
            var expectedItemsCount = 3;

            await WithUnitOfWorkAsync(async () =>
            {
                var result = await _payrollManager.GetAllPaging(gridParam);

                result.TotalCount.ShouldBe(expectedTotalCount);
                result.Items.Count.ShouldBe(expectedItemsCount);
                result.Items.ShouldContain(payroll => payroll.Id == 146);
                result.Items.ShouldContain(payroll => payroll.Id == 150);
            });
        }

        [Fact]
        public async Task GetAllPaging_Test2()
        {
            var gridParam = new GridParam
            {
                MaxResultCount = 10,
            };
            var expectedTotalCount = 4;
            var expectedItemsCount = 4;

            await WithUnitOfWorkAsync(async () =>
            {
                var result = await _payrollManager.GetAllPaging(gridParam);

                result.TotalCount.ShouldBe(expectedTotalCount);
                result.Items.Count.ShouldBe(expectedItemsCount);
                result.Items.ShouldContain(payroll => payroll.Id == 146);
                result.Items.ShouldContain(payroll => payroll.Id == 150);
                result.Items.ShouldContain(payroll => payroll.Id == 151);
            });
        }

        //[Fact]
        //public async Task Create_Test1()
        //{
        //    // Standard test case
        //    var newPayroll = new CreatePayrollDto
        //    {
        //        Year = 2023,
        //        Month = 3,
        //        StandardOpenTalk = 1
        //    };
        //    var expectedId = 153;
        //    var allPayrollsBeforeCreate = new List<Payroll>();

        //    await WithUnitOfWorkAsync(async () =>
        //    {
        //        allPayrollsBeforeCreate = _workScope.GetAll<Payroll>().ToList();

        //        var result = await _payrollManager.Create(newPayroll);

        //        Assert.Equal(newPayroll.Year, result.Year);
        //        Assert.Equal(newPayroll.Month, result.Month);
        //        Assert.Equal(newPayroll.StandardOpenTalk, result.StandardOpenTalk);
        //    });

        //    await WithUnitOfWorkAsync(async () =>
        //    {
        //        var allPayrollsAfterCreate = _workScope.GetAll<Payroll>();
        //        var payroll = await _workScope.GetAsync<Payroll>(expectedId);

        //        allPayrollsAfterCreate.Count().ShouldBeGreaterThan(allPayrollsBeforeCreate.Count);
        //        payroll.ShouldNotBeNull();
        //        payroll.ApplyMonth.ShouldBe(new DateTime(newPayroll.Year, newPayroll.Month, 1));
        //        payroll.OpenTalk.ShouldBe(newPayroll.StandardOpenTalk);
        //    });
        //}

        [Fact]
        public async Task Create_Test2()
        {
            // Existed payroll
            var payroll = new CreatePayrollDto
            {
                Year = 2023,
                Month = 1,
                StandardOpenTalk = 1
            };
            var expectedMessage = $"already has payroll for {payroll.Month}/{payroll.Year}";

            await WithUnitOfWorkAsync(async () =>
            {
                var exception = await Assert.ThrowsAsync<UserFriendlyException>(async () =>
                {
                    await _payrollManager.Create(payroll);
                });
                Assert.Equal(expectedMessage, exception.Message);
            });
            
        }

        [Fact]
        public void GetListDateFromPayroll_Test1()
        {
            var expectedCount = 4;

            WithUnitOfWork(() =>
            {
                var result = _payrollManager.GetListDateFromPayroll();

                result.ShouldNotBeNull();
                result.Count.ShouldBe(expectedCount);
                result.ShouldContain(new DateTime(2022, 12, 1));
                result.ShouldContain(new DateTime(2023, 1, 1));
                result.ShouldContain(new DateTime(2023, 2, 1));
            });
        }

        [Fact]
        public async Task Update_Test1()
        {
            // Standard test case
            var updatedPayroll = new UpdatePayrollDto
            {
                Id = 150,
                ApplyMonth = new DateTime(2023, 1, 1),
                Status = PayrollStatus.PendingCEO,
                NormalWorkingDay = 12,
                OpenTalk = 2
            };

            await WithUnitOfWorkAsync(async () =>
            {
                var result = await _payrollManager.Update(updatedPayroll);

                result.Id.ShouldBe(updatedPayroll.Id);
                result.ApplyMonth.ShouldBe(updatedPayroll.ApplyMonth);
                result.Status.ShouldBe(updatedPayroll.Status);
                result.NormalWorkingDay.ShouldBe(updatedPayroll.NormalWorkingDay);
                result.OpenTalk.ShouldBe(updatedPayroll.OpenTalk);
            });

            await WithUnitOfWorkAsync(async () =>
            {
                var payroll = await _workScope.GetAsync<Payroll>(updatedPayroll.Id);

                payroll.ApplyMonth.ShouldBe(updatedPayroll.ApplyMonth);
                payroll.Status.ShouldBe(updatedPayroll.Status);
                payroll.NormalWorkingDay.ShouldBe(updatedPayroll.NormalWorkingDay);
                payroll.OpenTalk.ShouldBe(updatedPayroll.OpenTalk);
            });
        }

        [Fact]
        public async Task Update_Test2()
        {
            // Non-existent payroll
            var payroll = new UpdatePayrollDto
            {
                Id = 1,
            };

            var exception = await Assert.ThrowsAsync<EntityNotFoundException>(async () =>
            {
                await _payrollManager.Update(payroll);
            });
        }

        [Fact]
        public async Task ChangeStatus_Test1()
        {
            //Standard test case
            var expectedPayroll = new ChangePayrollStatusDto
            {
                PayrollId = 150,
                Status = PayrollStatus.ApprovedByCEO
            };

            await WithUnitOfWorkAsync(async () =>
            {
                var result = await _payrollManager.ChangeStatus(expectedPayroll);
                Assert.Empty(result);
            });

            await WithUnitOfWorkAsync(async () =>
            {
                var payroll = await _workScope.GetAsync<Payroll>(expectedPayroll.PayrollId);

                payroll.Status.ShouldBe(expectedPayroll.Status);
            });
        }

        [Fact]
        public void SendMailChangeStatus_Test1()
        {
            //Change to PendingCEO
            var status = PayrollStatus.PendingCEO;
            var payrollDate = DateTime.Now;
            var payrollId = 150;

            WithUnitOfWork(() =>
            {
                var result = _payrollManager.SendMailChangeStatus(status, payrollDate, payrollId);
                Assert.Empty(result);
            });
        }

        [Fact]
        public void SendMailChangeStatus_Test2()
        {
            //Change to ApprovedByCEO
            var status = PayrollStatus.ApprovedByCEO;
            var payrollDate = DateTime.Now;
            var payrollId = 150;

            WithUnitOfWork(() =>
            {
                var result = _payrollManager.SendMailChangeStatus(status, payrollDate, payrollId);
                Assert.Empty(result);
            });
        }

        [Fact]
        public void SendMailChangeStatus_Test3()
        {
            //Change to RejectedByCEO
            var status = PayrollStatus.RejectedByCEO;
            var payrollDate = DateTime.Now;
            var payrollId = 150;

            WithUnitOfWork(() =>
            {
                var result = _payrollManager.SendMailChangeStatus(status, payrollDate, payrollId);
                Assert.Empty(result);
            });
        }

        [Fact]
        public void SendMailChangeStatus_Test4()
        {
            // Other status
            var payrollDate = new DateTime(2023, 3, 1);
            var payrollId = 146;
            var payrollStatus = PayrollStatus.PendingKT;

            WithUnitOfWork(() =>
            {
                var result = _payrollManager.SendMailChangeStatus(payrollStatus, payrollDate, payrollId);
                Assert.Empty(result);
            });
        }

        [Fact]
        public void SendMail_Test1()
        {
            // Standard test case
            var payrollDate = new DateTime(2023, 3, 1);
            var payrollId = 150;
            var payrollStatus = PayrollStatus.New;
            var templateType = MailFuncEnum.PayrollApprovedByCEO;

            WithUnitOfWork(() =>
            {
                var result = _payrollManager.SendMail(payrollDate, payrollId, payrollStatus, templateType);
                Assert.Empty(result);
            });
        }

        [Fact]
        public void SendMail_Test2()
        {
            // Empty receiver in email template
            var payrollDate = new DateTime(2023, 3, 1);
            var payrollId = 150;
            var payrollStatus = PayrollStatus.New;
            var templateType = MailFuncEnum.Checkpoint;

            var expectedMessage = $"Can not send mail because mail receiver is not set in template:&nbsp;Payroll&nbsp;{payrollStatus.ToString()}"; ;

            WithUnitOfWork(() =>
            {
                var result = _payrollManager.SendMail(payrollDate, payrollId, payrollStatus, templateType);
                Assert.Equal(expectedMessage, result);
            });
        }

        [Fact]
        public async Task Delete_Test1()
        {
            // Standard test case
            var deleteId = 150;

            var allPayrollsBeforeDelete = new List<Payroll>();
            var allPayslipsOfPayrollBeforeDelete = new List<Payslip>();

            await WithUnitOfWorkAsync(async () =>
            {
                allPayrollsBeforeDelete = _workScope.GetAll<Payroll>().ToList();
                allPayslipsOfPayrollBeforeDelete = _workScope.GetAll<Payslip>().Where(x => x.PayrollId == deleteId).ToList();

                var result = await _payrollManager.Delete(deleteId);

                Assert.Equal(deleteId, result);
            });

            await WithUnitOfWorkAsync(async () =>
            {
                var allPayrollsAfterDelete = _workScope.GetAll<Payroll>();
                var allPayslipsOfPayrollAfterDelete = _workScope.GetAll<Payslip>().Where(x => x.PayrollId == deleteId);

                allPayrollsAfterDelete.Count().ShouldBeLessThan(allPayrollsBeforeDelete.Count);
                allPayslipsOfPayrollAfterDelete.Count().ShouldBeLessThan(allPayslipsOfPayrollBeforeDelete.Count);

                await Assert.ThrowsAsync<EntityNotFoundException>(async () =>
                {
                    await _workScope.GetAsync<Payroll>(deleteId);
                });
            });
        }

        [Fact]
        public async void ExecuatePayroll_Test1()
        {
            // Standard test case
            var payrollId = 150;

            WithUnitOfWork(() =>
            {
                var result = _payrollManager.ExecuatePayroll(payrollId);
                
                Assert.Empty(result);
            });

            await WithUnitOfWorkAsync(async () =>
            {
                var payroll = await _workScope.GetAsync<Payroll>(payrollId);

                payroll.Status.ShouldBe(PayrollStatus.Executed);
            });
        }

        [Fact]
        public void ExecuatePayroll_Test2()
        {
            // Non-existent payroll
            var payrollId = 1;
            var expectedMessage = $"Can't find payroll with id {payrollId}";

            WithUnitOfWork(() =>
            {
                var exception = Assert.Throws<UserFriendlyException>(() =>
                {
                    _payrollManager.ExecuatePayroll(payrollId);
                });
                Assert.Equal(expectedMessage, exception.Message);
            });
        }

        [Fact]
        public void NotifyChangeStatus_Test1()
        {
            var payrollStatus = PayrollStatus.PendingCEO;
            var payrollDate = DateTime.Now;

            WithUnitOfWork(() =>
            {
                _payrollManager.NotifyChangeStatus(payrollStatus, payrollDate);
            });
        }

        [Fact]
        public void CreateFinfastOutcomeEntry_Test1()
        {
            var payrollId = 150;

            WithUnitOfWork(() =>
            {
                _payrollManager.CreateFinfastOutcomeEntry(payrollId);
            });
        }

        [Fact]
        public void ActivePunishmentBonusRefund_Test1()
        {
            var year = 2022;
            var month = 12;

            WithUnitOfWork(() =>
            {
                _payrollManager.ActivePunishmentBonusRefund(year, month);

                var allBonuses = _workScope.GetAll<Bonus>()
                    .Where(x => x.ApplyMonth.Year == year)
                    .Where(x => x.ApplyMonth.Month == month)
                    .Where(x => !x.IsActive);

                var allPunishments = _workScope.GetAll<Punishment>()
                    .Where(x => x.Date.Year == year)
                    .Where(x => x.Date.Month == month)
                    .Where(x => !x.IsActive);

                var allRefunds = _workScope.GetAll<Refund>()
                    .Where(x => x.Date.Year == year)
                    .Where(x => x.Date.Month == month)
                    .Where(x => !x.IsActive);

                Assert.Empty(allBonuses);
                Assert.Empty(allPunishments);
                Assert.Empty(allRefunds);
            });
        }

        [Fact]
        public void DeActivePunishmentBonusRefund_Test1()
        {
            var year = 2022;
            var month = 12;

            WithUnitOfWork(() =>
            {
                _payrollManager.DeActivePunishmentBonusRefund(year, month);

                var allBonuses = _workScope.GetAll<Bonus>()
                    .Where(x => x.ApplyMonth.Year == year)
                    .Where(x => x.ApplyMonth.Month == month)
                    .Where(x => x.IsActive);

                var allPunishments = _workScope.GetAll<Punishment>()
                    .Where(x => x.Date.Year == year)
                    .Where(x => x.Date.Month == month)
                    .Where(x => x.IsActive);

                var allRefunds = _workScope.GetAll<Refund>()
                    .Where(x => x.Date.Year == year)
                    .Where(x => x.Date.Month == month)
                    .Where(x => x.IsActive);

                Assert.Empty(allBonuses);
                Assert.Empty(allPunishments);
                Assert.Empty(allRefunds);
            });
        }

        [Fact]
        public void SetDoneDebt_Test1()
        {
            var payslipIds = new List<long>() { 57255 };
            var debtId = 89;

            WithUnitOfWork(async () =>
            {
                _payrollManager.SetDoneDebt(payslipIds);

                var debt = await _workScope.GetAsync<Debt>(debtId);
                Assert.Equal(DebtStatus.Done, debt.Status);
            });
        }
    }
}
