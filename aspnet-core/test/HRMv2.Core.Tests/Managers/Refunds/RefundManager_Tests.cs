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
using HRMv2.Entities;
using HRMv2.Manager.Categories.Benefits;
using HRMv2.Manager.Categories.JobPositions;
using HRMv2.Manager.Categories.Levels;
using HRMv2.Manager.Categories.UserTypes;
using HRMv2.Manager.Categories;
using HRMv2.Manager.ChangeEmployeeWorkingStatuses;
using HRMv2.Manager.Debts.PaidsManagger;
using HRMv2.Manager.Debts.PaymentPlansManager;
using HRMv2.Manager.EmployeeContracts;
using HRMv2.Manager.Employees;
using HRMv2.Manager.Histories;
using HRMv2.Manager.Notifications.Email;
using HRMv2.Manager.Refunds;
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
using Shouldly;
using Xunit;
using NSubstitute;
using HRMv2.Editions;
using Abp.Application.Editions;
using Abp.UI;
using NccCore.Paging;
using HRMv2.Manager.Refunds.Dto;
using Castle.Core.Internal;
using Abp.ObjectMapping;
using HRMv2.Manager.Punishments.Dto;
using static HRMv2.Constants.Enum.HRMEnum;

namespace HRMv2.Core.Tests.Managers.Refunds
{
    public class RefundManager_Tests : HRMv2CoreTestBase
    {
        private readonly RefundManager _refundManager;
        private IWorkScope _workScope;
        private EmployeeManager _employeeManager;
        private FinfastWebService _finfastWebService;
        private TimesheetWebService _timesheetWebService;
        private TimesheetManager _timesheetManager;
        private PaidManager _paidManager;
        private PaymentPlanManager _paymentPlanManager;
        private EmailManager _emailManager;
        private KomuService _komuService;
        private EditionManager _editionManager;

