using HRMv2.Manager.ChangeEmployeeWorkingStatuses;
using HRMv2.Manager.EmployeeContracts;
using HRMv2.Manager.Histories;
using HRMv2.Manager.Punishments;
using HRMv2.Manager.Punishments.Dto;
using HRMv2.Manager.SalaryRequests;
using HRMv2.UploadFileServices;
using HRMv2.WebServices.IMS;
using HRMv2.WebServices.Komu;
using HRMv2.WebServices.Project;
using HRMv2.WebServices.Talent;
using HRMv2.WebServices.Timesheet;
using Xunit;
using Abp.Dependency;
using Abp.BackgroundJobs;
using Abp.Net.Mail;
using Abp.Runtime.Session;
using Abp.Threading.Timers;
using HRMv2.Manager.Notifications.Email;
using HRMv2.NccCore;
using Microsoft.Extensions.Options;
using Moq;
using HRMv2.Manager.Employees;
using HRMv2.MultiTenancy;
using Abp.Application.Features;
using Abp.Domain.Repositories;
using Abp.MultiTenancy;
using HRMv2.Editions;
using Abp.Application.Editions;
using Abp.Domain.Uow;
using HRMv2.Manager.Categories;
using HRMv2.Manager.Categories.Levels;
using HRMv2.Manager.Categories.Benefits;
using HRMv2.Entities;
using Microsoft.Extensions.Configuration;
using Abp.Configuration;
using HRMv2.Manager.Categories.JobPositions;
using HRMv2.Manager.Categories.UserTypes;
using NccCore.Paging;
using Abp.UI;
using Abp.ObjectMapping;
using Abp.Domain.Entities;
using static HRMv2.Constants.Enum.HRMEnum;
using Microsoft.AspNetCore.Http;
using Shouldly;
using SortDirection = NccCore.Paging.SortDirection;

namespace HRMv2.Core.Tests.Managers.Punishments
{
    public class PunishmentManager_Tests : HRMv2CoreTestBase
    {
        private readonly PunishmentManager _punishmentManager;
        private readonly IWorkScope _workScope;

        public PunishmentManager_Tests()
        {
            _workScope = Resolve<IWorkScope>();

            var moqHttpClient = new Mock<HttpClient>();
            var moqAbpSession = new Mock<IAbpSession>();
            var moqIocResolver = new Mock<IIocResolver>();

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
            var moqEdition = new Mock<IRepository<Edition>>();
            var moqFeatureValueStore = new Mock<IAbpZeroFeatureValueStore>();
            var moqUnitOfWorkManager = new Mock<IUnitOfWorkManager>();

            var moqEditionManager = new Mock<EditionManager>(moqEdition.Object, moqFeatureValueStore.Object, moqUnitOfWorkManager.Object); ;

            // 1.2. TenantManager
            var moqTenant = new Mock<IRepository<Tenant>>();
            var moqTenantFeatureSetting = new Mock<IRepository<TenantFeatureSetting, long>>();

            var moqTenantManager = new Mock<TenantManager>(moqTenant.Object, moqTenantFeatureSetting.Object, moqEditionManager.Object, moqFeatureValueStore.Object);

            // 1.3. UploadFileService
            var moqUploadFile = new Mock<IUploadFileService>();

            var moqUploadFileService = new Mock<UploadFileService>(moqUploadFile.Object, moqTenantManager.Object, moqAbpSession.Object);

            /* == 2. CONTRACT MANAGER == */
            // 2.1. EmailManager
            var moqemailSender = new Mock<IEmailSender>();
            var moqTSconfig = new Mock<IOptions<TimesheetConfig>>();
            var moqEmailManager = new Mock<EmailManager>(_workScope, moqemailSender.Object, moqTSconfig.Object);

            // 2.2. ContractManager
            var moqContractManager = new Mock<ContractManager>(_workScope, moqUploadFileService.Object, moqEmailManager.Object);

            /* == 3. HISTORY MANAGER */
            // 3.1. BranchManager
            var moqBranchManager = new Mock<BranchManager>(_workScope);

            // 3.2. LevelManager
            var moqLevelManager = new Mock<LevelManager>(_workScope);

            // 3.3. ChangeEmployeeWorkingStatusManager 
            var moqBenefitManager = new Mock<BenefitManager>(_workScope);
            var moqIBackgroundJobManager = new Mock<IBackgroundJobManager>();
            var moqEmployee = new Mock<IRepository<Employee, long>>();
            var moqEmployeeWorkingHistory = new Mock<IRepository<EmployeeWorkingHistory, long>>();
            var moqSalaryChangeRequestEmployee = new Mock<IRepository<SalaryChangeRequestEmployee, long>>();
            var moqBenefitEmployee = new Mock<IRepository<BenefitEmployee, long>>();
            var moqBackgroundJobInfo = new Mock<IRepository<BackgroundJobInfo, long>>();
            var moqEmployeeContract = new Mock<IRepository<EmployeeContract, long>>();
            var moqProjectService = new Mock<ProjectService>(moqHttpClient.Object, moqAbpSession.Object, moqIocResolver.Object);
            var moqTimesheetWebService = new Mock<TimesheetWebService>(moqHttpClient.Object, moqAbpSession.Object, moqIocResolver.Object);
            var moqIMSWebService = new Mock<IMSWebService>(moqHttpClient.Object, moqAbpSession.Object, moqIocResolver.Object);
            var moqTalentWebService = new Mock<TalentWebService>(moqHttpClient.Object, moqAbpSession.Object, moqIocResolver.Object);
            var moqKomuService = new Mock<KomuService>(moqHttpClient.Object, moqAbpSession.Object, configuration, moqIocResolver.Object);
            var moqSettingManager = new Mock<ISettingManager>();

            var moqChangeEmployeeWorkingStatusManager
                = new Mock<ChangeEmployeeWorkingStatusManager>(_workScope, moqBenefitManager.Object, moqContractManager.Object,
                moqIBackgroundJobManager.Object, moqEmployee.Object, moqEmployeeWorkingHistory.Object, moqSalaryChangeRequestEmployee.Object,
                moqBenefitEmployee.Object, moqBackgroundJobInfo.Object, moqEmployeeContract.Object, moqProjectService.Object, moqTimesheetWebService.Object,
                moqIMSWebService.Object, moqTalentWebService.Object, moqKomuService.Object, moqSettingManager.Object);

            // 3.4. HistoryManager
            var moqHistoryManager = new Mock<HistoryManager>(_workScope, moqBranchManager.Object, moqLevelManager.Object, moqChangeEmployeeWorkingStatusManager.Object);

            /* == 4. SALARY REQUEST MANAGER */
            var moqJobPositionManager = new Mock<JobPositionManager>(_workScope);

            var moqIBackgroundJobStore = new Mock<IBackgroundJobStore>();
            var moqAbpAsyncTimer = new Mock<AbpAsyncTimer>();
            var moqBackgroundJobManager = new Mock<BackgroundJobManager>(moqIocResolver.Object, moqIBackgroundJobStore.Object, moqAbpAsyncTimer.Object);

            var moqSalaryRequestManager = new Mock<SalaryRequestManager>(moqLevelManager.Object, moqJobPositionManager.Object, moqContractManager.Object, _workScope, moqEmailManager.Object, moqBackgroundJobManager.Object);

            /* == 5. USER TYPE MANAGER */
            var moqUserTypeManager = new Mock<UserTypeManager>(_workScope);

            /* == 6. EMPLOYEE MANAGER */
            var moqEmployeeManager = new Mock<EmployeeManager>(moqUploadFileService.Object, moqContractManager.Object, moqHistoryManager.Object, moqProjectService.Object, moqTimesheetWebService.Object, moqIMSWebService.Object, moqTalentWebService.Object, _workScope, moqSalaryRequestManager.Object, moqBenefitManager.Object, moqUserTypeManager.Object, moqChangeEmployeeWorkingStatusManager.Object, moqIBackgroundJobManager.Object, moqBackgroundJobInfo.Object);

            /* == 7. PUNISHMENT MANAGER */
            _punishmentManager = new PunishmentManager(_workScope, moqEmployeeManager.Object);
            _punishmentManager.ObjectMapper = LocalIocManager.Resolve<IObjectMapper>();
        }

