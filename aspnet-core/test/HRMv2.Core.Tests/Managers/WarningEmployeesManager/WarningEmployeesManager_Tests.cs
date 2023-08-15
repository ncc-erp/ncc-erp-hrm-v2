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
using HRMv2.Manager.WarningEmployees;
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
using Castle.Core.Internal;
using Abp.ObjectMapping;
using static HRMv2.Constants.Enum.HRMEnum;
using HRMv2.Manager.Employees.Dto;
using HRMv2.Manager.WarningEmployees.Dto;
using DocumentFormat.OpenXml.Office.CustomUI;

namespace HRMv2.Core.Tests.Managers.WarningEmployees
{
    public class WarningEmployeesManager_Tests : HRMv2CoreTestBase
    {
        private readonly WarningEmployeeManager _warningEmployeesManager;
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

        public WarningEmployeesManager_Tests()
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
            var _storeJob = Resolve<IRepository<BackgroundJobInfo, long>>();
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

            
            _warningEmployeesManager = new WarningEmployeeManager(_workScope, _employeeManager, _backgroundJobManager, _storeJob, _historyManager);

            _warningEmployeesManager.ObjectMapper = Resolve<IObjectMapper>();
            _warningEmployeesManager.UnitOfWorkManager = Resolve<IUnitOfWorkManager>();
        }

        [Fact]
        public async Task Should_Get_All_Employees_To_Update_Contract_By_GridPram()
        {
            await WithUnitOfWorkAsync(async () =>
            {
                var result = await _warningEmployeesManager.GetAllEmployeesToUpdateContract(new InputMultiFilterEmployeePagingDto
                {
                    GridParam = new GridParam
                    {
                        MaxResultCount = 4,
                        SkipCount = 2,
                    },
                });

                result.TotalCount.ShouldBe(19);
                result.Items.Count.ShouldBe(4);
                result.Items.ShouldContain(item => item.Id == 880);
                result.Items.ShouldContain(item => item.Status == EmployeeStatus.Working);
                result.Items.ShouldNotContain(item => item.Status == EmployeeStatus.Quit);
                result.Items.ShouldNotContain(item => item.Status == EmployeeStatus.Pausing);
            });
        }

        [Fact]
        public async Task Should_Get_All_Employees_To_Update_Contract_By_Branch_Ids()
        {

            await WithUnitOfWorkAsync(async () =>
            {
                var result = await _warningEmployeesManager.GetAllEmployeesToUpdateContract(new InputMultiFilterEmployeePagingDto
                {
                    GridParam = new GridParam
                    {
                        SkipCount = 0,
                    },
                    BranchIds = new List<long> { 93, 94 },
                    /*Usertypes = new List<UserType> { UserType.Staff },
                    JobPositionIds = new List<long> { 47 }*/
                });

                result.TotalCount.ShouldBe(6);
                result.Items.Count.ShouldBe(6);
                result.Items.ShouldContain(item => item.BranchId == 93);
                result.Items.ShouldContain(item => item.BranchId == 94);
                result.Items.ShouldNotContain(item => item.BranchId == 95);
            });
        }

        [Fact]
        public async Task Should_Get_All_Employees_To_Update_Contract_By_User_Types()
        {

            await WithUnitOfWorkAsync(async () =>
            {
                var result = await _warningEmployeesManager.GetAllEmployeesToUpdateContract(new InputMultiFilterEmployeePagingDto
                {
                    GridParam = new GridParam
                    {
                        SkipCount = 0,
                    },
                    Usertypes = new List<UserType> { UserType.Staff, UserType.Internship },
                });

                result.TotalCount.ShouldBe(9);
                result.Items.Count.ShouldBe(9);
                result.Items.ShouldContain(item => item.UserType == UserType.Staff);
                result.Items.ShouldContain(item => item.UserType == UserType.Internship);
                result.Items.ShouldNotContain(item => item.UserType == UserType.ProbationaryStaff);
                result.Items.ShouldNotContain(item => item.UserType == UserType.Collaborators);
                result.Items.ShouldNotContain(item => item.UserType == UserType.Vendor);
            });
        }

