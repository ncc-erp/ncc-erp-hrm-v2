using Abp;
using Abp.Application.Editions;
using Abp.Application.Features;
using Abp.BackgroundJobs;
using Abp.Configuration;
using Abp.Dependency;
using Abp.Domain.Entities;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.MultiTenancy;
using Abp.Net.Mail;
using Abp.ObjectMapping;
using Abp.Runtime.Session;
using Abp.UI;
using HRMv2.Editions;
using HRMv2.Entities;
using HRMv2.Manager.Categories;
using HRMv2.Manager.Categories.Benefits;
using HRMv2.Manager.Categories.Levels;
using HRMv2.Manager.ChangeEmployeeWorkingStatuses;
using HRMv2.Manager.EmployeeContracts;
using HRMv2.Manager.Histories;
using HRMv2.Manager.Histories.Dto;
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
using Shouldly;
using Xunit;
using static HRMv2.Constants.Enum.HRMEnum;

namespace HRMv2.Core.Tests.Managers.Histories
{
    public class HistoryManager_Tests : HRMv2CoreTestBase
    {
        private readonly IUploadFileService _uploadFileService;

        private readonly HistoryManager _historyManager;
        private readonly IWorkScope _workScope;

        public HistoryManager_Tests()
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
            var mockEditionManager = new Mock<EditionManager>(mockEdition.Object, mockFeatureValueStore.Object, mockUnitOfWorkManager.Object); ;
            var mockTenant = new Mock<IRepository<Tenant>>();
            var mockTenantFeatureSetting = new Mock<IRepository<TenantFeatureSetting, long>>();
            var mockTenantManager = new Mock<TenantManager>(mockTenant.Object, mockTenantFeatureSetting.Object, mockEditionManager.Object, mockFeatureValueStore.Object);
            var mockIUploadFile = new Mock<IUploadFileService>();
            var mockIAbpSession = new Mock<IAbpSession>();
            var mockUploadFileService = new Mock<UploadFileService>(mockIUploadFile.Object, mockTenantManager.Object, mockIAbpSession.Object);

            _workScope = Resolve<IWorkScope>();
            var mockIEmailSender = new Mock<IEmailSender>();
            var mockTimesheetConfig = new Mock<IOptions<TimesheetConfig>>();
            var mockEmailManager = new Mock<EmailManager>(_workScope, mockIEmailSender.Object, mockTimesheetConfig.Object);
            var mockContractManager = new Mock<ContractManager>(_workScope, mockUploadFileService.Object, mockEmailManager.Object);

            var mockBranchManager = new Mock<BranchManager>(_workScope);
            var mockLevelManager = new Mock<LevelManager>(_workScope);
            var mockBenefitManager = new Mock<BenefitManager>(_workScope);
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

            var mockProjectService = new Mock<ProjectService>(mockHttpClient.Object, mockIAbpSession.Object, mockIIocResolver.Object);

            var mockTimesheetWebService = new Mock<TimesheetWebService>(mockHttpClient.Object, mockIAbpSession.Object, mockIIocResolver.Object);
            var mockIMSWebService = new Mock<IMSWebService>(mockHttpClient.Object, mockIAbpSession.Object, mockIIocResolver.Object);
            var mockTalentWebService = new Mock<TalentWebService>(mockHttpClient.Object, mockIAbpSession.Object, mockIIocResolver.Object);
            var mockKomuService = new Mock<KomuService>(mockHttpClient.Object, mockIAbpSession.Object, configuration, mockIIocResolver.Object);

            var mockChangeEmployeeWorkingStatusManager = new Mock<ChangeEmployeeWorkingStatusManager>(
                _workScope,
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

            _historyManager = new HistoryManager(
                _workScope,
                mockBranchManager.Object,
                mockLevelManager.Object,
                mockChangeEmployeeWorkingStatusManager.Object);

            _historyManager.ObjectMapper = Resolve<IObjectMapper>();
        }


        //GetPayslipSalary
        [Fact]
        public void GetPayslipSalarys_ReturnRightItemSalary()
        {
            List<StandardSalaryDto> listExpected = new List<StandardSalaryDto>
            {
                new StandardSalaryDto
                {
                    Date = DateTime.Parse("2022-12-01 00:00:00"),
                    Salary =  7000000.0,
                }
            };

           WithUnitOfWork(() =>
            {
                long payslipId = 57333;
                var result = _historyManager.GetPayslipSalary(payslipId);

                Assert.Equal(listExpected.Count, result.Count);

                var expectedObj = listExpected.FirstOrDefault();
                Assert.Equal(expectedObj.Salary, result?.FirstOrDefault()?.Salary);
                Assert.Equal(expectedObj.Date, result?.FirstOrDefault()?.Date);
            });
        }