        [Fact]
        public async Task GetAllPaging_Test1()
        {
            var inputTest = new GridParam
            {
                SkipCount = 3,
                MaxResultCount = 10,
            };

            var expectedTotalCount = 7;
            var expectedItemsCount = 4;

            await WithUnitOfWorkAsync(async() =>
            {
                var result = await _punishmentManager.GetAllPaging(inputTest);

                result.TotalCount.ShouldBe(expectedTotalCount);
                result.Items.Count.ShouldBe(expectedItemsCount);
                result.Items.ShouldContain(punishment => punishment.Id == 53);
                result.Items.ShouldContain(punishment => punishment.Id == 54);
                result.Items.ShouldContain(punishment => punishment.Id == 55);
            });
        }

        [Fact]
        public async Task GetAllPaging_Test2()
        {
            var searchText = "muộn";
            var filter = new GridParam
            {
                SearchText = searchText,
            };
            var expectedTotalCount = 3;
            var expectedItemsCount = 3;

            await WithUnitOfWorkAsync(async() =>
            {
                var result = await _punishmentManager.GetAllPaging(filter);

                result.TotalCount.ShouldBe(expectedTotalCount);
                result.Items.Count.ShouldBe(expectedItemsCount);
                result.Items.ShouldContain(punishment => punishment.Id == 55);
                result.Items.ShouldContain(punishment => punishment.Id == 57);
            });
        }

        [Fact]
        public void GetAll_Test1()
        {
            var expectedPunishmentCount = 7;
            WithUnitOfWork(() =>
            {
                var result = _punishmentManager.GetAll();

                result.Count.ShouldBe(expectedPunishmentCount);
                result.ShouldContain(punishment => punishment.Id == 53);
                result.ShouldContain(punishment => punishment.Name == "Phạt sao đỏ");
                result.ShouldContain(punishment => punishment.Date == new DateTime(2022, 12, 1));
                result.ShouldContain(punishment => punishment.IsActive == false);
                result.ShouldContain(punishment => punishment.PunishmentTypeId == 12);
            });
        }

        [Fact]
        public async Task GetPunishmentByEmployeeId_Test1()
        {
            // Standard test case
            var employeeId = 897;
            var skipCount = 1;
            var filter = new GridParam
            {
                SkipCount = skipCount,
            };

            var expectedTotalCount = 4;
            var expectedItemsCount = 3;

            await WithUnitOfWorkAsync(async() =>
            {
                var result = await _punishmentManager.GetPunishmentByEmployeeId(employeeId, filter);

                Assert.Equal(result.TotalCount - skipCount, result.Items.Count);
                result.TotalCount.ShouldBe(expectedTotalCount);
                result.Items.Count.ShouldBe(expectedItemsCount);
                result.Items.ShouldContain(punishment => punishment.PunishmentId == 55);
                result.Items.ShouldContain(punishment => punishment.PunishmentId == 57);
                result.Items.ShouldContain(punishment => punishment.PunishmentId == 58);
            });
        }