        [Fact]
        public async Task Should_Get_All_Employees_To_Update_Contract_By_Job_Postion_Ids()
        {

            await WithUnitOfWorkAsync(async () =>
            {
                var result = await _warningEmployeesManager.GetAllEmployeesToUpdateContract(new InputMultiFilterEmployeePagingDto
                {
                    GridParam = new GridParam
                    {
                        SkipCount = 0,
                    },
                    JobPositionIds = new List<long> { 47 },
                });

                result.TotalCount.ShouldBe(6);
                result.Items.Count.ShouldBe(6);
                result.Items.ShouldContain(item => item.JobPositionId == 47);
                result.Items.ShouldNotContain(item => item.JobPositionId == 48);
            });
        }

        [Fact]
        public async Task Should_Get_All_Employees_Back_To_Work_By_GridPrams()
        {
            await WithUnitOfWorkAsync(async () =>
            {
                var result = await _warningEmployeesManager.GetAllEmployeesBackToWork(new InputMultiFilterEmployeePagingDto
                {
                    GridParam = new GridParam
                    {
                        SkipCount= 1,
                        MaxResultCount = 10,
                    },
                });

                result.TotalCount.ShouldBe(4);
                result.Items.Count.ShouldBe(3);
                result.Items.ShouldContain(item => item.Id == 898);
                result.Items.ShouldContain(item => item.Id == 893);
                result.Items.ShouldContain(item => item.Id == 895);
                result.Items.ShouldContain(item => item.Status == EmployeeStatus.Pausing);
                result.Items.ShouldContain(item => item.Status == EmployeeStatus.MaternityLeave);
                result.Items.ShouldNotContain(item => item.Status == EmployeeStatus.Working);
                result.Items.ShouldNotContain(item => item.Status == EmployeeStatus.Quit);
            });
        }

        [Fact]
        public async Task Should_Get_All_Employees_Back_To_Work_By_StatusIds()
        {
            await WithUnitOfWorkAsync(async () =>
            {
                var result = await _warningEmployeesManager.GetAllEmployeesBackToWork(new InputMultiFilterEmployeePagingDto
                {
                    GridParam = new GridParam
                    {
                        SkipCount = 0,
                    },
                    StatusIds= new List<EmployeeStatus> { EmployeeStatus.Pausing }
                });

                result.TotalCount.ShouldBe(3);
                result.Items.Count.ShouldBe(3);
                result.Items.ShouldContain(item => item.Id == 899);
                result.Items.ShouldContain(item => item.Id == 898);
                result.Items.ShouldContain(item => item.Id == 893);
                result.Items.ShouldContain(item => item.Status == EmployeeStatus.Pausing);
                result.Items.ShouldNotContain(item => item.Status == EmployeeStatus.MaternityLeave);
                result.Items.ShouldNotContain(item => item.Status == EmployeeStatus.Working);
                result.Items.ShouldNotContain(item => item.Status == EmployeeStatus.Quit);
            });
        }

        [Fact]
        public async Task Should_Update_Valid_Employee_BackDate()
        {
            await WithUnitOfWorkAsync(async () =>
            {
                var result = await _warningEmployeesManager.UpdateEmployeeBackDate(new UpdateEmployeeBackdateDto
                {
                    EmployeeId = 893,
                    BackDate = new DateTime(2023, 1, 1)
                });

                result.EmployeeId.ShouldBe(893);
                result.BackDate.ShouldBe(new DateTime(2023, 1, 1));
            });

            await WithUnitOfWorkAsync(async () =>
            {
                var expectEmployeeWorkingHistory = new EmployeeWorkingHistory
                {
                    Id = 1314,
                    EmployeeId = 893,
                    Status = EmployeeStatus.Pausing,
                    BackDate = new DateTime(2023, 1, 1),
                    DateAt = new DateTime(2022, 12, 10),
                };

                var employeeWorkingHistory = await _workScope.GetAsync<EmployeeWorkingHistory>(1314);

                employeeWorkingHistory.Id.ShouldBe(expectEmployeeWorkingHistory.Id);
                employeeWorkingHistory.EmployeeId.ShouldBe(expectEmployeeWorkingHistory.EmployeeId);
                employeeWorkingHistory.BackDate.ShouldBe(expectEmployeeWorkingHistory.BackDate);
                employeeWorkingHistory.DateAt.ShouldBe(expectEmployeeWorkingHistory.DateAt);
                employeeWorkingHistory.Status.ShouldBe(expectEmployeeWorkingHistory.Status);
            });
        }