        [Fact]
        public void GetPayslipSalarys_WrongPayslipId_ReturnListEmpty()
        {

            WithUnitOfWork(() =>
            {
                long payslipId = 111111111111111;
                var result = _historyManager.GetPayslipSalary(payslipId);

                var ER = 0;
                Assert.Equal(ER, result.Count);
            });
        }


        //GetAllEmployeeSalaryHistory
        [Fact]
        public void GetAllEmployeeSalaryHistory_CheckRightListItem()
        {

            List<EmployeeSalaryHistoryDto> listEmployeeSalaryChangedHistories = new List<EmployeeSalaryHistoryDto>
            {
                  new EmployeeSalaryHistoryDto{
                    EmployeeId = 889,
                    FromSalary = 4000000.0,
                    ToSalary = 6800000.0,
                    FromUserType = Constants.Enum.HRMEnum.UserType.Internship,
                    ToUserType = Constants.Enum.HRMEnum.UserType.ProbationaryStaff,
                    FromJobPositionId = 47,
                    ToJobPositionId = 47,
                    FromLevelId = 314,
                    ToLevelId = 315,
                    CreationTime = DateTime.Parse("2022-12-19 14:05:58.1575870"),
                    ContractCode = "PHUC.LEHOANG/2/2023/HĐTV-NCC",
                    Note = null,
                    UpdatedUser = "admin admin",
                    ApplyDate = DateTime.Parse("2023-02-01 00:00:00"),
                    IsNotAllowToDelete = false,
                    HasContract = true,
                    Request = new ChangeRequestInfoDto
                    {
                        Id = 154,
                        Name = "Review intern đợt 1/2023",
                        Status = SalaryRequestStatus.Executed
                    },
                    Type = SalaryRequestType.Change
                  },
                 new EmployeeSalaryHistoryDto{
                    EmployeeId = 889,
                    FromSalary = 2000000.0,
                    ToSalary = 4000000.0,
                    FromUserType = Constants.Enum.HRMEnum.UserType.Internship,
                    ToUserType = Constants.Enum.HRMEnum.UserType.Internship,
                    FromJobPositionId = 47,
                    ToJobPositionId = 47,
                    FromLevelId = 313,
                    ToLevelId = 314,
                    CreationTime = DateTime.Parse("2022-12-19 13:59:44.982687"),
                    ContractCode = "PHUC.LEHOANG/1/2023/HĐĐT-NCC",
                    Note = null,
                    UpdatedUser = "admin admin",
                    ApplyDate = DateTime.Parse("2023-01-01 00:00:00"),
                    IsNotAllowToDelete = false,
                    HasContract = true,
                    Request = new ChangeRequestInfoDto
                    {
                        Id = 152,
                        Name = "Review intern đợt 12/2022",
                        Status = SalaryRequestStatus.Executed
                    },
                    Type = SalaryRequestType.Change
                 },
                 new EmployeeSalaryHistoryDto{
                    EmployeeId = 889,
                    FromSalary = 1000000.0,
                    ToSalary = 2000000.0,
                    FromUserType = Constants.Enum.HRMEnum.UserType.Internship,
                    ToUserType = Constants.Enum.HRMEnum.UserType.Internship,
                    FromJobPositionId = 47,
                    ToJobPositionId = 47,
                    FromLevelId = 312,
                    ToLevelId = 313,
                    CreationTime = DateTime.Parse("2022-12-15 16:28:58.310561"),
                    ContractCode = "PHUC.LEHOANG/12/2022/HĐĐT-NCC",
                    Note = null,
                    UpdatedUser = "admin admin",
                    ApplyDate = DateTime.Parse("2022-12-01 00:00:00"),
                    IsNotAllowToDelete = false,
                    HasContract = true,
                    Request = new ChangeRequestInfoDto
                    {
                        Id = 146,
                        Name = "Review intern từ timesheet đợt 11/2022",
                        Status = SalaryRequestStatus.Executed
                    },

                    Type = SalaryRequestType.Change,
                 },
                 new EmployeeSalaryHistoryDto{
                    EmployeeId = 889,
                    FromSalary = 1000000.0,
                    ToSalary = 1000000.0,
                    FromUserType = Constants.Enum.HRMEnum.UserType.Internship,
                    ToUserType = Constants.Enum.HRMEnum.UserType.Internship,
                    FromJobPositionId = 47,
                    ToJobPositionId = 47,
                    FromLevelId = 312,
                    ToLevelId = 312,
                    CreationTime = DateTime.Parse("2022-12-15 13:38:24.072519"),
                    ContractCode = "PHUC.LEHOANG/12/2022/HĐĐT-NCC",
                    Note = "Lương khởi tạo",
                    UpdatedUser = "admin admin",
                    ApplyDate = DateTime.Parse("2022-11-01 00:00:00"),
                    IsNotAllowToDelete = true,
                    HasContract = true,
                    Request = null,
                    Type = SalaryRequestType.Initial,
                 },
            };

            WithUnitOfWork(() =>
            {
                long employeeId = 889;
                List<EmployeeSalaryHistoryDto> AR = _historyManager.GetAllEmployeeSalaryHistory(employeeId);

                // Assert
                AR.Count.ShouldBeEquivalentTo(listEmployeeSalaryChangedHistories.Count);
            
                for (int i = 0; i < AR.Count; i++)
                {
                    var objAR = AR.Skip(i).Take(1).FirstOrDefault();
                    var objER = listEmployeeSalaryChangedHistories.Skip(i).Take(1).FirstOrDefault();

                    objAR?.EmployeeId.ShouldBeEquivalentTo(objER?.EmployeeId);
                    objAR?.FromSalary.ShouldBeEquivalentTo(objER?.FromSalary);
                    objAR?.ToSalary.ShouldBeEquivalentTo(objER?.ToSalary);
                    objAR?.FromUserType.ShouldBeEquivalentTo(objER?.FromUserType);
                    objAR?.ToUserType.ShouldBeEquivalentTo(objER?.ToUserType);
                    objAR?.FromJobPositionId.ShouldBeEquivalentTo(objER?.FromJobPositionId);
                    objAR?.ToJobPositionId.ShouldBeEquivalentTo(objER?.ToJobPositionId);
                    objAR?.FromLevelId.ShouldBeEquivalentTo(objER?.FromLevelId);
                    objAR?.ToLevelId.ShouldBeEquivalentTo(objER?.ToLevelId);
                    Assert.Equal(objER?.CreationTime.ToLongDateString(), objAR?.CreationTime.ToLongDateString());
                    objAR?.ContractCode.ShouldBeEquivalentTo(objER?.ContractCode);
                    objAR?.Note.ShouldBeEquivalentTo(objER?.Note);
                    objAR?.UpdatedUser.ShouldBeEquivalentTo(objER?.UpdatedUser);
                    objAR?.ApplyDate.ShouldBeEquivalentTo(objER?.ApplyDate);
                    objAR?.IsNotAllowToDelete.ShouldBeEquivalentTo(objER?.IsNotAllowToDelete);
                    objAR?.HasContract.ShouldBeEquivalentTo(objER?.HasContract);
                    objAR?.Type.ShouldBeEquivalentTo(objER?.Type);
                    objAR?.Request.ShouldBeEquivalentTo(objER?.Request);

                    if (i == AR.Count - 1)
                    {
                        objAR?.IsNotAllowToDelete.ShouldBeEquivalentTo(true);
                    }
                    else
                    {
                        objAR?.IsNotAllowToDelete.ShouldBeEquivalentTo(false);
                    }
                }
              
            });
        }