        [Fact]
        public async Task GetPunishmentByEmployeeId_Test2()
        {
            // Standard test case
            var employeeId = 897;
            var searchText = "muộn";
            var filter = new GridParam
            {
                SearchText = searchText,
            };

            var expectedTotalCount = 2;
            var expectedItemsCount = 2;

            await WithUnitOfWorkAsync(async () =>
            {
                var result = await _punishmentManager.GetPunishmentByEmployeeId(employeeId, filter);

                result.TotalCount.ShouldBe(expectedTotalCount);
                result.Items.Count.ShouldBe(expectedItemsCount);
                result.Items.ShouldContain(punishment => punishment.PunishmentId == 55);
                result.Items.ShouldContain(punishment => punishment.PunishmentId == 57);
            });
        }

        [Fact]
        public async Task Create_Test1()
        {
            // Standard test case
            var expectedPunishment = new CreatePunishmentDto
            {
                Id = 60,
                Name = "Quên checkin và checkout 1-2023",
                Date = new DateTime(2023, 1, 1),
                IsActive = true,
            };
            var allPunishmentBeforeCreate = new List<Punishment>();

            await WithUnitOfWorkAsync(async() =>
            {
                allPunishmentBeforeCreate = _workScope.GetAll<Punishment>().ToList();

                var result = await _punishmentManager.Create(expectedPunishment);

                result.Id.ShouldBeGreaterThan(allPunishmentBeforeCreate.Last().Id);
                result.Id.ShouldBe(expectedPunishment.Id);
                result.Name.ShouldBe(expectedPunishment.Name);
                result.Date.ShouldBe(expectedPunishment.Date);
                result.IsActive.ShouldBe(expectedPunishment.IsActive);
            });

            await WithUnitOfWorkAsync(async () =>
            {
                var allPunishmentsAfterCreate = _workScope.GetAll<Punishment>();
                var newPunishment = await _workScope.GetAsync<Punishment>(expectedPunishment.Id);

                allPunishmentsAfterCreate.Count().ShouldBeGreaterThan(allPunishmentBeforeCreate.Count);
                newPunishment.Name.ShouldBe(expectedPunishment.Name);
                newPunishment.Date.ShouldBe(expectedPunishment.Date);
                newPunishment.IsActive.ShouldBe(expectedPunishment.IsActive);
            });
        }

        [Fact]
        public async Task Create_Test2()
        {
            // Existed Name
            var input = new CreatePunishmentDto
            {
                Id = 60,
                Name = "phạt tháng 12",
                Date = new DateTime(2023, 1, 1),
                IsActive = false,
            };
            var expectedMessage = "Name is already exist";

            await WithUnitOfWorkAsync(async() =>
            {
                var exception = await Assert.ThrowsAsync<UserFriendlyException>(async() =>
                {
                    await _punishmentManager.Create(input);
                });

                Assert.Equal(expectedMessage, exception.Message);
            });
        }

        [Fact]
        public async Task GeneratePunishmentsByPunishmentType_Test1()
        {
            // Standard test case
            var input = new GeneratePunishmentDto
            {
                PunishmentTypeIds = new List<long> { 11 },
                Date = new DateTime(2022, 12, 1)
            };

            await WithUnitOfWorkAsync(async() =>
            {
                var result = await _punishmentManager.GeneratePunishmentsByPunishmentType(input);

                Assert.Equal(input.PunishmentTypeIds.Count, result.Count);

                return Task.CompletedTask;
            });
        }

        [Fact]
        public async Task Update_Test1()
        {
            // Standard test case
            var input = new UpdatePunishmentDto
            {
                Id = 53,
                Name = "Không tập thể dục 12-2022",
                Date = DateTime.Now,
                IsActive = true,
                isAbleUpdateNote = false
            };

            await WithUnitOfWorkAsync(async () =>
            {
                var result = await _punishmentManager.Update(input);

                result.Id.ShouldBe(input.Id);
                result.Name.ShouldBe(input.Name);
                result.Date.ShouldBe(input.Date);
                result.IsActive.ShouldBe(input.IsActive);
            });

            await WithUnitOfWorkAsync(async() =>
            {
                var punishment = await _workScope.GetAsync<Punishment>(input.Id);

                punishment.Name.ShouldBe(input.Name);
                punishment.Date.ShouldBe(input.Date);
                punishment.IsActive.ShouldBe(input.IsActive);
            });
        }

        [Fact]
        public async Task Update_Test2()
        {
            // Existed Name
            var input = new UpdatePunishmentDto
            {
                Id = 53,
                Name = "Phạt đi muộn 01/2023",
                Date = new DateTime(2023, 1, 1),
                IsActive = true,
                isAbleUpdateNote = false
            };
            var expectedMessage = $"This Punishment is Already Exist";

            await WithUnitOfWorkAsync(async() =>
            {
                var exception = await Assert.ThrowsAsync<UserFriendlyException>(async () =>
                {
                    await _punishmentManager.Update(input);
                });

                Assert.Equal(expectedMessage, exception.Message);
            });
        }

        [Fact]
        public async Task Delete_Test1()
        {
            // Standard test case
            var punishmentId = 53;

            var allPunishmentsBeforeDelete = new List<Punishment>();

            await WithUnitOfWorkAsync(async() =>
            {
                allPunishmentsBeforeDelete = _workScope.GetAll<Punishment>().ToList();
                var result = await _punishmentManager.Delete(punishmentId);

                Assert.Equal(punishmentId, result);
            });

            await WithUnitOfWorkAsync(async () =>
            {
                var allPunishmentsAfterDelete = _workScope.GetAll<Punishment>();
                allPunishmentsAfterDelete.Count().ShouldBeLessThan(allPunishmentsBeforeDelete.Count);
            });
        }

