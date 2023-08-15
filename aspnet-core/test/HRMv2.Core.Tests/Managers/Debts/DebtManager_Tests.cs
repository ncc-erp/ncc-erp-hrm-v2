using Abp.UI;
using HRMv2.Entities;
using HRMv2.Manager.Debts;
using HRMv2.Manager.Debts.Dto;
using HRMv2.Manager.Debts.PaidsManagger;
using HRMv2.Manager.Debts.PaymentPlansManager;
using HRMv2.NccCore;
using Microsoft.EntityFrameworkCore;
using NccCore.Paging;
using static HRMv2.Constants.Enum.HRMEnum;
using HRMv2.Manager.Notifications.Email;
using HRMv2.Manager.Notifications.Email.Dto;
using Abp.BackgroundJobs;
using Xunit;
using HRMv2.Manager.Employees;
using Abp.Application.Features;
using Abp.Configuration;
using Abp.Dependency;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.MultiTenancy;
using Abp.Net.Mail;
using Abp.Runtime.Session;
using Abp.Threading.Timers;
using HRMv2.Editions;
using HRMv2.Manager.Categories.Benefits;
using HRMv2.Manager.Categories.JobPositions;
using HRMv2.Manager.Categories.Levels;
using HRMv2.Manager.Categories.UserTypes;
using HRMv2.Manager.Categories;
using HRMv2.Manager.ChangeEmployeeWorkingStatuses;
using HRMv2.Manager.EmployeeContracts;
using HRMv2.Manager.Histories;
using HRMv2.Manager.SalaryRequests;
using HRMv2.MultiTenancy;
using HRMv2.UploadFileServices;
using HRMv2.WebServices.IMS;
using HRMv2.WebServices.Komu;
using HRMv2.WebServices.Project;
using HRMv2.WebServices.Talent;
using HRMv2.WebServices.Timesheet;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using NSubstitute;
using Abp.Application.Editions;
using Shouldly;
using SortDirection = NccCore.Paging.SortDirection;

namespace HRMv2.Core.Tests.Managers.Debts
{
    public class DebtManager_Tests : HRMv2CoreTestBase
    {
        private readonly EmployeeManager _employeeManager;
        private readonly EmailManager _emailManager;
        private readonly DebtManager _debt;
        private readonly IWorkScope _work;
        private readonly KomuService _komuService;
        private readonly PaidManager _paidManager;
        private readonly PaymentPlanManager _paymentPlanManager;