        [Fact]
        public void GetAllEmployeeSalaryHistory_WrongEmployeeId_ReturnListEmpty()
        {
            WithUnitOfWork(() =>
            {
                long employeeId = 111111111111;
                var result = _historyManager.GetAllEmployeeSalaryHistory(employeeId);

                result.Count.ShouldBeEquivalentTo(0);
            });
        }


        //GetAllEmployeePayslipHistory
        [Fact]
        public void GetAllEmployeePayslipHistory_CheckRightListItem()
        {
            List<EmployeePayslipHistoryDto> listEmployeePayslipHistories = new List<EmployeePayslipHistoryDto>
            {
        
                new EmployeePayslipHistoryDto{
                    Id = 57363,
                    EmployeeId = 889,
                    PayslipId = 57363,
                    RealSalary = 700000.0,
                    OTSalary = 0,
                    Punishment = 0,
                    RemainLeaveDayBefore = 0,
                    RemainLeaveDayAfter = 0,
                    UpdatedTime = DateTime.Parse("2022-12-19 14:11:30.567962"),
                    UpdatedUser = "admin admin",
                    ApplyMonth = DateTime.Parse("2023-02-01 00:00:00"),
                    Note = null,
                },
                new EmployeePayslipHistoryDto{
                    Id = 57353,
                    EmployeeId = 889,
                    PayslipId = 57353,
                    RealSalary = 6331304.0,
                    OTSalary = 0,
                    Punishment = -360000.0,
                    RemainLeaveDayBefore = 0,
                    RemainLeaveDayAfter = 0,
                    UpdatedTime = DateTime.Parse("2022-12-19  02:11:31 PM"),
                    UpdatedUser = "admin admin",
                    ApplyMonth = DateTime.Parse("2023-01-01 00:00:00"),
                    Note = null
                },
                new EmployeePayslipHistoryDto{
                    Id = 57259,
                    EmployeeId = 889,
                    PayslipId = 57259,
                    RealSalary = 3278260.0,
                    OTSalary = 0,
                    Punishment = 0,
                    RemainLeaveDayBefore = 0,
                    RemainLeaveDayAfter = 0,
                    UpdatedTime = DateTime.Parse("19/12/2022  10:36:23 AM"),
                    UpdatedUser = "admin admin",
                    ApplyMonth = DateTime.Parse("2022-12-01 00:00:00"),
                    Note = null,
                }
            };

            WithUnitOfWork(() =>
            {
                int employeeId = 889;
                var AR = _historyManager.GetAllEmployeePayslipHistory(employeeId);

                //Assert
                AR.Count.ShouldBeEquivalentTo(listEmployeePayslipHistories.Count);
                for (int i = 0; i < AR.Count; i++)
                {
                    var objAR = AR.Skip(i).Take(1).FirstOrDefault();
                    var objEX = listEmployeePayslipHistories.Skip(i).Take(1).FirstOrDefault();

                    objAR?.Id.ShouldBeEquivalentTo(objEX?.Id);
                    objAR?.EmployeeId.ShouldBeEquivalentTo(objEX?.EmployeeId);
                    objAR?.PayslipId.ShouldBeEquivalentTo(objEX?.PayslipId);
                    objAR?.RemainLeaveDayBefore.ShouldBeEquivalentTo(objEX?.RemainLeaveDayBefore);
                    objAR?.RemainLeaveDayAfter.ShouldBeEquivalentTo(objEX?.RemainLeaveDayAfter);
                    objAR?.OTSalary.ShouldBeEquivalentTo(objEX?.OTSalary);
                    objAR?.RealSalary.ShouldBeEquivalentTo(objEX?.RealSalary);
                    objAR?.Punishment.ShouldBeEquivalentTo(objEX?.Punishment);
                    objAR?.ApplyMonth.ShouldBeEquivalentTo(objEX?.ApplyMonth);
                    objAR?.UpdatedUser.ShouldBeEquivalentTo(objEX?.UpdatedUser);
                    objAR?.Note.ShouldBeEquivalentTo(objEX?.Note);
                    Assert.Equal(objEX?.UpdatedTime?.ToLongDateString(), objAR?.UpdatedTime?.ToLongDateString());
                }
            });
        }