        [Fact]
        public async Task Delete_Test2()
        {
            // Non-existent Id
            var punishmentId = 1;

            await WithUnitOfWorkAsync(async () =>
            {
                await Assert.ThrowsAsync<EntityNotFoundException>(async () =>
                {
                    await _punishmentManager.Delete(punishmentId);
                });
            });
        }

        [Fact]
        public async Task ChangeStatus_Test1()
        {
            // Standard test case
            var expectedPunishment = new Punishment
            {
                Id = 53,
                IsActive = true
            };

            await WithUnitOfWorkAsync(async() =>
            {
                var result = await _punishmentManager.ChangeStatus(expectedPunishment.Id);

                Assert.Equal(expectedPunishment.Id, result);
            });

            await WithUnitOfWorkAsync(async () =>
            {
                var punishment = await _workScope.GetAsync<Punishment>(expectedPunishment.Id);

                punishment.IsActive.ShouldBe(expectedPunishment.IsActive);
            });
        }

        [Fact]
        public async Task ChangeStatus_Test2()
        {
            // Non-existent punishment
            var expectedPunishment = new Punishment
            {
                Id = 1,
                IsActive = true
            };

            await WithUnitOfWorkAsync(async () =>
            {
                await Assert.ThrowsAsync<EntityNotFoundException>(async () =>
                {
                    await _punishmentManager.ChangeStatus(expectedPunishment.Id);
                });
            });
        }

        [Fact]
        public async Task IsPunishmentHasEmployee_Test1()
        {
            // Punishment has employees
            var punishmentId = 55;

            await WithUnitOfWorkAsync(async() =>
            {
                var result = await _punishmentManager.IsPunishmentHasEmployee(punishmentId);

                Assert.True(result);
            });
        }

        [Fact]
        public async Task IsPunishmentHasEmployee_Test2()
        {
            // Punishment has no employees
            var punishmentId = 59;

            await WithUnitOfWorkAsync(async() =>
            {
                var result = await _punishmentManager.IsPunishmentHasEmployee(punishmentId);

                Assert.False(result);
            });
        }

        [Fact]
        public void GetPunishmentById_Test1()
        {
            // Standard test case
            var expectedPunishment = new PunishmentDto
            {
                Id = 55,
                Name = "Phạt đi muộn 01/2023",
                Date = new DateTime(2023, 1, 1),
                IsActive = false,
                PunishmentTypeId = 11
            };

            WithUnitOfWork(() =>
            {
                var result = _punishmentManager.GetPunishmentById(expectedPunishment.Id);

                result.Id.ShouldBe(expectedPunishment.Id);
                result.Name.ShouldBe(expectedPunishment.Name);
                result.Date.ShouldBe(expectedPunishment.Date);
                result.IsActive.ShouldBe(expectedPunishment.IsActive);
                result.PunishmentTypeId.ShouldBe(expectedPunishment.PunishmentTypeId);
            });
        }

        [Fact]
        public void GetPunishmentById_Test2()
        {
            // Non-existent punishment
            var punishmentId = 10;

            WithUnitOfWork(() =>
            {
                var result = _punishmentManager.GetPunishmentById(punishmentId);

                Assert.Null(result);
            });
        }

        [Fact]
        public async Task AddEmployeeToPunishment_Test1()
        {
            // Standard test case
            var expectedEmployeeInPunishment = new AddEmployeeToPunishmentDto
            {
                Id = 477,
                EmployeeId = 890,
                PunishmentId = 54,
                Money = 20000,
                Note = "Không daily"
            };
            var allEmployeePunishmentsBeforeAdd = new List<PunishmentEmployee>();
            var allEmployeesInPunishmentBeforeAdd = new List<PunishmentEmployee>();

            await WithUnitOfWorkAsync(async() =>
            {
                var result = await _punishmentManager.AddEmployeeToPunishment(expectedEmployeeInPunishment);
                allEmployeePunishmentsBeforeAdd = _workScope.GetAll<PunishmentEmployee>().ToList();
                allEmployeesInPunishmentBeforeAdd = _workScope.GetAll<PunishmentEmployee>().Where(x => x.PunishmentId == expectedEmployeeInPunishment.PunishmentId).ToList();

                result.Id.ShouldBeGreaterThan(allEmployeePunishmentsBeforeAdd.Last().Id);
                result.Id.ShouldBe(expectedEmployeeInPunishment.Id);
                result.EmployeeId.ShouldBe(expectedEmployeeInPunishment.EmployeeId);
                result.PunishmentId.ShouldBe(expectedEmployeeInPunishment.PunishmentId);
                result.Money.ShouldBe(expectedEmployeeInPunishment.Money);
                result.Note.ShouldBe(expectedEmployeeInPunishment.Note);
            });

            await WithUnitOfWorkAsync(async () =>
            {
                var allEmployeePunishmentsAfterAdd = _workScope.GetAll<PunishmentEmployee>();
                var allEmployeesInPunishmentAfterAdd = _workScope.GetAll<PunishmentEmployee>().Where(x => x.PunishmentId == expectedEmployeeInPunishment.PunishmentId);

                var employeePunishment = await _workScope.GetAsync<PunishmentEmployee>(expectedEmployeeInPunishment.Id);

                allEmployeePunishmentsAfterAdd.Count().ShouldBeGreaterThan(allEmployeePunishmentsBeforeAdd.Count);
                allEmployeesInPunishmentAfterAdd.Count().ShouldBeGreaterThan(allEmployeesInPunishmentBeforeAdd.Count);
                employeePunishment.EmployeeId.ShouldBe(expectedEmployeeInPunishment.EmployeeId);
                employeePunishment.PunishmentId.ShouldBe(expectedEmployeeInPunishment.PunishmentId);
                employeePunishment.Money.ShouldBe(expectedEmployeeInPunishment.Money);
                employeePunishment.Note.ShouldBe(expectedEmployeeInPunishment.Note);
            });
        }

