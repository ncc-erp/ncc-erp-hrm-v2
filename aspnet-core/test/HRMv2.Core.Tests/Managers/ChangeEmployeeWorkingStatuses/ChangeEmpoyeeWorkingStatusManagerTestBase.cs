using Abp.Application.Editions;
using Abp.Application.Features;
using Abp.BackgroundJobs;
using Abp.Configuration;
using Abp.Dependency;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.MultiTenancy;
using Abp.Net.Mail;
using Abp.Runtime.Session;
using HRMv2.Editions;
using HRMv2.Entities;
using HRMv2.Manager.Benefits.Dto;
using HRMv2.Manager.Categories.Benefits;
using HRMv2.Manager.ChangeEmployeeWorkingStatuses;
using HRMv2.Manager.ChangeEmployeeWorkingStatuses.Dto;
using HRMv2.Manager.EmployeeContracts;
using HRMv2.Manager.Notifications.Email;
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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HRMv2.Constants.Enum.HRMEnum;

namespace HRMv2.Core.Tests.Managers.ChangeEmployeeWorkingStatuses
{
    public class ChangeEmpoyeeWorkingStatusManagerTestBase : HRMv2CoreTestBase
    {
        public ChangeEmployeeWorkingStatusManager ChangeEmployeeWokingStatusManagerInstance()
        {
            var configOptions = new Dictionary<string, string>
                {
                    {"KomuService:ChannelIdDevMode", ""},
                    {"KomuService:EnableKomuNotification", "true"}
                };

            var configuration = new ConfigurationBuilder()
              .AddInMemoryCollection(configOptions)
              .Build();
            var mockWorkScope = Resolve<IWorkScope>();
            var mockBenefitManager = new Mock<BenefitManager>(mockWorkScope);
            var mockEdition = new Mock<IRepository<Edition>>();
            var mockFeatureValueStore = new Mock<IAbpZeroFeatureValueStore>();
            var mockUnitOfWorkManager = Resolve<IUnitOfWorkManager>();
            var mockEditionManager = new Mock<EditionManager>(
                mockEdition.Object,
                mockFeatureValueStore.Object,
                mockUnitOfWorkManager
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
            var mockIAbpSession = Resolve<IAbpSession>();
            var mockUploadFileService = new Mock<UploadFileService>(
                mockIUploadFile.Object,
                mockTenantManager.Object,
                mockIAbpSession
                );
            var mockIEmailSender = new Mock<IEmailSender>();
            var mockTimesheetConfig = new Mock<IOptions<TimesheetConfig>>();
            var mockEmailManager = new Mock<EmailManager>(
                mockWorkScope,
                mockIEmailSender.Object,
                mockTimesheetConfig.Object
                );
            var mockContractManager = new Mock<ContractManager>(
                mockWorkScope,
                mockUploadFileService.Object,
                mockEmailManager.Object
                );
            var mockIBackgroundJobManager = new Mock<IBackgroundJobManager>();
            var mockEmployeeRepository = Resolve<IRepository<Employee, long>>();
            var mockEmployeeHistoriesRepository = Resolve<IRepository<EmployeeWorkingHistory, long>>();
            var mockSalaryChangeRequestEmployeeRepository = Resolve<IRepository<SalaryChangeRequestEmployee, long>>();
            var mockBenefitEmployeeRepository = Resolve<IRepository<BenefitEmployee, long>>();
            var mockStoreJob = Resolve<IRepository<BackgroundJobInfo, long>>();
            var mockEmployeeContractRepository = Resolve<IRepository<EmployeeContract, long>>();
            var mockIIocResolver = new Mock<IIocResolver>();
            var mockHttpClient = new Mock<HttpClient>();
            var mockProjectService = new Mock<ProjectService>(
                mockHttpClient.Object,
                mockIAbpSession,
                mockIIocResolver.Object);
            var mockTimesheetWebService = new Mock<TimesheetWebService>(
                mockHttpClient.Object,
                mockIAbpSession,
                mockIIocResolver.Object);
            var mockIMSWebService = new Mock<IMSWebService>(
                mockHttpClient.Object,
                mockIAbpSession,
                mockIIocResolver.Object);
            var mockTalentWebService = new Mock<TalentWebService>(
                mockHttpClient.Object,
                mockIAbpSession,
                mockIIocResolver.Object);
            var mockKomuService = new Mock<KomuService>(
                mockHttpClient.Object,
                mockIAbpSession,
                configuration,
                mockIIocResolver.Object);
            var mockISettingManager = Resolve<ISettingManager>();


            var changeEmployeeWorkingStatusManager = new ChangeEmployeeWorkingStatusManager(
                mockWorkScope,
                mockBenefitManager.Object,
                mockContractManager.Object,
                mockIBackgroundJobManager.Object,
                mockEmployeeRepository,
                mockEmployeeHistoriesRepository,
                mockSalaryChangeRequestEmployeeRepository,
                mockBenefitEmployeeRepository,
                mockStoreJob,
                mockEmployeeContractRepository,
                mockProjectService.Object,
                mockTimesheetWebService.Object,
                mockIMSWebService.Object,
                mockTalentWebService.Object,
                mockKomuService.Object,
                mockISettingManager
                );
            changeEmployeeWorkingStatusManager.AbpSession = Resolve<IAbpSession>();
            return changeEmployeeWorkingStatusManager;
        }

        public GetBenefitsOfEmployeeDto BenefitsOfEmployeeDto()
        {
            return new GetBenefitsOfEmployeeDto
            {
                Id = 3163,
                BenefitName = "Benifit 1",
                BenefitId = 12,
                BenefitType = BenefitType.CheDoChung,
                Money = 500000,
                StartDate = new DateTime(2020, 1, 1),
                EndDate = new DateTime(2099, 11, 1),
                Status = true
            };
        }

        public ToQuitDto ToQuitDto()
        {
            var benifitOfEmployee = BenefitsOfEmployeeDto();
            var listBenifitEmployee = new List<GetBenefitsOfEmployeeDto> { benifitOfEmployee };

            return new ToQuitDto
            {
                EmployeeId = 894,
                ApplyDate = new DateTime(2022, 10, 1),
                Note = "Note for quite",
                ListCurrentBenefits = listBenifitEmployee,
            };
        }

        public ToPauseDto ToPauseDto()
        {
            var benifitOfEmployee = BenefitsOfEmployeeDto();
            var listBenifitEmployee = new List<GetBenefitsOfEmployeeDto> { benifitOfEmployee };

            return new ToPauseDto
            {
                EmployeeId = 894,
                ApplyDate = new DateTime(2022, 10, 1),
                Note = "Note for quite",
                ListCurrentBenefits = listBenifitEmployee,
            };
        }

        public ToMaternityLeaveDto ToMaternityLeaveDto()
        {
            var benifitOfEmployee = BenefitsOfEmployeeDto();
            var listBenifitEmployee = new List<GetBenefitsOfEmployeeDto> { benifitOfEmployee };

            return new ToMaternityLeaveDto
            {
                EmployeeId = 894,
                ApplyDate = new DateTime(2022, 10, 1),
                Note = "Note for quite",
                ListCurrentBenefits = listBenifitEmployee,
                ToSalary = 100000000,
            };
        }
        public ToWorkingDto ToWorkingDto()
        {
            var benifitOfEmployee = BenefitsOfEmployeeDto();
            var listBenifitEmployee = new List<GetBenefitsOfEmployeeDto> { benifitOfEmployee };

            return new ToWorkingDto
            {
                EmployeeId = 894,
                ApplyDate = new DateTime(2022, 10, 1),
                Note = "Note for quite",
                ListCurrentBenefits = listBenifitEmployee,
            };
        }

        public ToWorkingDto ToWorkingDto2()
        {
            var benifitOfEmployee = BenefitsOfEmployeeDto();
            var listBenifitEmployee = new List<GetBenefitsOfEmployeeDto> { benifitOfEmployee };

            return new ToWorkingDto
            {
                EmployeeId = 894,
                ApplyDate = new DateTime(2022, 2, 1),
                Note = "Note for quite",
                ListCurrentBenefits = listBenifitEmployee,
                RealSalary = 10000,
                BasicSalary = 10,
                ToLevelId = 321,
                ToJobPositionId = 47,
                ProbationPercentage = 100,
                ToUserType = UserType.Staff
            };
        }
    }
}