        public DebtManager_Tests()
        {
            _work = Resolve<IWorkScope>();
            var _httpClient = Resolve<HttpClient>();
            var _iAbpSession = Resolve<IAbpSession>();
            var _iocResovler = Resolve<IIocResolver>();
            _paidManager = Substitute.For<PaidManager>(_work);
            _paymentPlanManager = Substitute.For<PaymentPlanManager>(_work);
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
            _emailManager = Substitute.For<EmailManager>(_work, _emailSender, _timesheetConfig);
            // 2.2. ContractManager
            var _contractManager = Substitute.For<ContractManager>(_work, _uploadFileService, _emailManager);
            /* == 3. HISTORY MANAGER */
            // 3.1. BranchManager
            var moqBranchManager = Substitute.For<BranchManager>(_work);
            // 3.2. LevelManager
            var moqLevelManager = Substitute.For<LevelManager>(_work);
            // 3.3. ChangeEmployeeWorkingStatusManager 
            var _benefitManager = Substitute.For<BenefitManager>(_work);
            var _IBackgroundJobManager = Resolve<IBackgroundJobManager>();
            var _employee = Resolve<IRepository<Employee, long>>();
            var _employeeWorkingHistory = Resolve<IRepository<EmployeeWorkingHistory, long>>();
            var _salaryChangeRequestEmployee = Resolve<IRepository<SalaryChangeRequestEmployee, long>>();
            var _benefitEmployee = Resolve<IRepository<BenefitEmployee, long>>();
            var _backgroundJobInfo = Resolve<IRepository<BackgroundJobInfo, long>>();
            var _employeeContract = Resolve<IRepository<EmployeeContract, long>>();
            var _projectService = Substitute.For<ProjectService>(_httpClient, _iAbpSession, _iocResovler);
            var _timesheetWebService = Substitute.For<TimesheetWebService>(_httpClient, _iAbpSession, _iocResovler); ;
            var _IMSWebService = Substitute.For<IMSWebService>(_httpClient, _iAbpSession, _iocResovler);
            var _talentWebService = Substitute.For<TalentWebService>(_httpClient, _iAbpSession, _iocResovler);
            _komuService = Substitute.For<KomuService>(_httpClient, _iAbpSession, configuration, _iocResovler);
            var _settingManager = Resolve<ISettingManager>();
            var _changeEmployeeWorkingStatusManager
                = Substitute.For<ChangeEmployeeWorkingStatusManager>(_work, _benefitManager, _contractManager,
                _IBackgroundJobManager, _employee, _employeeWorkingHistory, _salaryChangeRequestEmployee,
                _benefitEmployee, _backgroundJobInfo, _employeeContract, _projectService, _timesheetWebService,
                _IMSWebService, _talentWebService, _komuService, _settingManager);
            // 3.4. HistoryManager
            var _historyManager = Substitute.For<HistoryManager>(_work, moqBranchManager, moqLevelManager, _changeEmployeeWorkingStatusManager);
            /* == 4. SALARY REQUEST MANAGER */
            var _jobPositionManager = Substitute.For<JobPositionManager>(_work);
            var _IBackgroundJobStore = Resolve<IBackgroundJobStore>();
            var _AbpAsyncTimer = Resolve<AbpAsyncTimer>();
            var _backgroundJobManager = Substitute.For<BackgroundJobManager>(_iocResovler, _IBackgroundJobStore, _AbpAsyncTimer);
            var _salaryRequestManager = Substitute.For<SalaryRequestManager>(moqLevelManager, _jobPositionManager, _contractManager, _work, _emailManager, _backgroundJobManager);
            /* == 5. USER TYPE MANAGER */
            var moqUserTypeManager = Substitute.For<UserTypeManager>(_work);
            /* == 6. EMPLOYEE MANAGER */
            _employeeManager = Substitute.For<EmployeeManager>(_uploadFileService, _contractManager, _historyManager, _projectService, _timesheetWebService, _IMSWebService, _talentWebService, _work, _salaryRequestManager, _benefitManager, moqUserTypeManager, _changeEmployeeWorkingStatusManager, _backgroundJobManager, _backgroundJobInfo);
            _debt = new DebtManager(_paidManager, _paymentPlanManager, _work, _employeeManager, _emailManager, _backgroundJobManager);
        }

        [Fact]
        public void GetAll()
        {
            WithUnitOfWork(() =>
            {
                var debts = _debt.GetAll();
                Assert.Equal(10, debts.Count);
                debts.ShouldContain(debt => debt.EmployeeId == 880);
                debts.ShouldContain(debt => debt.InterestRate == 0.5);
                debts.ShouldContain(debt => debt.Money == 6000000.0);
                debts.ShouldContain(debt => debt.Email == "an.phamthien@ncc.asia");
                debts.ShouldContain(debt => debt.FullName == "Phạm Thiên An");
                debts.ShouldContain(debt => debt.PaymentType == DebtPaymentType.Salary);
                debts.ShouldContain(debt => debt.Status == EmployeeStatus.Working);
                debts.ShouldContain(debt => debt.DebtStatus == DebtStatus.Inprogress);
            });
        }

        [Fact]
        public async Task Get()
        {
            WithUnitOfWork(async () =>
            {
                var Id = 90;
                var debt = _debt.Get(Id);
                Assert.Equal(880, debt.EmployeeId);
                Assert.Equal(0.5, debt.InterestRate);
                Assert.Equal(6000000.0, debt.Money);
                Assert.Equal("an.phamthien@ncc.asia", debt.Email);
                Assert.Equal("Phạm Thiên An", debt.FullName);
                Assert.Equal(DebtPaymentType.Salary, debt.PaymentType);
                Assert.Equal(EmployeeStatus.Working, debt.Status);
                Assert.Equal(DebtStatus.Inprogress, debt.DebtStatus);
            });
        }