        [Fact]
        public async Task AddEmployeeToPunishment_Test2()
        {
            // Existed Employee in Punishment
            var expectedEmployeeInPunishment = new AddEmployeeToPunishmentDto
            {
                Id = 477,
                EmployeeId = 900,
                PunishmentId = 54,
                Money = 20000,
                Note = "Không daily"
            };
            var expectedMessage = $"This User Is Already Exist";

            await WithUnitOfWorkAsync(async() =>
            {
                var exception = await Assert.ThrowsAsync<UserFriendlyException>(async() => 
                {
                    await _punishmentManager.AddEmployeeToPunishment(expectedEmployeeInPunishment);
                });
                Assert.Equal(expectedMessage, exception.Message);
            });
        }

        [Fact]
        public async Task UpdateEmployeeInPunishment_Test1()
        {
            // Standard test case
            var expectedEmployeeInPunishment = new UpdateEmployeeInPunishmentDto
            {
                Id = 450,
                EmployeeId = 890,
                PunishmentId = 54,
                Money = 20000,
                Note = "Không daily"
            };

            await WithUnitOfWorkAsync(async() =>
            {
                var result = await _punishmentManager.UpdateEmployeeInPunishment(expectedEmployeeInPunishment);

                result.Id.ShouldBe(expectedEmployeeInPunishment.Id);
                result.EmployeeId.ShouldBe(expectedEmployeeInPunishment.EmployeeId);
                result.PunishmentId.ShouldBe(expectedEmployeeInPunishment.PunishmentId);
                result.Money.ShouldBe(expectedEmployeeInPunishment.Money);
                result.Note.ShouldBe(expectedEmployeeInPunishment.Note);
            });

            await WithUnitOfWorkAsync(async () =>
            {
                var employeePunishment = await _workScope.GetAsync<PunishmentEmployee>(expectedEmployeeInPunishment.Id);

                employeePunishment.EmployeeId.ShouldBe(expectedEmployeeInPunishment.EmployeeId);
                employeePunishment.PunishmentId.ShouldBe(expectedEmployeeInPunishment.PunishmentId);
                employeePunishment.Money.ShouldBe(expectedEmployeeInPunishment.Money);
                employeePunishment.Note.ShouldBe(expectedEmployeeInPunishment.Note);
            });
        }

        [Fact]
        public async Task UpdateEmployeeInPunishment_Test2()
        {
            // Existed Employee in Punishment
            var expectedEmployeeInPunishment = new UpdateEmployeeInPunishmentDto
            {
                Id = 450,
                EmployeeId = 900,
                PunishmentId = 54,
                Money = 20000,
                Note = "Không daily"
            };
            var expectedMessage = $"This User Is Already Exist";

            await WithUnitOfWorkAsync(async() =>
            {
                var exception = await Assert.ThrowsAsync<UserFriendlyException>(async() => 
                {
                    await _punishmentManager.UpdateEmployeeInPunishment(expectedEmployeeInPunishment);
                });

                Assert.Equal(expectedMessage, exception.Message);
            });
        }

        [Fact]
        public async Task UpdatePunishmentOfEmployee_Test1()
        {
            // Standard test case
            var expectedEmployeeInPunishment = new UpdateEmployeeInPunishmentDto
            {
                Id = 450,
                EmployeeId = 882,
                PunishmentId = 59,
                Money = 20000,
                Note = "Không daily"
            };

            await WithUnitOfWorkAsync(async() =>
            {
                var result = await _punishmentManager.UpdatePunishmentOfEmployee(expectedEmployeeInPunishment);

                result.Id.ShouldBe(expectedEmployeeInPunishment.Id);
                result.EmployeeId.ShouldBe(expectedEmployeeInPunishment.EmployeeId);
                result.PunishmentId.ShouldBe(expectedEmployeeInPunishment.PunishmentId);
                result.Money.ShouldBe(expectedEmployeeInPunishment.Money);
                result.Note.ShouldBe(expectedEmployeeInPunishment.Note);
            });
        }

        [Fact]
        public async Task UpdatePunishmentOfEmployee_Test2()
        {
            // Existed Employee in Punishment
            var expectedEmployeeInPunishment = new UpdateEmployeeInPunishmentDto
            {
                Id = 460,
                EmployeeId = 897,
                PunishmentId = 56,
                Money = 20000,
                Note = "Không daily"
            };
            var expectedMessage = $"This User Is Already Exist";

            await WithUnitOfWorkAsync(async() =>
            {
                var exception = await Assert.ThrowsAsync<UserFriendlyException>(async () =>
                {
                    await _punishmentManager.UpdatePunishmentOfEmployee(expectedEmployeeInPunishment);
                });

                Assert.Equal(expectedMessage, exception.Message);
            });
        }

        [Fact]
        public async Task DeleteEmployeeFromPunishment_Test1()
        {
            // Standard test case
            var employeePunishmentId = 450;
            var allEmployeePunishmentsBeforeDelete = new List<PunishmentEmployee>();

            await WithUnitOfWorkAsync(async() =>
            {
                allEmployeePunishmentsBeforeDelete = _workScope.GetAll<PunishmentEmployee>().ToList();

                var result = await _punishmentManager.DeleteEmployeeFromPunishment(employeePunishmentId);
                result.ShouldBe(employeePunishmentId);
            });

            await WithUnitOfWorkAsync(async () =>
            {
                var allEmployeePunishmentsAfterDelete = _workScope.GetAll<PunishmentEmployee>().ToList();
                allEmployeePunishmentsAfterDelete.Count.ShouldBeLessThan(allEmployeePunishmentsBeforeDelete.Count);

                await Assert.ThrowsAsync<EntityNotFoundException>(async () =>
                {
                    await _workScope.GetAsync<PunishmentEmployee>(employeePunishmentId);
                });
            });
        }

