using Abp.Application.Editions;
using Abp.Application.Features;
using Abp.BackgroundJobs;
using Abp.Configuration;
using Abp.Dependency;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.MultiTenancy;
using Abp.Net.Mail;
using Abp.ObjectMapping;
using Abp.Runtime.Session;
using Abp.Threading.Timers;
using HRMv2.Editions;
using HRMv2.Entities;
using HRMv2.Manager.Categories;
using HRMv2.Manager.Categories.Benefits;
using HRMv2.Manager.Categories.JobPositions;
using HRMv2.Manager.Categories.Levels;
using HRMv2.Manager.Categories.UserTypes;
using HRMv2.Manager.ChangeEmployeeWorkingStatuses;
using HRMv2.Manager.EmployeeContracts;
using HRMv2.Manager.Employees;
using HRMv2.Manager.Employees.Dto;
using HRMv2.Manager.Histories;
using HRMv2.Manager.Notifications.Email;
using HRMv2.Manager.SalaryRequests;
using HRMv2.MultiTenancy;
using HRMv2.NccCore;
using HRMv2.UploadFileServices;
using HRMv2.WebServices.IMS;
using HRMv2.WebServices.Komu;
using HRMv2.WebServices.Project;
using HRMv2.WebServices.Talent;
using HRMv2.WebServices.Timesheet;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Moq;
using NccCore.Paging;
using static HRMv2.Constants.Enum.HRMEnum;