        [Fact]
        public async Task Should_Not_Update_Valid_Employee_BackDate_Without_Found_EmployeeId()
        {
            await WithUnitOfWorkAsync(async () =>
            {
                var exception = await Assert.ThrowsAsync<UserFriendlyException>(async () =>
                {
                    await _warningEmployeesManager.UpdateEmployeeBackDate(new UpdateEmployeeBackdateDto
                    {
                        EmployeeId = -100,
                        BackDate = new DateTime(2023, 1, 1)
                    });
                });

                exception.Message.ShouldBe("Can't found employee working history");
            });
        }

        [Fact]
        public async Task Should_Get_Temp_Employee_Talent_Paging_By_GridPrams()
        {
            await WithUnitOfWorkAsync(async () =>
            {
                var result = await _warningEmployeesManager.GetTempEmployeeTalentPaging(new InputGetTemEmployeeTalentDto
                {
                    GridParam = new GridParam
                    {
                        SkipCount = 1,
                        MaxResultCount= 10,
                    }
                },true);

                result.TotalCount.ShouldBe(5);
                result.Items.Count.ShouldBe(4);
                result.Items.ShouldContain(item => item.Id == 2);
                result.Items.ShouldContain(item => item.Id == 3);
                result.Items.ShouldContain(item => item.Id == 4);
                result.Items.ShouldContain(item => item.Id == 5);
            });
        }

        [Fact]
        public async Task Should_Get_Temp_Employee_Talent_Paging_By_BranchIds()
        {
            await WithUnitOfWorkAsync(async () =>
            {
                var result = await _warningEmployeesManager.GetTempEmployeeTalentPaging(new InputGetTemEmployeeTalentDto
                {
                    GridParam = new GridParam
                    {
                        SkipCount = 0,
                    },
                    BranchIds = new List<long> { 93, 94}
                },true);

                result.TotalCount.ShouldBe(3);
                result.Items.Count.ShouldBe(3);
                result.Items.ShouldContain(item => item.BranchId == 93);
                result.Items.ShouldContain(item => item.BranchId == 94);
                result.Items.ShouldNotContain(item => item.BranchId == 92);
            });
        }

        [Fact]
        public async Task Should_Get_Temp_Employee_Talent_Paging_By_User_Types()
        {
            await WithUnitOfWorkAsync(async () =>
            {
                var result = await _warningEmployeesManager.GetTempEmployeeTalentPaging(new InputGetTemEmployeeTalentDto
                {
                    GridParam = new GridParam
                    {
                        SkipCount = 0,
                    },
                    Usertypes = new List<UserType> { UserType.Internship}
                }, true);

                result.TotalCount.ShouldBe(5);
                result.Items.Count.ShouldBe(5);
                result.Items.ShouldContain(item => item.UserType == UserType.Internship);
                result.Items.ShouldNotContain(item => item.UserType == UserType.Staff);
                result.Items.ShouldNotContain(item => item.UserType == UserType.ProbationaryStaff);
                result.Items.ShouldNotContain(item => item.UserType == UserType.Collaborators);
                result.Items.ShouldNotContain(item => item.UserType == UserType.Vendor);
            });
        }

        [Fact]
        public async Task Should_Get_Temp_Employee_Talent_Paging_By_Job_Posiotions()
        {
            await WithUnitOfWorkAsync(async () =>
            {
                var result = await _warningEmployeesManager.GetTempEmployeeTalentPaging(new InputGetTemEmployeeTalentDto
                {
                    GridParam = new GridParam
                    {
                        SkipCount = 0,
                    },
                    JobPositionIds = new List<long> { 47, 48 }
                }, true);

                result.TotalCount.ShouldBe(4);
                result.Items.Count.ShouldBe(4);
                result.Items.ShouldContain(item => item.JobPositionId == 47);
                result.Items.ShouldContain(item => item.JobPositionId == 48);
                result.Items.ShouldNotContain(item => item.JobPositionId == 50);
            });
        }