        [Fact]
        public void GetAllEmployeePayslipHistory_WrongEmployeeId_ReturnListEmpty()
        {

            WithUnitOfWork(() =>
            {
                int employeeId = 0;
                var result = _historyManager.GetAllEmployeePayslipHistory(employeeId);

                result.Count.ShouldBeEquivalentTo(0);
            });
        }


        //GetAllEmployeeBranchHistory
        [Fact]
        public void GetAllEmployeeBranchHistory_ReturnListItem()
        {

            List<EmployeeBranchHistoryDto> listEmployeeBranchChangedHistories = new List<EmployeeBranchHistoryDto>
            {
                 new EmployeeBranchHistoryDto {
                    EmployeeId = 889,
                    BranchId = 93,
                 }
            };


            WithUnitOfWork(() =>
            {
                int employeeId = 889;
                var AR = _historyManager.GetAllEmployeeBranchHistory(employeeId);

                AR.Count.ShouldBeEquivalentTo(listEmployeeBranchChangedHistories.Count);
                for (int i = 0; i < AR.Count; i++)
                {
                    var objAR = AR.Skip(i).Take(1).FirstOrDefault();
                    var objER = listEmployeeBranchChangedHistories.Skip(i).Take(1).FirstOrDefault();
                    objAR?.BranchId.ShouldBeEquivalentTo(objER?.BranchId);
                    objAR?.EmployeeId.ShouldBeEquivalentTo(objER?.EmployeeId);

                    if (i == AR.Count - 1)
                    {
                        objAR?.IsNotAllowToDelete.ShouldBeEquivalentTo(true);
                    }
                    else
                    {
                        objAR?.IsNotAllowToDelete.ShouldBeEquivalentTo(false);
                    }
                }
            });
        }