        [Fact]
        public async Task DeleteEmployeeFromPunishment_Test2()
        {
            // Non-existent employee in punishment
            var employeePunishmentId = 5;

            await WithUnitOfWorkAsync(async() =>
            {
                var exception = await Assert.ThrowsAsync<EntityNotFoundException>(async() =>
                {
                    await _punishmentManager.DeleteEmployeeFromPunishment(employeePunishmentId);
                });
            });
        }

        [Fact]
        public async Task GetAllEmployeeInPunishment_Test1()
        {
            // Standard test case
            var punishmentId = 53;
            var employeePunishment = new GetEmployeePunishment
            {
                GridParam = new GridParam { SortDirection = SortDirection.ASC } ,
                TeamIds = new List<long> { 42, 43 },
                IsAndCondition = false,
                StatusIds = new List<EmployeeStatus> { EmployeeStatus.Working },
                LevelIds = new List<long> { 319 },
                Usertypes = new List<UserType> { UserType.Staff, UserType.ProbationaryStaff },
                BranchIds = new List<long> { 95 },
                JobPositionIds = new List<long> { 47 }
            };

            var expectedTotalCount = 2;
            var expectedItemsCount = 2;

            await WithUnitOfWorkAsync(async() =>
            {
                var result = await _punishmentManager.GetAllEmployeeInPunishment(punishmentId, employeePunishment);

                result.TotalCount.ShouldBe(expectedTotalCount);
                result.Items.Count.ShouldBe(expectedItemsCount);
                result.Items.ShouldContain(employeePunishment => employeePunishment.EmployeeId == 881);
                result.Items.ShouldContain(employeePunishment => employeePunishment.EmployeeId == 882);
            });
        }

        [Fact]
        public async Task GetAllEmployeeInPunishment_Test2()
        {
            // Empty Result
            var punishmentId = 2;
            var employeePunishment = new GetEmployeePunishment
            {
                GridParam = new GridParam { SortDirection = SortDirection.ASC },
                TeamIds = null,
                IsAndCondition = false,
                StatusIds = new List<EmployeeStatus> { EmployeeStatus.Quit },
                LevelIds = new List<long> { 319 },
                Usertypes = new List<UserType> { UserType.Staff },
                BranchIds = new List<long> { 95 },
                JobPositionIds = new List<long> { 47 }
            };

            await WithUnitOfWorkAsync(async() =>
            {
                var result = await _punishmentManager.GetAllEmployeeInPunishment(punishmentId, employeePunishment);

                Assert.Empty(result.Items);
            });
        }

        [Fact]
        public void GetAllEmployeeNotInPunishment_Test1()
        {
            // Standard test case
            var punishmentId = 53;

            WithUnitOfWork(() =>
            {
                var result = _punishmentManager.GetAllEmployeeNotInPunishment(punishmentId);

                var allEmployees = _workScope.GetAll<Employee>().ToList();
                var allEmployeesInPunishment = _workScope.GetAll<PunishmentEmployee>().Where(x => x.PunishmentId == punishmentId).ToList();

                Assert.Equal(allEmployees.Count - allEmployeesInPunishment.Count, result.Count);
            });
        }

        [Fact]
        public void GetListDate_Test1()
        {
            WithUnitOfWork(() =>
            {
                List<DateTime> result = _punishmentManager.GetListDate();

                Assert.Equal(5, result.Count);
            });
        }

        [Fact]
        public void GetDateFromPunishments_Test1()
        {
            var expectedResult = 3;
            WithUnitOfWork(() =>
            {
                var result = _punishmentManager.GetDateFromPunishments();

                Assert.Equal(expectedResult, result.Count);
            });
        }

        [Fact]
        public void GetDateFromPunishmentsOfEmployee_Test1()
        {
            // Standard test case
            var employeeId = 897;

            WithUnitOfWork(() =>
            {
                var result = _punishmentManager.GetDateFromPunishmentsOfEmployee(employeeId);

                var expectedResult = _workScope.GetAll<PunishmentEmployee>()
                    .Where(x => x.EmployeeId == employeeId)
                    .Select(x => x.Punishment.Date)
                    .OrderByDescending(x => x.Date)
                    .Distinct()
                    .ToList();

                Assert.Equal(expectedResult.Count, result.Count);
            });
        }