        [Fact]
        public async Task Should_Get_Temp_Employee_Talent_Paging_By_Status_Ids()
        {
            await WithUnitOfWorkAsync(async () =>
            {
                var result = await _warningEmployeesManager.GetTempEmployeeTalentPaging(new InputGetTemEmployeeTalentDto
                {
                    GridParam = new GridParam
                    {
                        SkipCount = 0,
                    },
                    StatusIds = new List<TalentOnboardStatus> { TalentOnboardStatus.AcceptedOffer }
                }, true);

                result.TotalCount.ShouldBe(1);
                result.Items.Count.ShouldBe(1);
                result.Items.ShouldContain(item => item.OnboardStatus == TalentOnboardStatus.AcceptedOffer);
                result.Items.ShouldNotContain(item => item.OnboardStatus == TalentOnboardStatus.Onboarded);
                result.Items.ShouldNotContain(item => item.OnboardStatus == TalentOnboardStatus.RejectedOffer);
            });
        }

        [Fact]
        public async Task Should_Get_Temp_Employee_Talent_By_Id()
        {
            var expectTempEmployeeTalent = new TempEmployeeTalent
            {
                Id= 1,
                NCCEmail = "yeah.nguyenvan@ncc.comcc",
                FullName = "Nguyen Van Yea",
                Gender = Sex.Male,
                UserType = UserType.Internship,
                BranchId = 93,
                JobPositionId = 47,
                LevelId = 312,
                Phone = "0789658655",
                DateOfBirth = new DateTime(2002, 6, 6),
                OnboardDate = new DateTime(2022, 9, 8),
                Status = TalentOnboardStatus.Onboarded,
                Salary = 30000000,
                Skills = "o co gi"
            };
            await WithUnitOfWorkAsync(async () =>
            {
                var result = _warningEmployeesManager.GetTempEmployeeTalentById(1);

                result.Id.ShouldBe(expectTempEmployeeTalent.Id);
                result.NCCEmail.ShouldBe(expectTempEmployeeTalent.NCCEmail);
                result.FullName.ShouldBe(expectTempEmployeeTalent.FullName);
                result.BranchId.ShouldBe(expectTempEmployeeTalent.BranchId);
                result.JobPositionId.ShouldBe(expectTempEmployeeTalent.JobPositionId);
                result.LevelId.ShouldBe(expectTempEmployeeTalent.LevelId);
            });
        }

        [Fact]
        public async Task Should_Get_Request_Update_Info()
        {
            await WithUnitOfWorkAsync(async () =>
            {
                var result = await _warningEmployeesManager.GetRequestUpdateInfo(new InputMultiFilterRequestDto
                {
                    GridParam = new GridParam
                    {
                        SkipCount = 2,
                        MaxResultCount = 4
                    },
                    RequestStatuses = new List<RequestStatus> { RequestStatus.Pending } 
                });

                result.TotalCount.ShouldBe(9);
                result.Items.Count.ShouldBe(4);
                result.Items.ShouldContain(item => item.Id == 507 && item.EmployeeId == 887);
                result.Items.ShouldContain(item => item.Id == 515 && item.EmployeeId == 897);
                result.Items.ShouldContain(item => item.RequestStatus == RequestStatus.Pending);
                result.Items.ShouldNotContain(item => item.Id == 500);
                result.Items.ShouldNotContain(item => item.RequestStatus == RequestStatus.Approved);
                result.Items.ShouldNotContain(item => item.RequestStatus == RequestStatus.Rejected);
            });
        }