        public RefundManager_Tests()
        {
            _workScope = Resolve<IWorkScope>();

            var _httpClient = Resolve<HttpClient>();
            var _iAbpSession = Resolve<IAbpSession>();
            var _iocResovler = Resolve<IIocResolver>();
            this._timesheetWebService = Substitute.For<TimesheetWebService>(_httpClient, _iAbpSession, _iocResovler);
            _timesheetManager = Substitute.For<TimesheetManager>(this._timesheetWebService, _workScope);

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

            var _editionManager = Substitute.For<EditionManager>(_edition, _featureValueStore, _unitOfWorkManager);

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

            _emailManager = Substitute.For<EmailManager>(_workScope, _emailSender, _timesheetConfig);

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
            var _timesheetWebService = Substitute.For<TimesheetWebService>(_httpClient, _iAbpSession, _iocResovler); ;
            var _IMSWebService = Substitute.For<IMSWebService>(_httpClient, _iAbpSession, _iocResovler);
            var _talentWebService = Substitute.For<TalentWebService>(_httpClient, _iAbpSession, _iocResovler);
            _komuService = Substitute.For<KomuService>(_httpClient, _iAbpSession, configuration, _iocResovler);
            var _settingManager = Resolve<ISettingManager>();

            var _changeEmployeeWorkingStatusManager = Substitute.For<ChangeEmployeeWorkingStatusManager>(_workScope, _benefitManager, _contractManager,
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
            var _userTypeManager = Substitute.For<UserTypeManager>(_workScope);

            /* == 6. EMPLOYEE MANAGER */
            _employeeManager = Substitute.For<EmployeeManager>(_uploadFileService, _contractManager, _historyManager, _projectService, _timesheetWebService, _IMSWebService, _talentWebService, _workScope, _salaryRequestManager, _benefitManager, _userTypeManager, _changeEmployeeWorkingStatusManager, _backgroundJobManager, _backgroundJobInfo);

            _refundManager = new RefundManager(_workScope, _employeeManager);

            _refundManager.ObjectMapper = Resolve<IObjectMapper>();
            _refundManager.UnitOfWorkManager = Resolve<IUnitOfWorkManager>();

        }

        [Fact]
        public async Task Should_Get_All_Refund()
        {
            await WithUnitOfWorkAsync(async () =>
            {
                var result = _refundManager.GetAll();

                result.Count.ShouldBeGreaterThanOrEqualTo(4);
                result.ShouldContain(refund => refund.Id == 26);
                result.ShouldContain(refund => refund.Name == "refund tiền lương tháng 10");
                result.ShouldContain(refund => refund.Date == new DateTime(2023, 1, 1));
            });
        }

        [Fact]
        public async Task Should_Get_A_Refund()
        {
            var expectRefund = new GetRefundDto
            {
                Id = 26,
                Name = "refund tiền lương",
                Date = new DateTime(2022, 12, 1),
                EmployeeCount= 0,
                TotalMoney= 0,
                IsActive= false,
            };

            await WithUnitOfWorkAsync(async () =>
            {
                var result = _refundManager.Get(26);
                // result: EmployeeCount, TotalMoney fields equals 0

                result.Id.ShouldBe(expectRefund.Id);
                result.Name.ShouldBe(expectRefund.Name);
                result.Date.ShouldBe(expectRefund.Date);
                result.IsActive.ShouldBe(expectRefund.IsActive);
            });
        }

        [Fact]
        public async Task Should_Get_All_Employee_Not_In_Refund()
        {
            await WithUnitOfWorkAsync(async () =>
            {
                var result = _refundManager.GetAllEmployeeNotInRefund(26);

                result.Count.ShouldBeGreaterThanOrEqualTo(6);
                result.ShouldNotContain(employee => employee.Id == 898);
                result.ShouldContain(employee => employee.Id == 881);
            });
        }

        [Fact]
        public async Task Should_Active_Refund()
        {
            await WithUnitOfWorkAsync(async () =>
            {
                var result = await _refundManager.ActiveRefund(26);

                result.ShouldBe(26);
            });

            await WithUnitOfWorkAsync(async () =>
            {
                var result = await _workScope.GetAsync<Refund>(26);

                result.IsActive.ShouldBe(true);
            });
        }

        [Fact]
        public async Task Should_Not_Active_Refund_Without_Id()
        {
            var id = -100;
            await WithUnitOfWorkAsync(async () =>
            {
                var exception = await Assert.ThrowsAsync<UserFriendlyException>(async () =>
                {
                    await _refundManager.ActiveRefund(id);
                });

                exception.Message.ShouldBe($"can't find refund with id {id}");
            });
        }

        [Fact]
        public async Task Should_DeActive_Refund()
        {
            await WithUnitOfWorkAsync(async () =>
            {
                var result = await _refundManager.DeActiveRefund(27);

                result.ShouldBe(27);
            });

            await WithUnitOfWorkAsync(async () =>
            {
                var result = await _workScope.GetAsync<Refund>(27);

                result.IsActive.ShouldBe(false);
            });
        }

        [Fact]
        public async Task Should_Not_DeActive_Refund_Without_Id()
        {
            var id = -100;
            await WithUnitOfWorkAsync(async () =>
            {
                var exception = await Assert.ThrowsAsync<UserFriendlyException>(async () =>
                {
                    await _refundManager.DeActiveRefund(id);
                });

                exception.Message.ShouldBe($"can't find refund with id {id}");
            });
        }

        [Fact]
        public async Task Should_Get_All_Paging()
        {
            await WithUnitOfWorkAsync(async () =>
            {
                var filter = new GridParam
                {
                    MaxResultCount = 3,
                    SkipCount = 2,
                };
                var result = await _refundManager.GetAllPaging(filter);

                result.TotalCount.ShouldBe(4);
                result.Items.Count.ShouldBe(2);
                result.Items.ShouldContain(refund => refund.Id == 26);
                result.Items.ShouldNotContain(refund => refund.Id == 28);
            });
        }

        [Fact]
        public async Task Should_Get_All_Paging_By_Search_Text()
        {
            await WithUnitOfWorkAsync(async () =>
            {
                var filter = new GridParam
                {
                    SearchText = "refund tiền lương"
                };
                var result = await _refundManager.GetAllPaging(filter);

                result.TotalCount.ShouldBe(2);
                result.Items.Count.ShouldBe(2);
                result.Items.ShouldContain(refund => refund.Id == 26);
                result.Items.ShouldNotContain(refund => refund.Id == 28);
            });
        }

        [Fact]
        public async Task Should_Create_A_Valid_Refund()
        {
            var expectRefund = new Refund { 
                Id = 30,
                Name = "Refund test",
                Date = new DateTime(2022, 12, 20),
                IsActive= true,
            };
            await WithUnitOfWorkAsync(async () =>
            {
                var result = await _refundManager.Create(new CreateRefundDto
                {
                    Name = "Refund test",
                    Date = new DateTime(2022, 12, 20)
                });;

                result.Name.ShouldBe(expectRefund.Name);
                result.Date.ShouldBe(expectRefund.Date);
            });

            await WithUnitOfWorkAsync(async () =>
            {
                var allRefund = _workScope.GetAll<Refund>();
                var refund = await _workScope.GetAsync<Refund>(30);

                allRefund.ToArray().Find(refund => refund.Id == expectRefund.Id).ShouldNotBeNull();
                refund.Id.ShouldBe(expectRefund.Id);
                refund.Name.ShouldBe(expectRefund.Name);
                refund.Date.ShouldBe(expectRefund.Date);
                refund.IsActive.ShouldBe(expectRefund.IsActive);
            });
        }

        [Fact]
        public async Task Should_Not_Create_A_Refund_Existing_Name()
        {
            await WithUnitOfWorkAsync(async () =>
            {
                var exception = await Assert.ThrowsAsync<UserFriendlyException>(async () =>
                {
                    await _refundManager.Create(new CreateRefundDto
                    {
                        Name = "refund tiền lương",
                        Date = new DateTime(2022, 12, 20)
                    });
                });

                exception.Message.ShouldBe("Name is already exist!");
            });
        }

        [Fact]
        public async Task Should_Update_A_Valid_Refund()
        {
            var expectRefund = new Refund
            {
                Id = 26,
                Name = "Refund test update",
                Date = new DateTime(2022, 12, 20),
                IsActive = true,
            };
            await WithUnitOfWorkAsync(async () =>
            {
                var result = await _refundManager.Update(new UpdateRefundDto
                {   
                    Id = 26,
                    Name = "Refund test update",
                    Date = new DateTime(2022, 12, 20),
                    IsActive = true,
                    UpdateNote = true,
                });

                result.Id.ShouldBe(expectRefund.Id);
                result.Name.ShouldBe(expectRefund.Name);
                result.IsActive.ShouldBeTrue();
                result.Date.ShouldBe(expectRefund.Date);
                result.UpdateNote.ShouldBeTrue();
            });

            await WithUnitOfWorkAsync(async () =>
            {
                var refund = await _workScope.GetAsync<Refund>(26);
                var allRefundEmployees = _workScope.GetAll<RefundEmployee>();

                var refundEmployeesByRefundId = allRefundEmployees.ToArray().Where(refundEmployees => refundEmployees.RefundId == 26);

                foreach(var item in refundEmployeesByRefundId)
                {
                    item.Note.ShouldBe("Refund test update");
                }

                refund.Id.ShouldBe(26);
                refund.Name.ShouldBe(expectRefund.Name);
                refund.IsActive.ShouldBeTrue();
                refund.Date.ShouldBe(expectRefund.Date);
            });
        }

        [Fact]
        public async Task Should_Not_Update_A_Refund_Existing_Name()
        {
            await WithUnitOfWorkAsync(async () =>
            {
                var exception = await Assert.ThrowsAsync<UserFriendlyException>(async () =>
                {
                    await _refundManager.Update(new UpdateRefundDto
                    {
                        Id = 26,
                        Name = "refund tiền lương tháng 10",
                        Date = new DateTime(2022, 12, 20),
                        IsActive = true,
                        UpdateNote = true,
                    });
                });

                exception.Message.ShouldBe("This refund name is already exist");
            });
        }

        [Fact]
        public async Task Should_Not_Update_A_Refund_Without_Id()
        {
            var id = -100;
            await WithUnitOfWorkAsync(async () =>
            {
                var exception = await Assert.ThrowsAsync<UserFriendlyException>(async () =>
                {
                    await _refundManager.Update(new UpdateRefundDto
                    {
                        Id = id,
                        Name = "Refund test update",
                        Date = new DateTime(2022, 12, 20),
                        IsActive = true,
                        UpdateNote = true,
                    });
                });

                exception.Message.ShouldBe($"Can't find refund with id {id}");
            });
        }

        [Fact]
        public async Task Should_Delete_A_Valid_Refund()
        {
            await WithUnitOfWorkAsync(async () =>
            {
                var result = await _refundManager.Delete(26);

                result.ShouldBe(26);
            });

            await WithUnitOfWorkAsync(async () =>
            {
                var allRefund = _workScope.GetAll<Refund>();

                allRefund.Count().ShouldBe(3);
                allRefund.ToArray().Find(Refund => Refund.Id == 26).ShouldBeNull();
            });
        }

        [Fact]
        public async Task Should_Get_Refund_Employees_Paging()
        {
            await WithUnitOfWorkAsync(async () =>
            {
                var result = await _refundManager.GetRefundEmployeesPaging(26, new GetEmployeePunishment
                {
                    GridParam= new GridParam
                    {
                        MaxResultCount= 10,
                        SkipCount= 0,
                    },
                    TeamIds = new List<long> { 42 },
                    LevelIds = new List<long> { 317 },
                    Usertypes = new List<UserType> { UserType.Staff },
                    BranchIds = new List<long> { 95 },
                    JobPositionIds = new List<long> { 51 }
                });

                result.ShouldNotBeNull();
                result.TotalCount.ShouldBe(1);
                result.Items.Count.ShouldBe(1);
                result.Items[0].ShouldNotBeNull();
                result.Items[0].Teams.ShouldContain(team => team.TeamId == 42);
                result.Items.ShouldContain(refundEmpolyee => refundEmpolyee.LevelId == 317);
                result.Items.ShouldContain(refundEmpolyee => refundEmpolyee.UserType == UserType.Staff);
                result.Items.ShouldContain(refundEmpolyee => refundEmpolyee.BranchId == 95);
                result.Items.ShouldContain(refundEmpolyee => refundEmpolyee.JobPositionId == 51);
            });
        }

        [Fact]
        public async Task Should_Add_Refund_Employees_To_Refund()
        {
            var expectRefundEmployee = new RefundEmployee
            {
                Id = 84,
                RefundId = 26,
                EmployeeId = 885,
                Money = 100000,
                Note = "refund tiền lương",
                IsDeleted= false,
            };

            await WithUnitOfWorkAsync(async () =>
            {
                var result = await _refundManager.AddEmployeeToRefund(new AddEmployeeToRefundDto
                {
                    RefundId = 26,
                    EmployeeId = 885,
                    Money= 100000,
                    Note = "refund tiền lương"
                });

                result.ShouldNotBeNull();
                result.RefundId.ShouldBe(expectRefundEmployee.RefundId);
                result.EmployeeId.ShouldBe(expectRefundEmployee.EmployeeId);
                result.Money.ShouldBe(expectRefundEmployee.Money);
                result.Note.ShouldBe(expectRefundEmployee.Note);
            });

            await WithUnitOfWorkAsync(async () =>
            {
                var allRefundEmployee = _workScope.GetAll<RefundEmployee>();
                var refundEmployee = await _workScope.GetAsync<RefundEmployee>(84);

                allRefundEmployee.ToArray().Find(refundEmployee => refundEmployee.RefundId == 26).ShouldNotBeNull();
                refundEmployee.Id.ShouldBe(expectRefundEmployee.Id);
                refundEmployee.RefundId.ShouldBe(expectRefundEmployee.RefundId);
                refundEmployee.EmployeeId.ShouldBe(expectRefundEmployee.EmployeeId);
                refundEmployee.Money.ShouldBe(expectRefundEmployee.Money);
                refundEmployee.Note.ShouldBe(expectRefundEmployee.Note);
            });
        }

        [Fact]
        public async Task Should_Update_Refund_Employee()
        {
            var expectRefundEmployee = new RefundEmployee
            {
                Id = 76,
                RefundId = 1,
                EmployeeId = 1,
                Money = 1000000,
                Note = "refund note update",
                IsDeleted = false,
            };

            await WithUnitOfWorkAsync(async () =>
            {
                var result = await _refundManager.UpdateRefundEmployee(new UpdateRefundemployeeDto
                {
                    Id= 76,
                    RefundId = 1,
                    EmployeeId = 1,
                    Money = 1000000,
                    Note = "refund note update"
                });

                result.ShouldNotBeNull();
                result.Id.ShouldBe(expectRefundEmployee.Id);
                result.EmployeeId.ShouldBe(expectRefundEmployee.EmployeeId);
                result.RefundId.ShouldBe(expectRefundEmployee.RefundId);
                result.Money.ShouldBe(expectRefundEmployee.Money);
                result.Note.ShouldBe(expectRefundEmployee.Note);
            });

            await WithUnitOfWorkAsync(async () =>
            {
                var refundEmployee = await _workScope.GetAsync<RefundEmployee>(76);

                refundEmployee.ShouldNotBeNull();
                refundEmployee.Id.ShouldBe(expectRefundEmployee.Id);
                refundEmployee.EmployeeId.ShouldBe(expectRefundEmployee.EmployeeId);
                refundEmployee.RefundId.ShouldBe(expectRefundEmployee.RefundId);
                refundEmployee.Money.ShouldBe(expectRefundEmployee.Money);
                refundEmployee.Note.ShouldBe(expectRefundEmployee.Note);
            });
        }


        [Fact]
        public async Task Should_Delete_Refund_Employee()
        {
            await WithUnitOfWorkAsync(async () =>
            {
                var refundEmployees = _workScope.GetAll<RefundEmployee>();

                var result = await _refundManager.DeleteRefundEmployee(76);

                result.ShouldBe(76);
                refundEmployees.Count().ShouldBe(8);

            });

            await WithUnitOfWorkAsync(async () =>
            {
                var refundEmployees = _workScope.GetAll<RefundEmployee>();

                refundEmployees.ToArray().Find(refundEmployee => refundEmployee.Id == 76).ShouldBeNull();

                refundEmployees.Count().ShouldBe(7);
            });
        }
    }
}