        [Fact]
        public void GetAllEmployeeBranchHistory_WrongEmployeeId_ReturnListEmpty()
        {

            WithUnitOfWork(() =>
            {
                int employeeId = 0;
                var result = _historyManager.GetAllEmployeeBranchHistory(employeeId);

                result.Count.ShouldBeEquivalentTo(0);
            });
        }


        //GetAllEmployeeWorkingHistory
        [Fact]
        public void GetAllEmployeeWorkingHistory_ReturnListItem()
        {

            List<EmployeeWorkingHistory> listEmployeeWorkingHistories = new List<EmployeeWorkingHistory>
            {
                 new EmployeeWorkingHistory {
                    EmployeeId = 893,
                    DateAt = DateTime.Parse("2022-12-10 00:00:00"),
                 },
                 new EmployeeWorkingHistory {
                    EmployeeId = 893,
                    DateAt = DateTime.Parse("2022-10-01 00:00:00"),
                 }

            };


            WithUnitOfWork(() =>
            {
                int employeeId = 893;
                var AR = _historyManager.GetAllEmployeeWorkingHistory(employeeId);

                AR.Count.ShouldBeEquivalentTo(listEmployeeWorkingHistories.Count);
                for (int i = 0; i < AR.Count; i++)
                {
                    var objAR = AR.Skip(i).Take(1).FirstOrDefault();
                    var objER = listEmployeeWorkingHistories.Skip(i).Take(1).FirstOrDefault();
                    objAR?.EmployeeId.ShouldBeEquivalentTo(objER?.EmployeeId);
                    objAR?.DateAt.ShouldBeEquivalentTo(objER?.DateAt);

                    if (i == AR.Count - 1)
                    {
                        objAR?.IsNotAllowToDelete.ShouldBeEquivalentTo(true);
                    }
                    else
                    {
                        objAR?.IsNotAllowToDelete.ShouldBeEquivalentTo(false);
                    }
                }
            });
        }

        [Fact]
        public void GetAllEmployeeWorkingHistory_WrongEmployeeId_ReturnListEmpty()
        {

            WithUnitOfWork(() =>
            {
                int employeeId = 0;
                var result = _historyManager.GetAllEmployeeWorkingHistory(employeeId);

                result.Count.ShouldBeEquivalentTo(0);
            });
        }


        //CreateWorkingHistory
        [Fact]
        public async Task CreateWorkingHistory_Success()
        {
            List<EmployeeWorkingHistory> workingHistories;
            var countBef = 0;
            await WithUnitOfWorkAsync(async () =>
            {
                workingHistories = _workScope.GetAll<EmployeeWorkingHistory>().ToList();
                countBef = workingHistories.Count;

                CreateWorkingHistoryDto createWorkingHistory = new CreateWorkingHistoryDto()
                {
                    EmployeeId = 1,
                    Status = EmployeeStatus.Working,
                    Note = "Initial",
                    DateAt = DateTime.Now
                };

                _historyManager.CreateWorkingHistory(createWorkingHistory);
            });

            WithUnitOfWork(() =>
            {
                workingHistories = _workScope.GetAll<EmployeeWorkingHistory>().ToList();
                var countAft = workingHistories.Count;
                countAft.ShouldBeEquivalentTo(countBef + 1);
            });
        }


        //CreateBranchHistory
        [Fact]
        public async Task CreateBranchHistory_Success()
        {
            List<EmployeeBranchHistory> workingHistories;
            var countBef = 0;
            await WithUnitOfWorkAsync(async () =>
            {
                workingHistories = _workScope.GetAll<EmployeeBranchHistory>().ToList();
                countBef = workingHistories.Count;

                CreateBranchHistoryDto createBranchHistory = new CreateBranchHistoryDto()
                {
                    BranchId = 94,
                    Note = "",
                    DateAt = DateTime.Now

                };

                _historyManager.CreateBranchHistory(createBranchHistory);
            });

            WithUnitOfWork(() =>
            {
                workingHistories = _workScope.GetAll<EmployeeBranchHistory>().ToList();
                var countAft = workingHistories.Count;
                countAft.ShouldBeEquivalentTo(countBef + 1);
            });
        }