        [Fact]
        public async Task Should_Approve_Request_Update_Info()
        {
            await WithUnitOfWorkAsync(async () =>
            {
                await _warningEmployeesManager.ApproveRequestUpdateInfo(new ApproveChangeInfoDto
                {
                    Id = 500,
                    Phone = "0912345678",
                    Birthday = new DateTime(2000, 2, 2),
                    BankId = 1,
                    BankAccountNumber = "196009852000",
                    TaxCode = "1234567",
                    IdCard = "123456789",
                    Address = "Thanh Trì - Hà Nội",
                    IssuedOn = new DateTime(2022, 12, 2),
                    IssuedBy = "CỤC CẢNH SÁT ĐKQL CƯ TRÚ VÀ DLQG VỀ DÂN CƯ"
                });
            });

            await WithUnitOfWorkAsync(async () =>
            {
                var expectEmployee = new Employee
                {
                    Id = 880,
                    Phone = "0912345678",
                    Birthday = new DateTime(2000, 2, 2),
                    BankId = 1,
                    BankAccountNumber = "196009852000",
                    TaxCode = "1234567",
                    IdCard = "123456789",
                    Address = "Thanh Trì - Hà Nội",
                    IssuedOn = new DateTime(2022, 12, 2),
                    IssuedBy = "CỤC CẢNH SÁT ĐKQL CƯ TRÚ VÀ DLQG VỀ DÂN CƯ"
                };

                var request = await _workScope.GetAsync<TempEmployeeTS>(500);
                var employee = await _workScope.GetAsync<Employee>(880);

                request.RequestStatus.ShouldBe(RequestStatus.Approved);
                employee.Id.ShouldBe(expectEmployee.Id);
                employee.Phone.ShouldBe(expectEmployee.Phone);
                employee.Birthday.ShouldBe(expectEmployee.Birthday);
                employee.BankId.ShouldBe(expectEmployee.BankId);
                employee.BankAccountNumber.ShouldBe(expectEmployee.BankAccountNumber);
                employee.TaxCode.ShouldBe(expectEmployee.TaxCode);
                employee.IdCard.ShouldBe(expectEmployee.IdCard);
                employee.Address.ShouldBe(expectEmployee.Address);
                employee.IssuedOn.ShouldBe(expectEmployee.IssuedOn);
                employee.IssuedBy.ShouldBe(expectEmployee.IssuedBy);
            });
        }

        [Fact]
        public async Task Should_Not_Approve_Request_Update_Info()
        {
            await WithUnitOfWorkAsync(async () =>
            {
                var id = -100;

                var exception = await Assert.ThrowsAsync<UserFriendlyException>(async () =>
                {
                    await _warningEmployeesManager.ApproveRequestUpdateInfo(new ApproveChangeInfoDto
                    {
                        Id = id,
                    });
                });

                exception.Message.ShouldBe($"Can not found any request with Id = {id}");
            });
        }

        [Fact]
        public async Task Should_Reject_Request_Update_Info()
        {
            await WithUnitOfWorkAsync(async () =>
            {
                await _warningEmployeesManager.RejectRequestUpdateInfo(new RejectChangeInfoDto
                {
                    Id = 501,
                });
            });

            await WithUnitOfWorkAsync(async () =>
            {
                var request = await _workScope.GetAsync<TempEmployeeTS>(501);

                request.RequestStatus.ShouldBe(RequestStatus.Rejected);
            });
        }

        [Fact]
        public async Task Should_Not_Reject_Request_Update_Info()
        {
            await WithUnitOfWorkAsync(async () =>
            {
                var id = -100;

                var exception = await Assert.ThrowsAsync<UserFriendlyException>(async () =>
                {
                    await _warningEmployeesManager.RejectRequestUpdateInfo(new RejectChangeInfoDto
                    {
                        Id = id,
                    });
                });

                exception.Message.ShouldBe($"Can not found any request with Id = {id}");
            });
        }

        [Fact]
        public async Task Should_Get_Request_Detail_By_Id()
        {
            var expectResquest = new GetRequestDetailDto
            {
                Id = 502,
                EmployeeId = 882,
                BankAccountNumber = "19007989789",
                EmployeeBankAccountNumber = "19007989789",
                TaxCode = "18452",
                EmployeeTaxCode = "18452",
                Address = "Ninh Bình",
                EmployeeAddress = "Ninh Bình",
            };

            await WithUnitOfWorkAsync(async () =>
            {
                var result = _warningEmployeesManager.GetRequestDetailById(502);

                result.Id.ShouldBe(expectResquest.Id);
                result.EmployeeId.ShouldBe(expectResquest.EmployeeId);
                result.BankAccountNumber.ShouldBe(expectResquest.BankAccountNumber);
                result.EmployeeBankAccountNumber.ShouldBe(expectResquest.EmployeeBankAccountNumber);
                result.TaxCode.ShouldBe(expectResquest.TaxCode);
                result.EmployeeTaxCode.ShouldBe(expectResquest.EmployeeTaxCode);
                result.Address.ShouldBe(expectResquest.Address);
                result.EmployeeAddress.ShouldBe(expectResquest.EmployeeAddress);

            });
        }
    }
}