        [Fact]
        public void ImportEmployeePunishmentsFromFile_Test1()
        {
            // Standard test case
            //var file = ".\\Managers\\Punishments\\Files\\import-employee-to-punishment-test-1.xlsx";
            //var stream = new MemoryStream(File.ReadAllBytes(file).ToArray());
            //var formFile = new FormFile(stream, 0, stream.Length, "streamFile", file.Split(@"\").Last());
            var fileName = "import-employee-to-punishment-test-1.xlsx";
            var root = Directory.GetCurrentDirectory().Replace("\\bin\\Debug\\net6.0", "").Replace("/bin/Debug/net6.0", "");
            string path = Path.Combine(root, "Managers", "Punishments", "Files", fileName);

            var stream = new MemoryStream(File.ReadAllBytes(path).ToArray());
            var formFile = new FormFile(stream, 0, stream.Length, "streamFile", fileName);

            var input = new ImportFileDto
            {
                File = formFile,
                PunishmentId = 55
            };

            var expectedPunishmentEmployee = new
            {
                Id = 477,
                Email = "an.phamthien@ncc.asia",
                Money = 10000,
                Note = "Quên lock máy"
            };

            var allPunishmentEmployeesBeforeImport = new List<PunishmentEmployee>();

            WithUnitOfWork(async() =>
            {
                allPunishmentEmployeesBeforeImport = _workScope.GetAll<PunishmentEmployee>().ToList();

                var result = _punishmentManager.ImportEmployeePunishmentsFromFile(input);
                
                var successList = result.GetType().GetProperties().First(o => o.Name == "successList").GetValue(result, null);
                var failedList = result.GetType().GetProperties().First(o => o.Name == "failedList").GetValue(result, null);
                
                var success = successList as ICollection<PunishmentEmployee>;
                var failed = failedList as ICollection<ResponseFailDto>;

                Assert.Equal(1, success.Count);
                Assert.Empty(failed);

                success.First().Id.ShouldBe(expectedPunishmentEmployee.Id);    
                success.First().Money.ShouldBe(expectedPunishmentEmployee.Money);
                success.First().Note.ShouldBe(expectedPunishmentEmployee.Note);

                var employee = await _workScope.GetAsync<Employee>(success.First().EmployeeId);
                employee.Email.ShouldBe(expectedPunishmentEmployee.Email);

            });

            WithUnitOfWork(async () =>
            {
                var allPunishmentEmployeesAfterImport = _workScope.GetAll<PunishmentEmployee>();
                var punishmentEmployee = await _workScope.GetAsync<PunishmentEmployee>(expectedPunishmentEmployee.Id);
                var employee = await _workScope.GetAsync<Employee>(punishmentEmployee.EmployeeId);

                allPunishmentEmployeesAfterImport.Count().ShouldBeGreaterThan(allPunishmentEmployeesBeforeImport.Count);
                punishmentEmployee.ShouldNotBeNull();
                employee.Email.ShouldBe(expectedPunishmentEmployee.Email);
                punishmentEmployee.Money.ShouldBe(expectedPunishmentEmployee.Money);
                punishmentEmployee.Note.ShouldBe(expectedPunishmentEmployee.Note);
            });
        }

        //[Fact]
        //public void ImportEmployeePunishmentsFromFile_Test2()
        //{
        //    // Incorrect file format
        ////    var file = ".\\Managers\\Punishments\\Files\\import-employee-to-punishment-test-2.pdf";
        ////    var stream = new MemoryStream(File.ReadAllBytes(file).ToArray());
        ////    var formFile = new FormFile(stream, 0, stream.Length, "streamFile", file.Split(@"\").Last());
        //      var fileName = "import-employee-to-punishment-test-2.xlsx";
        //    var root = Directory.GetCurrentDirectory().Replace("\\bin\\Debug\\net6.0", "").Replace("/bin/Debug/net6.0", "");
        //    string path = Path.Combine(root, "Managers", "Punishments", "Files", fileName);

        //    var stream = new MemoryStream(File.ReadAllBytes(path).ToArray());
        //    var formFile = new FormFile(stream, 0, stream.Length, "streamFile", fileName);
        //    var input = new ImportFileDto
        //    {
        //        File = formFile,
        //        PunishmentId = 55
        //    };
        //    var expectedMessage = "File null or is not .xlsx file";

        //    WithUnitOfWork(() =>
        //    {
        //        try
        //        {
        //            _punishmentManager.ImportEmployeePunishmentsFromFile(input);
        //        }
        //        catch (Exception e)
        //        {
        //            Assert.True(e is UserFriendlyException);
        //            Assert.Equal(expectedMessage, e.Message);
        //        }
        //    });
        //}

        [Fact]
        public void ImportEmployeePunishmentsFromFile_Test3()
        {
            // Non-existent Punishment
            //var file = ".\\Managers\\Punishments\\Files\\import-employee-to-punishment-test-1.xlsx";
            //var stream = new MemoryStream(File.ReadAllBytes(file).ToArray());
            //var formFile = new FormFile(stream, 0, stream.Length, "streamFile", file.Split(@"\").Last());
            var fileName = "import-employee-to-punishment-test-1.xlsx";
            var root = Directory.GetCurrentDirectory().Replace("\\bin\\Debug\\net6.0", "").Replace("/bin/Debug/net6.0", "");
            string path = Path.Combine(root, "Managers", "Punishments", "Files", fileName);

            var stream = new MemoryStream(File.ReadAllBytes(path).ToArray());
            var formFile = new FormFile(stream, 0, stream.Length, "streamFile", fileName);
            var input = new ImportFileDto
            {
                File = formFile,
                PunishmentId = 10
            };
            var expectedMessage = "Not found Punishment Id " + input.PunishmentId;

            WithUnitOfWork(() =>
            {
                try
                {
                    _punishmentManager.ImportEmployeePunishmentsFromFile(input);
                }
                catch (Exception e)
                {
                    Assert.True(e is UserFriendlyException);
                    Assert.Equal(expectedMessage, e.Message);
                }
            });
        }