        [Fact]
        public async Task GetByEmployeeId()
        {
            WithUnitOfWork(async () =>
            {
                var Id = 883;
                GridParam input = new();
                var debts = await _debt.GetByEmployeeId(Id, input);
                Assert.Equal(2, debts.Items.Count);
                debts.Items.ShouldContain(debt => debt.EmployeeId == 883);
                debts.Items.ShouldContain(debt => debt.InterestRate == 0);
                debts.Items.ShouldContain(debt => debt.Money == 6000000.0);
                debts.Items.ShouldContain(debt => debt.Email == "hung.trankien@ncc.asia");
                debts.Items.ShouldContain(debt => debt.FullName == "Trần Kiên Hưng");
                debts.Items.ShouldContain(debt => debt.PaymentType == DebtPaymentType.Salary);
                debts.Items.ShouldContain(debt => debt.Status == EmployeeStatus.Working);
                debts.Items.ShouldContain(debt => debt.DebtStatus == DebtStatus.Inprogress);
            });
        }

        [Fact]
        public async Task Create()
        {
            WithUnitOfWork(async () =>
            {
                long Id = 905;
                //Employee employee = _employeeManager.Get(Id);
                CreateDebtDto input = new()
                {
                    EmployeeId = Id,
                    InterestRate = 0.05,
                    Money = 100000,
                    StartDate = DateTime.Now,
                    EndDate = DateTime.Now,
                    Note = "",
                    PaymentType = DebtPaymentType.Salary,
                    DebtStatus = DebtStatus.Inprogress,
                };
                long debt = await _debt.Create(input);
                Assert.Equal(Id, debt);
            });
        }

        [Fact]
        public async Task Update()
        {
            long Id = 90;
            var Note = "lmao";
            var input = new UpdateDebtDto
            {
                Id = Id,
                InterestRate = 0.5,
                Money = 6000000.0,
                Note = Note,
                PaymentType = DebtPaymentType.Salary,
                DebtStatus = DebtStatus.Inprogress,
            };
            WithUnitOfWork(async () =>
            {
                var debt = await _debt.Update(input);
                Assert.NotNull(debt);
            });
            WithUnitOfWork(async () =>
            {
                var debt = await _work.GetAsync<Debt>(Id);
                Assert.Equal(Note, debt.Note);
                Assert.Equal(880, debt.EmployeeId);
                Assert.Equal(0.5, debt.InterestRate);
                Assert.Equal(6000000.0, debt.Money);
                Assert.Equal(DebtPaymentType.Salary, debt.PaymentType);
                Assert.Equal(DebtStatus.Inprogress, debt.Status);
            });
        }

        [Fact]
        public async Task SetDone()
        {
            WithUnitOfWork(async () =>
            {
                var Id = 89;
                var debt = _debt.SetDone(Id);
                Assert.Equal(Id, debt);
            });
        }

        [Fact]
        public async Task SetDone_ReturnException()
        {
            WithUnitOfWork(() =>
            {
                var Id = 90;
                var caughtException = Assert.Throws<UserFriendlyException>(() => _debt.SetDone(Id));
                Assert.Equal($"User's total payment does not equal to principal + interest", caughtException.Message);
            });
        }

        [Fact]
        public async Task Delete()
        {
            var id = 91;
            WithUnitOfWork(async () =>
            {
                var deletedDebtId = await _debt.Delete(id);
                Assert.Equal(id, deletedDebtId);
            });
            WithUnitOfWork(async () =>
            {
                var debts = _work.GetAll<Debt>();
                var debt = await debts.AnyAsync(x => x.Id == id);
                Assert.False(debt);
            });
        }

        [Fact]
        public async Task Delete_ReturnException()
        {
            WithUnitOfWork(async () =>
            {
                var id = 90;
                var caughtException = await Assert.ThrowsAsync<UserFriendlyException>(async () => await _debt.Delete(id));
                Assert.Equal($"Cannot delete this debt because it is currently in paying process or it is already done", caughtException.Message);
            });
        }