        //DeleteSalaryHistory
        [Fact]
        public async Task DeleteSalaryHistory_WrongId_ReturnUserFriendlyException()
        {

            await WithUnitOfWorkAsync(async () =>
              {
                  long id = 1000000;

                  var exception = await Assert.ThrowsAsync<UserFriendlyException>(async () =>
                  {
                      await _historyManager.DeleteSalaryHistory(id);
                  });


                  var expectErrorMsg = "Not found SalaryChangeRequestEmployee with Id {id}";
                  Assert.Equal(exception.Message, expectErrorMsg);

              });
        }

        [Fact]
        public async Task DeleteSalaryHistory_TypeInitial_ReturnUserFriendlyException()
        {

            await WithUnitOfWorkAsync(async () =>
            {
                long id = 1603;

                var exception = await Assert.ThrowsAsync<UserFriendlyException>(async () =>
                {
                    await _historyManager.DeleteSalaryHistory(id);
                });

                var expectErrorMsg = "Can't delete change request employee with type Initial";
                Assert.Equal(exception.Message, expectErrorMsg);

            });
        }

        [Fact]
        public async Task DeleteSalaryHistory_Success()
        {
            List<SalaryChangeRequestEmployee> empSalaryChangeHistories;
            var countBef = 0;
            await WithUnitOfWorkAsync(async () =>
            {
                long id = 1623;
                empSalaryChangeHistories = _workScope.GetAll<SalaryChangeRequestEmployee>().ToList();
                countBef = empSalaryChangeHistories.Count;

                long result = await _historyManager.DeleteSalaryHistory(id);
                result.ShouldBeEquivalentTo(id);

            });

            WithUnitOfWork(() =>
            {
                empSalaryChangeHistories = _workScope.GetAll<SalaryChangeRequestEmployee>().ToList();
                var countAft = empSalaryChangeHistories.Count;

                countAft.ShouldBeEquivalentTo(countBef - 1);
            });
        }


        //DeleteEmployeeContractByChangeRequestEmployeeId
        [Fact]
        public async Task DeleteEmployeeContractByChangeRequestEmployeeId_WrongId_NoDelete()
        {
            List<EmployeeContract> employeeContracts;
            var countBef = 0;

            await WithUnitOfWorkAsync(async () =>
            {
                long id = 11111111111;

                employeeContracts = _workScope.GetAll<EmployeeContract>().ToList();
                countBef = employeeContracts.Count;
                await _historyManager.DeleteEmployeeContractByChangeRequestEmployeeId(id);
            });

            WithUnitOfWork(() =>
            {
                employeeContracts = _workScope.GetAll<EmployeeContract>().ToList();
                var countAft = employeeContracts.Count;

                countAft.ShouldBeEquivalentTo(countBef);

            });
        }


        [Fact]
        public async Task DeleteEmployeeContractByChangeRequestEmployeeId_Success()
        {
            List<EmployeeContract> employeeContracts;
            var countBef = 0;

            await WithUnitOfWorkAsync(async () =>
            {
                long id = 1602;

                employeeContracts = _workScope.GetAll<EmployeeContract>().ToList();
                countBef = employeeContracts.Count;
                await _historyManager.DeleteEmployeeContractByChangeRequestEmployeeId(id);
            });

            WithUnitOfWork(() =>
            {
                employeeContracts = _workScope.GetAll<EmployeeContract>().ToList();
                var countAft = employeeContracts.Count;

                countAft.ShouldBeEquivalentTo(countBef - 1);

            });
        }


        //DeleteBranchHistory
        [Fact]
        public async Task DeleteBranchHistory_WrongId_EntityNotFoundException()
        {
            await WithUnitOfWorkAsync(async () =>
            {
                long id = 11111111111;

                await Assert.ThrowsAsync<EntityNotFoundException>(async () =>
                {
                    await _historyManager.DeleteBranchHistory(id);
                });
            });
        }