namespace HRMv2.Core.Tests.Managers.Employees
{
    public class EmployeeManagerTestBase : HRMv2CoreTestBase
    {
        public EmployeeManager EmployeeManagerInstance()
        {
            var configOptions = new Dictionary<string, string>
                {
                    {"KomuService:ChannelIdDevMode", ""},
                    {"KomuService:EnableKomuNotification", "true"}
                };

            var configuration = new ConfigurationBuilder()
              .AddInMemoryCollection(configOptions)
              .Build();
            var mockEdition = new Mock<IRepository<Edition>>();
            var mockFeatureValueStore = new Mock<IAbpZeroFeatureValueStore>();
            var mockUnitOfWorkManager = new Mock<IUnitOfWorkManager>();
            var mockEditionManager = new Mock<EditionManager>(
                mockEdition.Object,
                mockFeatureValueStore.Object,
                mockUnitOfWorkManager.Object
                );
            var mockTenant = new Mock<IRepository<Tenant>>();
            var mockTenantFeatureSetting = new Mock<IRepository<TenantFeatureSetting, long>>();
            var mockTenantManager = new Mock<TenantManager>(
                mockTenant.Object,
                mockTenantFeatureSetting.Object,
                mockEditionManager.Object,
                mockFeatureValueStore.Object
                );
            var mockIUploadFile = new Mock<IUploadFileService>();
            var mockIAbpSession = new Mock<IAbpSession>();
            var mockUploadFileService = new Mock<UploadFileService>(
                mockIUploadFile.Object,
                mockTenantManager.Object,
                mockIAbpSession.Object
                );

            var mockIWorkScope = Resolve<IWorkScope>();
            var mockIEmailSender = new Mock<IEmailSender>();
            var mockTimesheetConfig = new Mock<IOptions<TimesheetConfig>>();
            var mockEmailManager = new Mock<EmailManager>(
                mockIWorkScope,
                mockIEmailSender.Object,
                mockTimesheetConfig.Object
                );
            var mockContractManager = new Mock<ContractManager>(
                mockIWorkScope,
                mockUploadFileService.Object,
                mockEmailManager.Object
                );

            mockContractManager.Object.ObjectMapper = Resolve<IObjectMapper>();

            var mockBranchManager = new Mock<BranchManager>(mockIWorkScope);
            var mockLevelManager = new Mock<LevelManager>(mockIWorkScope);
            var mockBenefitManager = new Mock<BenefitManager>(mockIWorkScope);
            var mockIBackgroundJobManager = new Mock<IBackgroundJobManager>();
            var mockEmployeeRepository = new Mock<IRepository<Employee, long>>();
            var mockBenefitEmployeeRepository = new Mock<IRepository<BenefitEmployee, long>>();
            var mockSalaryChangeRequestEmployeeRepository = new Mock<IRepository<SalaryChangeRequestEmployee, long>>();
            var mockEmployeeWorkingHistoryRepository = new Mock<IRepository<EmployeeWorkingHistory, long>>();
            var mockEmployeeContractRepository = new Mock<IRepository<EmployeeContract, long>>();
            var mockBackgroundJobInfoRepository = new Mock<IRepository<BackgroundJobInfo, long>>();
            var mockISettingManager = new Mock<ISettingManager>();
            var mockIIocResolver = new Mock<IIocResolver>();
            var mockHttpClient = new Mock<HttpClient>();

            var mockProjectService = new Mock<ProjectService>(
                mockHttpClient.Object,
                mockIAbpSession.Object,
                mockIIocResolver.Object
                );

            var mockTimesheetWebService = new Mock<TimesheetWebService>(
                mockHttpClient.Object,
                mockIAbpSession.Object,
                mockIIocResolver.Object
                );
            var mockIMSWebService = new Mock<IMSWebService>(
                mockHttpClient.Object,
                mockIAbpSession.Object,
                mockIIocResolver.Object
                );
            var mockTalentWebService = new Mock<TalentWebService>(
                mockHttpClient.Object,
                mockIAbpSession.Object,
                mockIIocResolver.Object
                );
            var mockKomuService = new Mock<KomuService>(
                mockHttpClient.Object,
                mockIAbpSession.Object,
                configuration,
                mockIIocResolver.Object);

            var mockChangeEmployeeWorkingStatusManager = new Mock<ChangeEmployeeWorkingStatusManager>(
                mockIWorkScope,
                mockBenefitManager.Object,
                mockContractManager.Object,
                mockIBackgroundJobManager.Object,
                mockEmployeeRepository.Object,
                mockEmployeeWorkingHistoryRepository.Object,
                mockSalaryChangeRequestEmployeeRepository.Object,
                mockBenefitEmployeeRepository.Object,
                mockBackgroundJobInfoRepository.Object,
                mockEmployeeContractRepository.Object,
                mockProjectService.Object,
                mockTimesheetWebService.Object,
                mockIMSWebService.Object,
                mockTalentWebService.Object,
                mockKomuService.Object,
                mockISettingManager.Object);
            var mockHistoryManager = new Mock<HistoryManager>(
                mockIWorkScope,
                mockBranchManager.Object,
                mockLevelManager.Object,
                mockChangeEmployeeWorkingStatusManager.Object
                );
            mockHistoryManager.Object.ObjectMapper = Resolve<IObjectMapper>();

            var mockJobPositionManager = new Mock<JobPositionManager>(mockIWorkScope);
            var mockIBackgroundJobStore = new Mock<IBackgroundJobStore>();
            var mockAbpAsyncTimer = new Mock<AbpAsyncTimer>();
            var mockBackgroundJobManager = new Mock<BackgroundJobManager>(
                mockIIocResolver.Object,
                mockIBackgroundJobStore.Object,
                mockAbpAsyncTimer.Object
                );
            var mockSalaryRequestManager = new Mock<SalaryRequestManager>(
                mockLevelManager.Object,
                mockJobPositionManager.Object,
                mockContractManager.Object,
                mockIWorkScope,
                mockEmailManager.Object,
                mockBackgroundJobManager.Object);
            mockSalaryRequestManager.Object.ObjectMapper = Resolve<IObjectMapper>();

            var mockUserTypeManager = new Mock<UserTypeManager>(mockIWorkScope);

            var employeeManager = new EmployeeManager(
                mockUploadFileService.Object,
                mockContractManager.Object,
                mockHistoryManager.Object,
                mockProjectService.Object,
                mockTimesheetWebService.Object,
                mockIMSWebService.Object,
                mockTalentWebService.Object,
                mockIWorkScope,
                mockSalaryRequestManager.Object,
                mockBenefitManager.Object,
                mockUserTypeManager.Object,
                mockChangeEmployeeWorkingStatusManager.Object,
                mockBackgroundJobManager.Object,
                mockBackgroundJobInfoRepository.Object);
            employeeManager.ObjectMapper = Resolve<IObjectMapper>();
            employeeManager.UnitOfWorkManager = Resolve<IUnitOfWorkManager>();
            employeeManager.AbpSession = Resolve<IAbpSession>();
            return employeeManager;
        }