        [Fact]
        public async Task GetAllPaging_ShouldFilterByStatusIds()
        {
            await WithUnitOfWorkAsync(async () =>
            {
                // Arrange
                GetDebtEmployeeInputDto input = new()
                {
                    GridParam = new GridParam(),
                    StatusIds = new List<EmployeeStatus> { EmployeeStatus.Working },
                };

                var result = await _debt.GetAllPaging(input);
                Assert.Equal(10, result.Items.Count);
                result.Items.ShouldContain(debt => debt.EmployeeId == 885);
                result.Items.ShouldContain(debt => debt.EmployeeId == 886);
                result.Items.ShouldContain(debt => debt.EmployeeId == 887);
                result.Items.ShouldContain(debt => debt.EmployeeId == 901);
            });
        }

        [Fact]
        public async Task GetAllPaging_ShouldFilterByBranchIds()
        {
            await WithUnitOfWorkAsync(async () =>
            {
                // Arrange
                GetDebtEmployeeInputDto input = new()
                {
                    GridParam = new GridParam(),
                    BranchIds = new List<long> { 1, 2 },
                };

                var result = await _debt.GetAllPaging(input);
                Assert.Equal(0, result.Items.Count);
            });
        }

        [Fact]
        public async Task GetAllPaging_ShouldFilterByUserTypes()
        {
            await WithUnitOfWorkAsync(async () =>
            {
                // Arrange
                GetDebtEmployeeInputDto input = new()
                {
                    GridParam = new GridParam(),
                    UserTypes = new List<UserType> { UserType.Staff, UserType.Collaborators },
                };

                var result = await _debt.GetAllPaging(input);
                Assert.Equal(7, result.Items.Count);
                result.Items.ShouldContain(debt => debt.EmployeeId == 885);
                result.Items.ShouldContain(debt => debt.EmployeeId == 886);
                result.Items.ShouldContain(debt => debt.EmployeeId == 887);
                result.Items.ShouldContain(debt => debt.EmployeeId == 901);
            });
        }

        [Fact]
        public async Task GetAllPaging_ShouldFilterByLevelIds()
        {
            await WithUnitOfWorkAsync(async () =>
            {
                // Arrange
                GetDebtEmployeeInputDto input = new()
                {
                    GridParam = new GridParam(),
                    LevelIds = new List<long> { 1, 2 },
                };

                var result = await _debt.GetAllPaging(input);
                Assert.Equal(0, result.Items.Count);
            });
        }

        [Fact]
        public async Task GetAllPaging_ShouldFilterByJobPositionIds()
        {
            await WithUnitOfWorkAsync(async () =>
            {
                // Arrange
                GetDebtEmployeeInputDto input = new()
                {
                    GridParam = new GridParam(),
                    JobPositionIds = new List<long> { 1, 2 },
                };

                var result = await _debt.GetAllPaging(input);
                Assert.Equal(0, result.Items.Count);
            });
        }

        [Fact]
        public async Task GetAllPaging_ShouldFilterByGridParam()
        {
            await WithUnitOfWorkAsync(async () =>
            {
                // Arrange
                GetDebtEmployeeInputDto input = new()
                {
                    GridParam = new GridParam() { SkipCount = 2 }
                };

                var result = await _debt.GetAllPaging(input);
                Assert.Equal(8, result.Items.Count);
                result.Items.ShouldContain(debt => debt.EmployeeId == 885);
                result.Items.ShouldContain(debt => debt.EmployeeId == 886);
                result.Items.ShouldContain(debt => debt.EmployeeId == 887);
                result.Items.ShouldContain(debt => debt.EmployeeId == 901);
            });
        }