        [Fact]
        public async Task DeleteBranchHistory_RightId_Success()
        {

            List<EmployeeBranchHistory> employeeBranchHistories;
            var countBef = 0;
            await WithUnitOfWorkAsync(async () =>
            {
                long id = 885;
                employeeBranchHistories = _workScope.GetAll<EmployeeBranchHistory>().ToList();
                countBef = employeeBranchHistories.Count;
                long result = await _historyManager.DeleteBranchHistory(id);

                result.ShouldBeEquivalentTo(id);
            });

            WithUnitOfWork(() =>
            {
                employeeBranchHistories = _workScope.GetAll<EmployeeBranchHistory>().ToList();
                var countAft = employeeBranchHistories.Count;

                countAft.ShouldBeEquivalentTo(countBef - 1);
            });
        }


        //DeleteWorkingHistory
        [Fact]
        public async Task DeleteWorkingHistory_WrongId_EntityNotFoundException()
        {
            await WithUnitOfWorkAsync(async () =>
            {
                long id = 11111111111;
                long employeeId = 111111;

                await Assert.ThrowsAsync<EntityNotFoundException>(async () =>
                {
                    await _historyManager.DeleteWorkingHistory(id, employeeId);
                });
            });
        }


        [Fact]
        public async Task DeleteWorkingHistory_WrongEmployeeId_NullReferenceException()
        {
            await WithUnitOfWorkAsync(async () =>
            {
                long id = 1300;
                long employeeId = 1111111111111;

                await Assert.ThrowsAsync<NullReferenceException>(async () =>
                {
                    await _historyManager.DeleteWorkingHistory(id, employeeId);
                });

            });
        }

        [Fact]
        public async Task DeleteWorkingHistory_RightId_Success()
        {

            List<EmployeeWorkingHistory> employeeWorkingHistories;
            var countBef = 0;
            await WithUnitOfWorkAsync(async () =>
            {
                employeeWorkingHistories = _workScope.GetAll<EmployeeWorkingHistory>().ToList();
                countBef = employeeWorkingHistories.Count;
                long id = 1300;
                long employeeId = 881;

                long result = await _historyManager.DeleteWorkingHistory(id, employeeId);
                result.ShouldBeEquivalentTo(id);

            });

            WithUnitOfWork(() =>
            {
                employeeWorkingHistories = _workScope.GetAll<EmployeeWorkingHistory>().ToList();
                var countAft = employeeWorkingHistories.Count;
                countAft.ShouldBeEquivalentTo(countBef - 1);
            });
        }


        //UpdateEmployeeWhenDeleteSalaryHistory
        [Fact]
        public async Task UpdateEmployeeWhenDeleteSalaryHistory_WrongId_UserFriendlyException()
        {
            await WithUnitOfWorkAsync(async () =>
            {
                long requestId = 111111111;
                long employeeId = 111111111;

                var exception = await Assert.ThrowsAsync<UserFriendlyException>(async () =>
                {
                    await _historyManager.UpdateEmployeeWhenDeleteSalaryHistory(requestId, employeeId);
                });

                var expectErrorMsg = "Can't found salary change request employee to update employee";
                Assert.Equal(exception.Message, expectErrorMsg);
            });
        }

        [Fact]
        public async Task UpdateEmployeeWhenDeleteSalaryHistory_NotStopWorkType_Success()
        {

            long requestId = 1602;
            long employeeId = 880;


            await WithUnitOfWorkAsync(async () =>
            {
                await _historyManager.UpdateEmployeeWhenDeleteSalaryHistory(requestId, employeeId);
            });

            WithUnitOfWork(() =>
            {
                var employeeContract = _workScope.GetAll<EmployeeContract>()
                    .Where(x => x.EmployeeId == employeeId && x.SalaryRequestEmployeeId != requestId)
                    .OrderByDescending(x => x.StartDate)
                    .ThenByDescending(x => x.CreationTime)
                    .FirstOrDefault();
                var employee = _workScope.GetAsync<Employee>(employeeId).Result;

                employee.ProbationPercentage.ShouldBe(employeeContract.ProbationPercentage);
                employee.Salary.ShouldBe(employeeContract.BasicSalary);
            });
        }

        [Fact]
        public async Task UpdateEmployeeWhenDeleteSalaryHistory_StopWorkType_Success()
        {

            long requestId = 1624;
            long employeeId = 898;

            await WithUnitOfWorkAsync(async () =>
            {
                await _historyManager.UpdateEmployeeWhenDeleteSalaryHistory(requestId, employeeId);
            });

            WithUnitOfWork(() =>
            {
                var employee = _workScope.GetAsync<Employee>(employeeId).Result;

                employee.ProbationPercentage.ShouldBe(0);
                employee.Salary.ShouldBe(0);
            });
        }