        public GetEmployeeToAddDto EmployeeToAddDto()
        {
            return new GetEmployeeToAddDto
            {
                GridParam = new GridParam
                {
                    MaxResultCount = 9999999,
                    SkipCount = 0,
                }
            };
        }

        public CreateUpdateEmployeeDto EmployeeCreationDto()
        {
            return new CreateUpdateEmployeeDto
            {
                FullName = "Tran Van A",
                Email = "a.tranvan@ncc.asia",
                Sex = Sex.Male,
                Phone = "0988876554",
                Birthday = new DateTime(2000, 1, 1),
                IdCard = "091313393190",
                IssuedOn = new DateTime(2018, 1, 1),
                IssuedBy = "Cuc canh sat HN",
                PlaceOfPermanent = "Ha noi",
                Address = "Ha noi",
                BankId = 32,
                BankAccountNumber = "1900290976589",
                UserType = UserType.Internship,
                JobPositionId = 47,
                LevelId = 314,
                BranchId = 94,
                Skills = new List<long>() { 59, 60, 61 },
                Teams = new List<long>() { 39, 43 },
                Status = EmployeeStatus.Working,
                RemainLeaveDay = 3,
                Salary = 10000.00,
                RealSalary = 2000000.00,
                ProbationPercentage = 100,
                Avatar = "new Avatar",
                TaxCode = "01211",
                ContractCode = "131333",
                InsuranceStatus = InsuranceStatus.BHXH,
                StartWorkingDate = new DateTime(2020, 1, 1),
                ContractEndDate = new DateTime(2099, 1, 1),
                ContractStartDate = new DateTime(2020, 6, 6)
            };
        }

        public CreateUpdateEmployeeDto EmployeeUpdatingDto()
        {
            return new CreateUpdateEmployeeDto
            {
                Id = 905,
                FullName = "Tran Van A",
                Email = "bao1.tranngoc@ncc.asia",
                Sex = Sex.Male,
                Phone = "0988876554",
                Birthday = new DateTime(2000, 1, 1),
                IdCard = "091313393190",
                IssuedOn = new DateTime(2018, 1, 1),
                IssuedBy = "Cuc canh sat HN",
                PlaceOfPermanent = "Ha noi",
                Address = "Ha noi",
                BankId = 32,
                BankAccountNumber = "1900290976589",
                UserType = UserType.Internship,
                JobPositionId = 47,
                LevelId = 314,
                BranchId = 94,
                Skills = new List<long>() { 59, 60, 61 },
                Teams = new List<long>() { 39, 43 },
                Status = EmployeeStatus.Working,
                RemainLeaveDay = 3,
                Salary = 10000.00,
                RealSalary = 2000000.00,
                ProbationPercentage = 100,
                Avatar = "new Avatar",
                TaxCode = "01211",
                ContractCode = "131333",
                InsuranceStatus = InsuranceStatus.BHXH,
                StartWorkingDate = new DateTime(2020, 1, 1),
                ContractEndDate = new DateTime(2099, 1, 1),
                ContractStartDate = new DateTime(2020, 6, 6)
            };
        }

        public ChangeBranchDto BranchChangingDto()
        {
            return new ChangeBranchDto
            {
                CurrentUserLoginId = 53,
                EmployeeId = 905,
                BranchId = 95,
                Date = new DateTime(2022, 10, 10)
            };
        }
    }
}