        [Fact]
        public async Task GetAllPaging_ShouldFilterByDebtStatusIds()
        {
            await WithUnitOfWorkAsync(async () =>
            {
                // Arrange
                GetDebtEmployeeInputDto input = new()
                {
                    DebtStatusIds = new List<long> { 1 },
                    GridParam = new GridParam()
                };

                var result = await _debt.GetAllPaging(input);
                Assert.True(result.Items.All(x => x.DebtStatus == DebtStatus.Inprogress));
                result.Items.ShouldNotContain(debt => debt.EmployeeId == 885);
                result.Items.ShouldContain(debt => debt.EmployeeId == 883);
                result.Items.ShouldContain(debt => debt.EmployeeId == 905);
            });
        }

        [Fact]
        public async Task GetAllPaging_ShouldFilterByPaymentTypeIds()
        {
            await WithUnitOfWorkAsync(async () =>
            {
                // Arrange
                GetDebtEmployeeInputDto input = new()
                {
                    PaymentTypeIds = new List<long> { 2 },
                    GridParam = new GridParam()
                };

                var result = await _debt.GetAllPaging(input);
                Assert.True(result.Items.All(x => x.PaymentType == DebtPaymentType.RealMoney));
                result.Items.ShouldContain(debt => debt.EmployeeId == 883);
                result.Items.ShouldNotContain(debt => debt.EmployeeId == 885);
                result.Items.ShouldNotContain(debt => debt.EmployeeId == 905);
            });
        }

        [Fact]
        public async Task SendMailToOneEmployee_ReturnException()
        {
            WithUnitOfWork(() =>
            {
                var input = new SendMailDto
                {
                    DebtId = 85,
                    MailContent = new MailPreviewInfoDto
                    {
                        TemplateId = 1,
                        Name = "Test template",
                        Description = "This is a test template for previewing emails",
                        MailFuncType = MailFuncEnum.Debt,
                        Subject = "Test email subject",
                        BodyMessage = "This is the body of the test email",
                        SendToEmail = "test@example.com",
                        PropertiesSupport = new string[] { "Property1", "Property2" },
                        CCs = new List<string> { "cc1@example.com", "cc2@example.com" },
                        CurrentUserLoginId = 1,
                        TenantId = 1
                    }
                };

                var caughtException = Assert.Throws<UserFriendlyException>(() => _debt.SendMailToOneEmployee(input));
                Assert.Equal($"Can not found debt with Id = {input.DebtId}", caughtException.Message);
            });
        }

        [Fact]
        public async Task SendMailToOneEmployee()
        {
            WithUnitOfWork(() =>
            {
                var input = new SendMailDto
                {
                    DebtId = 90,
                    MailContent = new MailPreviewInfoDto
                    {
                        TemplateId = 1,
                        Name = "Test template",
                        Description = "This is a test template for previewing emails",
                        MailFuncType = MailFuncEnum.Debt,
                        Subject = "Test email subject",
                        BodyMessage = "This is the body of the test email",
                        SendToEmail = "test@example.com",
                        PropertiesSupport = new string[] { "Property1", "Property2" },
                        CCs = new List<string> { "cc1@example.com", "cc2@example.com" },
                        CurrentUserLoginId = 1,
                        TenantId = 1
                    }
                };
                _debt.SendMailToOneEmployee(input);
                Assert.True(true);
            });
        }

        [Fact]
        public async Task SendMailToAllEmployee()
        {
            WithUnitOfWork(() =>
            {
                var input = new GetDebtEmployeeInputDto
                {
                    GridParam = new GridParam
                    {
                        // set grid param values
                        Sort = "",
                        SortDirection = SortDirection.ASC,
                        SearchText = ""
                    },
                    TeamIds = new List<long> { 1, 2, 3 },
                    IsAndCondition = true,
                    StatusIds = new List<EmployeeStatus> { EmployeeStatus.Working },
                    LevelIds = new List<long> { 1, 2 },
                    UserTypes = new List<UserType> { UserType.Internship, UserType.Staff },
                    BranchIds = new List<long> { 1, 2 },
                    JobPositionIds = new List<long> { 1, 2 },
                    DebtStatusIds = new List<long> { 1, 2 },
                    PaymentTypeIds = new List<long> { 1, 2 }
                };

                var result = _debt.SendMailToAllEmployee(input);
                Assert.Equal($"Started sending 0 email.", result);
            });
        }
    }
}