        //UpdateEmployeeStatus
        [Fact]
        public async Task UpdateEmployeeStatus_WrongId_AggregateException()
        {
            await WithUnitOfWorkAsync(async () =>
            {
                long id = 111111111;
                long employeeId = 111111111;

                await Assert.ThrowsAsync<AggregateException>(async () =>
                {
                    await _historyManager.UpdateEmployeeStatus(employeeId, id);
                });
            });
        }

        [Fact]
        public async Task UpdateEmployeeStatus_RightId_Success()
        {
            long id = 1316;
            long employeeId = 894;

            await WithUnitOfWorkAsync(async () =>
            {
                var result = await _historyManager.UpdateEmployeeStatus(employeeId, id);
                result.ShouldBe(employeeId);
            });
        }


        //UpdateNoteInBranchHistory
        [Fact]
        public async Task UpdateNoteInBranchHistory_WrongId_AggregateException()
        {

            UpdateNoteBranchHistoryDto input = new UpdateNoteBranchHistoryDto()
            {
                Id = 11111111111,
            };


            await Assert.ThrowsAsync<AggregateException>(async () =>
            {
                await _historyManager.UpdateNoteInBranchHistory(input);
            });
        }

        [Fact]
        public async Task UpdateNoteInBranchHistory_RightId_Success()
        {
            UpdateNoteBranchHistoryDto input = new UpdateNoteBranchHistoryDto();
            input.Id = 885;
            input.Note = "Note will be changed ok";

            await WithUnitOfWorkAsync(async () =>
            {
                await _historyManager.UpdateNoteInBranchHistory(input);
            });

            WithUnitOfWork(() =>
            {
                var entity = _workScope.GetAsync<EmployeeBranchHistory>(input.Id).Result;
                Assert.Equal(input.Note, entity.Note);
                Assert.Equal(input.Id, entity.Id);
            });
        }

        //UpdateNoteInBranchHistory
        [Fact]
        public async Task UpdateNoteInWorkingHistory_WrongId_AggregateException()
        {

            UpdateNoteWorkingHistoryDto input = new UpdateNoteWorkingHistoryDto()
            {
                Id = 11111111111,
            };


            await Assert.ThrowsAsync<AggregateException>(async () =>
            {
                await _historyManager.UpdateNoteInWorkingHistory(input);
            });
        }

        [Fact]
        public async Task UpdateNoteInWorkingHistory_RightId_Success()
        {
            UpdateNoteWorkingHistoryDto input = new UpdateNoteWorkingHistoryDto();
            input.Id = 1300;
            input.Note = "UpdateNoteInWorkingHistory - Note will be changed ok";

            await WithUnitOfWorkAsync(async () =>
            {
                await _historyManager.UpdateNoteInWorkingHistory(input);
            });

            WithUnitOfWork(() =>
            {
                var entity = _workScope.GetAsync<EmployeeWorkingHistory>(input.Id).Result;
                Assert.Equal(input.Note, entity.Note);
                Assert.Equal(input.Id, entity.Id);
            });
        }

        //UpdateNoteInSalaryHistory
        [Fact]
        public async Task UpdateNoteInSalaryHistory_WrongId_AggregateException()
        {

            UpdateNoteWorkingHistoryDto input = new UpdateNoteWorkingHistoryDto()
            {
                Id = 11111111111,
            };

            await Assert.ThrowsAsync<AggregateException>(async () =>
            {
                await _historyManager.UpdateNoteInSalaryHistory(input);
            });
        }

        [Fact]
        public async Task UpdateNoteInSalaryHistory_RightId_Success()
        {
            UpdateNoteWorkingHistoryDto input = new UpdateNoteWorkingHistoryDto();
            input.Id = 1602;
            input.Note = "UpdateNoteInSalaryHistory - Note will be changed ok";

            await WithUnitOfWorkAsync(async () =>
            {
                await _historyManager.UpdateNoteInSalaryHistory(input);
            });

            WithUnitOfWork(() =>
            {
                var entity = _workScope.GetAsync<SalaryChangeRequestEmployee>(input.Id).Result;
                Assert.Equal(input.Note, entity.Note);
                Assert.Equal(input.Id, entity.Id);
            });
        }
    }
}