        [Fact]
        public void ImportEmployeePunishmentsFromFile_Test4()
        {
            // Wrong number of columns
            //var file = ".\\Managers\\Punishments\\Files\\import-employee-to-punishment-test-4.xlsx";
            //var stream = new MemoryStream(File.ReadAllBytes(file).ToArray());
            //var formFile = new FormFile(stream, 0, stream.Length, "streamFile", file.Split(@"\").Last());
            var fileName = "import-employee-to-punishment-test-4.xlsx";
            var root = Directory.GetCurrentDirectory().Replace("\\bin\\Debug\\net6.0", "").Replace("/bin/Debug/net6.0", "");
            string path = Path.Combine(root, "Managers", "Punishments", "Files", fileName);

            var stream = new MemoryStream(File.ReadAllBytes(path).ToArray());
            var formFile = new FormFile(stream, 0, stream.Length, "streamFile", fileName);
            var input = new ImportFileDto
            {
                File = formFile,
                PunishmentId = 55
            };
            var expectedMessage = "Number of columns < 3 => Invlid format";

            WithUnitOfWork(() =>
            {
                try
                {
                    _punishmentManager.ImportEmployeePunishmentsFromFile(input);
                }
                catch (Exception e)
                {
                    Assert.True(e is UserFriendlyException);
                    Assert.Equal(expectedMessage, e.Message);
                }
            });
        }

        [Fact]
        public void ImportEmployeePunishmentsFromFile_Test5()
        {
            // Some imports fail
            //var file = ".\\Managers\\Punishments\\Files\\import-employee-to-punishment-test-5.xlsx";
            //var stream = new MemoryStream(File.ReadAllBytes(file).ToArray());
            //var formFile = new FormFile(stream, 0, stream.Length, "streamFile", file.Split(@"\").Last());
            var fileName = "import-employee-to-punishment-test-5.xlsx";
            var root = Directory.GetCurrentDirectory().Replace("\\bin\\Debug\\net6.0", "").Replace("/bin/Debug/net6.0", "");
            string path = Path.Combine(root, "Managers", "Punishments", "Files", fileName);

            var stream = new MemoryStream(File.ReadAllBytes(path).ToArray());
            var formFile = new FormFile(stream, 0, stream.Length, "streamFile", fileName);
            var input = new ImportFileDto
            {
                File = formFile,
                PunishmentId = 55
            };

            var expectedPunishmentEmployee = new
            {
                Id = 477,
                Email = "thong.nguyenba@ncc.asia",
                Money = 10000,
                Note = "Quên lock máy"
            };

            var allPunishmentEmployeesBeforeImport = new List<PunishmentEmployee>();

            WithUnitOfWork(async() =>
            {
                allPunishmentEmployeesBeforeImport = _workScope.GetAll<PunishmentEmployee>().ToList();

                var result = _punishmentManager.ImportEmployeePunishmentsFromFile(input);
                
                var successList = result.GetType().GetProperties().First(o => o.Name == "successList").GetValue(result, null);
                var failedList = result.GetType().GetProperties().First(o => o.Name == "failedList").GetValue(result, null);
                
                var success = successList as ICollection<PunishmentEmployee>;
                var failed = failedList as ICollection<ResponseFailDto>;

                Assert.Equal(1, success.Count);
                Assert.Equal(1, failed.Count);

                success.First().Id.ShouldBe(expectedPunishmentEmployee.Id);
                success.First().Money.ShouldBe(expectedPunishmentEmployee.Money);
                success.First().Note.ShouldBe(expectedPunishmentEmployee.Note);

                var employee = await _workScope.GetAsync<Employee>(success.First().EmployeeId);
                employee.Email.ShouldBe(expectedPunishmentEmployee.Email);
            });

            WithUnitOfWork(async () =>
            {
                var allPunishmentEmployeesAfterImport = _workScope.GetAll<PunishmentEmployee>();
                var punishmentEmployee = await _workScope.GetAsync<PunishmentEmployee>(expectedPunishmentEmployee.Id);
                var employee = await _workScope.GetAsync<Employee>(punishmentEmployee.EmployeeId);

                allPunishmentEmployeesAfterImport.Count().ShouldBeGreaterThan(allPunishmentEmployeesBeforeImport.Count);
                punishmentEmployee.ShouldNotBeNull();
                employee.Email.ShouldBe(expectedPunishmentEmployee.Email);
                punishmentEmployee.Money.ShouldBe(expectedPunishmentEmployee.Money);
                punishmentEmployee.Note.ShouldBe(expectedPunishmentEmployee.Note);
            });
        }

        [Fact]
        public void ImportEmployeePunishmentsFromFile_Test6()
        {
            // All imports fail
            //var file = ".\\Managers\\Punishments\\Files\\import-employee-to-punishment-test-6.xlsx";
            //var stream = new MemoryStream(File.ReadAllBytes(file).ToArray());
            //var formFile = new FormFile(stream, 0, stream.Length, "streamFile", file.Split(@"\").Last());
            var fileName = "import-employee-to-punishment-test-6.xlsx";
            var root = Directory.GetCurrentDirectory().Replace("\\bin\\Debug\\net6.0", "").Replace("/bin/Debug/net6.0", "");
            string path = Path.Combine(root, "Managers", "Punishments", "Files", fileName);

            var stream = new MemoryStream(File.ReadAllBytes(path).ToArray());
            var formFile = new FormFile(stream, 0, stream.Length, "streamFile", fileName);
            var input = new ImportFileDto
            {
                File = formFile,
                PunishmentId = 55
            };

            WithUnitOfWork(() =>
            {
                var result = _punishmentManager.ImportEmployeePunishmentsFromFile(input);
                
                var successList = result.GetType().GetProperties().First(o => o.Name == "successList").GetValue(result, null);
                var failedList = result.GetType().GetProperties().First(o => o.Name == "failedList").GetValue(result, null);

                var success = successList as ICollection<PunishmentEmployee>;
                var failed = failedList as ICollection<ResponseFailDto>;

                Assert.Empty(success);
                Assert.Equal(2, failed.Count);
            });
        }
    }
}
