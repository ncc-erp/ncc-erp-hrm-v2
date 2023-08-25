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
using Abp.Threading.Timers;
using Abp.UI;
using HRMv2.Editions;
using HRMv2.Entities;
using HRMv2.Manager.Bonuses.Dto;
using HRMv2.Manager.Categories;
using HRMv2.Manager.Categories.Benefits;
using HRMv2.Manager.Categories.Bonuss;
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
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Moq;
using NccCore.Paging;
using Shouldly;
using Xunit;
using static HRMv2.Constants.Enum.HRMEnum;
using SortDirection = NccCore.Paging.SortDirection;

namespace HRMv2.Core.Tests.Managers.Bonuses
{

    public class BonusManager_Tests : HRMv2CoreTestBase
    {
        private readonly BonusManager _bonusManager;
        private readonly IWorkScope _workScope;

        public BonusManager_Tests()
        {
            _workScope = Resolve<IWorkScope>();

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

            var mockHistoryManager = new Mock<HistoryManager>(_workScope, mockBranchManager.Object, mockLevelManager.Object, mockChangeEmployeeWorkingStatusManager.Object);
            var mockUserTypeManager = new Mock<UserTypeManager>(_workScope);
            var mockJobPositionManager = new Mock<JobPositionManager>(_workScope);
            var mockIBackgroundJobStore = new Mock<IBackgroundJobStore>();
            var mockAbpAsyncTimer = new Mock<AbpAsyncTimer>();
            var mockBackgroundJobManager = new Mock<BackgroundJobManager>(mockIIocResolver.Object, mockIBackgroundJobStore.Object, mockAbpAsyncTimer.Object);
            var mockSalaryRequestManager = new Mock<SalaryRequestManager>(
                mockLevelManager.Object,
                mockJobPositionManager.Object,
                mockContractManager.Object,
                _workScope,
                mockEmailManager.Object,
                mockBackgroundJobManager.Object);

            var mockEmployeeManager = new Mock<EmployeeManager>(
               mockUploadFileService.Object,
               mockContractManager.Object,
               mockHistoryManager.Object,
               mockProjectService.Object,
               mockTimesheetWebService.Object,
               mockIMSWebService.Object,
               mockTalentWebService.Object,
               _workScope,
               mockSalaryRequestManager.Object,
               mockBenefitManager.Object,
               mockUserTypeManager.Object,
               mockChangeEmployeeWorkingStatusManager.Object,
               mockBackgroundJobManager.Object,
               mockBackgroundJobInfoRepository.Object);


            _bonusManager = new BonusManager(
                _workScope,
                mockEmployeeManager.Object,
                mockEmailManager.Object,
                mockBackgroundJobManager.Object);
            _bonusManager.ObjectMapper = Resolve<IObjectMapper>();
        }

         
        //GetAllPaging
        [Fact]
        public async Task GetAllPaging_SkipParam()
        {
            var gridParam = new GridParam
            {
                SkipCount = 4,
            };

            await WithUnitOfWorkAsync(async () =>
            {
                var result = await _bonusManager.GetAllPaging(gridParam);
                Assert.Equal(3, result.Items.Count);
            });
        }

        [Fact]
        public async Task GetAllPaging_MaxParam()
        {
            var gridParam = new GridParam
            {
                SkipCount = 3,
                MaxResultCount = 2,
            };

            await WithUnitOfWorkAsync(async () =>
            {
                var result = await _bonusManager.GetAllPaging(gridParam);
                Assert.Equal(2, result.Items.Count);
            });
        }

        [Fact]
        public async Task GetAllPaging_SearchParam()
        {
            var gridParam = new GridParam
            {
                SearchText = "Search not map",
            };

            await WithUnitOfWorkAsync(async () =>
            {
                var result = await _bonusManager.GetAllPaging(gridParam);
                Assert.Equal(0, result.Items.Count);
            });
        }

        [Fact]
        public async Task GetAllPaging_SortParam()
        {
            var gridParam = new GridParam
            {
                SortDirection = SortDirection.DESC,
                Sort = "totalMoney",
                MaxResultCount = 3,
            };

            await WithUnitOfWorkAsync(async () =>
            {
                var result = await _bonusManager.GetAllPaging(gridParam);
                for (int i = 0; i < result.Items.Count; i++)
                {
                    var curEl = result.Items[i];
                    if (i < result.Items.Count - 1)
                    {
                        var nextEl = result.Items[i + 1];
                        curEl.TotalMoney.ShouldBeGreaterThanOrEqualTo(nextEl.TotalMoney);
                    }
                }

            });
        }

        //GetAll
        [Fact]
        public void GetAll_CheckReturnList()
        {
            List<GetBonusDto> listBonuses = new List<GetBonusDto>
            {
                new GetBonusDto {
                    Id = 54,
                    Name = "Thưởng sinh nhât tháng 12",
                    IsActive = false,
                    ApplyMonth = DateTime.Parse("2022-12-15 00:00:00")
                },
                new GetBonusDto {
                    Id = 55,
                    Name = "Thưởng dự án tháng 12",
                    IsActive = false,
                    ApplyMonth = DateTime.Parse("2022-12-15 00:00:00")
                },
                new GetBonusDto {
                    Id = 56,
                    Name = "Bonus check point tháng 01/2003",
                    IsActive = false,
                    ApplyMonth = DateTime.Parse("2023-01-15 00:00:00")
                },
                new GetBonusDto {
                    Id = 57,
                    Name = "Thưởng sinh nhật tháng 01/2023",
                    IsActive = false,
                    ApplyMonth = DateTime.Parse("2023-01-15 00:00:00")
                },
                new GetBonusDto {
                    Id = 58,
                    Name = "Thưởng sinh nhật tháng 02/2023",
                    IsActive = false,
                    ApplyMonth = DateTime.Parse("2023-02-15 00:00:00")
                },
                new GetBonusDto {
                    Id = 60,
                    Name = "Bonus thưởng dự án 02/2023",
                    IsActive = false,
                    ApplyMonth = DateTime.Parse("2023-02-15 00:00:00")
                },
                new GetBonusDto {
                    Id = 61,
                    Name = "Bonus thưởng dự án 5/2023",
                    IsActive = true,
                    ApplyMonth = DateTime.Parse("2023-02-15 00:00:00")
                },
            };

            WithUnitOfWork(() =>
            {
                var listActual = _bonusManager.GetAll();

                Assert.NotNull(listActual);
                Assert.Equal(listBonuses.Count, listActual.Count);

                for (int i = 0; i < listActual.Count; i++)
                {
                    var objAR = listActual.Skip(i).Take(1).FirstOrDefault();
                    var objER = listBonuses.Skip(i).Take(1).FirstOrDefault();

                    objAR?.Id.ShouldBeEquivalentTo(objER?.Id);
                    objAR?.Name.ShouldBeEquivalentTo(objER?.Name);
                    objAR?.IsActive.ShouldBeEquivalentTo(objER?.IsActive);

                    Assert.Equal(objER?.ApplyMonth.ToString(), objAR?.ApplyMonth.ToString());
                }
            });
        }


        //GetAllPagingBonusesByEmployeeId
        [Fact]
        public async Task GetAllPagingBonusesByEmployeeId_WrongEmployeeId_ReturnEmptyList()
        {
            var gridParam = new GridParam();

            long employeeId = 0;

            await WithUnitOfWorkAsync(async () =>
            {
                var result = await _bonusManager.GetAllPagingBonusesByEmployeeId(employeeId, gridParam);
                Assert.Equal(0, result.Items.Count);
            });
        }


        [Fact]
        public async Task GetAllPagingBonusesByEmployeeId_RightEmployeeId_SkipParam()
        {
            var gridParam = new GridParam
            {
                SkipCount = 1,
            };
            long employeeId = 891;

            await WithUnitOfWorkAsync(async () =>
            {
                var result = await _bonusManager.GetAllPagingBonusesByEmployeeId(employeeId, gridParam);
                Assert.Equal(4, result.Items.Count);
            });
        }

        [Fact]
        public async Task GetAllPagingBonusesByEmployeeId_RightEmployeeId_MaxParam()
        {
            var gridParam = new GridParam
            {
                MaxResultCount = 10,
            };
            long employeeId = 891;

            await WithUnitOfWorkAsync(async () =>
            {
                var result = await _bonusManager.GetAllPagingBonusesByEmployeeId(employeeId, gridParam);
                Assert.Equal(5, result.Items.Count);
            });
        }

        [Fact]
        public async Task GetAllPagingBonusesByEmployeeId_RightEmployeeId_SearchParam()
        {
            var gridParam = new GridParam
            {
                SearchText = "Thưởng sinh nhât tháng 12",
            };
            long employeeId = 891;

            await WithUnitOfWorkAsync(async () =>
            {
                var result = await _bonusManager.GetAllPagingBonusesByEmployeeId(employeeId, gridParam);
                Assert.Equal(1, result.Items.Count);
            });
        }


        [Fact]
        public async Task GetAllPagingBonusesByEmployeeId_RightEmployeeId_SortParam()
        {
            var gridParam = new GridParam
            {
                SortDirection = SortDirection.DESC,
                Sort = "money",
            };
            long employeeId = 891;

            await WithUnitOfWorkAsync(async () =>
            {
                var result = await _bonusManager.GetAllPagingBonusesByEmployeeId(employeeId, gridParam);
                Assert.NotNull(result);

                for (int i = 0; i < result.Items.Count; i++)
                {
                    var curEl = result.Items[i];
                    if (i == result.Items.Count - 1) return;
                    var nextEl = result.Items[i + 1];
                    curEl.Money.ShouldBeGreaterThanOrEqualTo(nextEl.Money);
                }
            });
        }


        //GetBonusesByEmployeeId
        [Fact]
        public void GetBonusesByEmployeeId_RightEmployeeId_ReturnListItem()
        {

            List<GetBonusesOfEmployeeDto> listEmployeeBonuses = new List<GetBonusesOfEmployeeDto>
            {
                 new GetBonusesOfEmployeeDto {
                    Id = 988,
                    BonusId = 56,
                    BonusName = "Bonus check point tháng 01/2003",
                    ApplyMonth = DateTime.Parse("2023-01-15 00:00:00"),
                    Money = 1000000.0,
                    Note = "Bonus check point tháng 11/2022",
                    IsActive = false,
                    UpdatedTime = DateTime.Parse("2022-12-19 13:32:28.432514"),
                    UpdatedUser = "admin admin"
                 },
                  new GetBonusesOfEmployeeDto {
                    Id = 1015,
                    BonusId = 60,
                    BonusName = "Bonus thưởng dự án 02/2023",
                    ApplyMonth = DateTime.Parse("2023-02-15 00:00:00"),
                    Money = 200000.0,
                    Note = "Bonus thưởng dự án 02/2023",
                    IsActive = false,
                    UpdatedTime = DateTime.Parse("2022-12-19 13:47:53.195762"),
                    UpdatedUser = "admin admin"
                 }
            };


            WithUnitOfWork(() =>
            {
                int employeeId = 880;
                var AR = _bonusManager.GetBonusesByEmployeeId(employeeId);

                AR.Count.ShouldBeEquivalentTo(listEmployeeBonuses.Count);
                for (int i = 0; i < AR.Count; i++)
                {
                    var objAR = AR.Skip(i).Take(1).FirstOrDefault();
                    var objER = listEmployeeBonuses.Skip(i).Take(1).FirstOrDefault();


                    objAR?.Id.ShouldBeEquivalentTo(objER?.Id);
                    objAR?.BonusId.ShouldBeEquivalentTo(objER?.BonusId);
                    objAR?.BonusName.ShouldBeEquivalentTo(objER?.BonusName);
                    objAR?.ApplyMonth.ShouldBeEquivalentTo(objER?.ApplyMonth);
                    objAR?.Money.ShouldBeEquivalentTo(objER?.Money);
                    objAR?.Note.ShouldBeEquivalentTo(objER?.Note);
                    objAR?.IsActive.ShouldBeEquivalentTo(objER?.IsActive);
                    objAR?.UpdatedUser.ShouldBeEquivalentTo(objER?.UpdatedUser);
                    Assert.Equal(objER?.UpdatedTime.ToString(), objAR?.UpdatedTime.ToString());
                }
            });
        }


        [Fact]
        public void GetBonusesByEmployeeId_WrongEmployeeId_ReturnListEmpty()
        {

            WithUnitOfWork(() =>
            {
                int employeeId = 0;
                var result = _bonusManager.GetBonusesByEmployeeId(employeeId);

                result.Count.ShouldBeEquivalentTo(0);
            });
        }


        //GetAllEmployeeNotInBonus
        [Fact]
        public void GetAllEmployeeNotInBonus_RightBonusId_CheckListId()
        {

            long[] listEmployeeIdInBonus = new long[19] { 897, 888, 887, 886, 889, 904, 903, 890, 905, 885, 891, 900, 901, 880, 881, 882, 883, 884, 902 };

            WithUnitOfWork(() =>
            {
                int bonusId = 60;
                var AR = _bonusManager.GetAllEmployeeNotInBonus(bonusId);

                var expecEmployeeNotInBonusCount = 5;
                AR.Count.ShouldBeEquivalentTo(expecEmployeeNotInBonusCount);
                for (int i = 0; i < AR.Count; i++)
                {
                    listEmployeeIdInBonus.ToArray().ShouldNotContain(AR[i].Id);
                }
            });
        }

        [Fact]
        public void GetAllEmployeeNotInBonus_RightBonusId_ReturnRightListItem()
        {

            List<GetEmployeeBasicInfoDto> listExpected = new List<GetEmployeeBasicInfoDto>
            {
                new GetEmployeeBasicInfoDto
                {
                    Id = 899,
                    FullName = "Đinh Vương Bảo",
                    Email = "bao.dinhvuong@ncc.asia"
                },
                new GetEmployeeBasicInfoDto
                {
                    Id = 898,
                    FullName = "Huỳnh Hải Ninh",
                    Email = "ninh.huynhhai@ncc.asia"
                },
                new GetEmployeeBasicInfoDto
                {
                    Id = 895,
                    FullName = "Lê Trần Hà Giang",
                    Email = "giang.letranha@ncc.asia"
                },
                new GetEmployeeBasicInfoDto
                {
                    Id = 894,
                    FullName = "Lý Hùng Trọng",
                    Email = "trong.lyhung@ncc.asia"
                },
                new GetEmployeeBasicInfoDto
                {
                    Id = 893,
                    FullName = "Trần Diễm Tú",
                    Email = "tu.trandiem@ncc.asia"
                }
            };

            WithUnitOfWork(() =>
            {
                int bonusId = 60;
                var AR = _bonusManager.GetAllEmployeeNotInBonus(bonusId);

                Assert.NotNull(AR);
                AR.Count.ShouldBeEquivalentTo(listExpected.Count);
                for (int i = 0; i < AR.Count; i++)
                {
                    var objAR = AR.Skip(i).Take(1).FirstOrDefault();
                    var objER = listExpected.Skip(i).Take(1).FirstOrDefault();

                    objAR?.Id.ShouldBeEquivalentTo(objER?.Id);
                    objAR?.FullName.ShouldBeEquivalentTo(objER?.FullName);
                    objAR?.Email.ShouldBeEquivalentTo(objER?.Email);
                }
            });
        }

        [Fact]
        public void GetAllEmployeeNotInBonus_WrongBonusId_ReturnTotalEmployee()
        {

            WithUnitOfWork(() =>
            {
                int bonusId = -1;
                var ER = _workScope.GetAll<Employee>().ToList();
                var AR = _bonusManager.GetAllEmployeeNotInBonus(bonusId);


                AR.Count.ShouldBeEquivalentTo(ER.Count);
            });
        }


        //Create
        [Fact]
        public async Task Create_Success()
        {
            var bonuses = new List<Bonus>();
            long countBef = 0;

            await WithUnitOfWorkAsync(async () =>
            {
                bonuses = _workScope.GetAll<Bonus>().ToList();
                countBef = bonuses.Count;

                var bonusDto = new BonusDto()
                {
                    Name = "Surplus, Final, YES",
                };
                var result = await _bonusManager.Create(bonusDto);

                Assert.NotNull(result);
                Assert.Equal(bonusDto.Name, result.Name);
                Assert.True(result.IsActive);
            });

            WithUnitOfWork(() =>
            {
                bonuses = _workScope.GetAll<Bonus>().ToList();

                long countAft = bonuses.Count;
                countAft.ShouldBeEquivalentTo(countBef + 1);
            });
        }

        [Fact]
        public async Task Create_EmptyDto_InvalidOperationException()
        {
            await WithUnitOfWorkAsync(async () =>
            {

                await Assert.ThrowsAsync<InvalidOperationException>(async () =>
                {
                    await _bonusManager.Create(new BonusDto());
                });
            });
        }

        [Fact]
        public async Task Create_ExistName_UserFriendlyException()
        {
            await WithUnitOfWorkAsync(async () =>
            {
                var bonusDto = new BonusDto()
                {
                    Name = "Thưởng sinh nhât tháng 12",
                };

                var exception = await Assert.ThrowsAsync<UserFriendlyException>(async () =>
                {
                    await _bonusManager.Create(bonusDto);
                });

                var expectErrorMsg = "Name is Already Exist";
                Assert.Equal(exception.Message, expectErrorMsg);
            });
        }


        //Update
        [Fact]
        public async Task Update_IdNotActive_ObjectDisposedException()
        {
            var editBonusDto = new EditBonusDto()
            {
                Id = 59,
                IsActive = true,
                ApplyMonth = DateTime.Now,
                Name = "Surplus"
            };

            await Assert.ThrowsAsync<ObjectDisposedException>(async () =>
            {
                await _bonusManager.Update(editBonusDto);
            });
        }

        [Fact]
        public async Task Update_ChangedApplyMothLessThanPayroll_UserFriendlyException()
        {
            await WithUnitOfWorkAsync(async () =>
            {
                var editBonusDto = new EditBonusDto()
                {
                    Id = 61,
                    IsActive = false,
                    ApplyMonth = DateTime.Parse("2022-12-21 00:00:00"), //Change this they < Payroll latest ApplyMonth
                    Name = "Surplus"
                };

                var exception = await Assert.ThrowsAsync<UserFriendlyException>(async () =>
                {
                    var result = await _bonusManager.Update(editBonusDto);
                });


                Assert.Contains("Only update apply month", exception.Message);
            });
        }

        [Fact]
        public async Task Update_Success()
        {
            var editBonusDto = new EditBonusDto()
            {
                Id = 61,
                IsActive = false,
                ApplyMonth = DateTime.Parse("2023-02-15 00:00:00"), //Dont change this
                Name = "Surplus"
            };


            await WithUnitOfWorkAsync(async () =>
            {
                var result = await _bonusManager.Update(editBonusDto);

                Assert.NotNull(result);
                Assert.Equal(editBonusDto.Name, result.Name);
            });

            WithUnitOfWork(() =>
            {
                var entity = _workScope.GetAsync<Bonus>(editBonusDto.Id).Result;
                var expectedDateTime = new DateTime(editBonusDto.ApplyMonth.Year, editBonusDto.ApplyMonth.Month, 15);

                Assert.Equal(editBonusDto.Name, entity.Name);
                Assert.Equal(editBonusDto.IsActive, entity.IsActive);
                Assert.Equal(expectedDateTime, entity.ApplyMonth);
                Assert.Equal(editBonusDto.Id, entity.Id);
            });
        }


        //UpdateBonusEmployee
        [Fact]
        public async Task UpdateBonusEmployee_WrongId_EntityNotFoundException()
        {
            UpdateBonusEmployeeDto dto = new UpdateBonusEmployeeDto();
            dto.Id = 0;

            await Assert.ThrowsAsync<EntityNotFoundException>(async () =>
            {
                await _bonusManager.UpdateBonusEmployee(dto);
            });
        }

        [Fact]
        public async Task UpdateBonusEmployee_WrongEmployeeId_NullReferenceException()
        {
            UpdateBonusEmployeeDto dto = new UpdateBonusEmployeeDto()
            {
                Id = 966,
                EmployeeId = 0,
            };

            await Assert.ThrowsAsync<NullReferenceException>(async () =>
            {
                await _bonusManager.UpdateBonusEmployee(dto);
            });
        }

        [Fact]
        public async Task UpdateBonusEmployee_RightId_Success()
        {
            UpdateBonusEmployeeDto dto = new UpdateBonusEmployeeDto()
            {
                Id = 966,
                EmployeeId = 891,
                BonusId = 55,
                Money = 123456,
                Note = "Note Changed from now"
            };

            await WithUnitOfWorkAsync(async () =>
            {
                var result = await _bonusManager.UpdateBonusEmployee(dto);
                Assert.NotNull(result);
            });

            WithUnitOfWork(() =>
            {
                var entity = _workScope.GetAsync<BonusEmployee>(dto.Id).Result;

                Assert.Equal(dto.Id, entity.Id);
                Assert.Equal(dto.EmployeeId, entity.EmployeeId);
                Assert.Equal(dto.BonusId, entity.BonusId);
                Assert.Equal(dto.Money, entity.Money);
                Assert.Equal(dto.Note, entity.Note);
            });
        }


        //Delete
        [Fact]
        public async Task Delete_WrongId_EntityNotFoundException()
        {
            long id = 0;

            await Assert.ThrowsAsync<EntityNotFoundException>(async () =>
            {
                await _bonusManager.Delete(id);
            });
        }

        [Fact]
        public async Task Delete_RightId_Success()
        {
            List<Bonus> bonuses;
            var countBef = 0;

            await WithUnitOfWorkAsync(async () =>
            {
                long id = 54;

                bonuses = _workScope.GetAll<Bonus>().ToList();
                countBef = bonuses.Count;
                var resultId = await _bonusManager.Delete(id);
                Assert.Equal(id, resultId);
            });

            WithUnitOfWork(() =>
            {
                bonuses = _workScope.GetAll<Bonus>().ToList();
                var countAft = bonuses.Count;

                countAft.ShouldBeEquivalentTo(countBef - 1);
            });
        }


        //IsBonusHasEmployee
        [Fact]
        public async Task IsBonusHasEmployee_RightId_ShouldTrue()
        {

            await WithUnitOfWorkAsync(async () =>
            {
                long bonusId = 60;

                var result = await _bonusManager.IsBonusHasEmployee(bonusId);
                result.ShouldBe(true);
            });
        }

        [Fact]
        public async Task IsBonusHasEmployee_RightId_ShouldFalse()
        {

            await WithUnitOfWorkAsync(async () =>
            {
                long bonusId = 61; //Id has employee bonus

                var result = await _bonusManager.IsBonusHasEmployee(bonusId);
                result.ShouldBe(false);
            });
        }

        [Fact]
        public async Task IsBonusHasEmployee_WrongId_ShouldFalse()
        {

            await WithUnitOfWorkAsync(async () =>
            {
                long bonusId = 0;

                var result = await _bonusManager.IsBonusHasEmployee(bonusId);
                result.ShouldBe(false);
            });
        }


        //GetListDate
        [Fact]
        public void GetListDate_Success()
        {

            WithUnitOfWork(() =>
            {
                int expectSize = 5;

                var results = _bonusManager.GetListDate();
                Assert.NotNull(results);
                Assert.Equal(expectSize, results.Count);

                for (int i = 0; i < results.Count; i++)
                {
                    if (i == results.Count - 1) return;

                    results[i].ShouldBeGreaterThanOrEqualTo(results[i + 1]);
                }
            });
        }


        //GetListMonthFilter
        [Fact]
        public async Task GetListMonthFilter_ReturnRightListItem()
        {
            List<DateTime> listTimeUniqueExpected = new List<DateTime>
            {
                DateTime.Parse("2023-02-15 00:00:00"),
                DateTime.Parse("2023-01-15 00:00:00"),
                DateTime.Parse("2022-12-15 00:00:00"),
            };
            await WithUnitOfWorkAsync(async () =>
            {
                var AR = await _bonusManager.GetListMonthFilter();
                Assert.NotNull(AR);
                AR.Count.ShouldBeEquivalentTo(listTimeUniqueExpected.Count);

                for (int i = 0; i < AR.Count; i++)
                {
                    var objAR = AR.Skip(i).Take(1).FirstOrDefault();
                    var objER = listTimeUniqueExpected.Skip(i).Take(1).FirstOrDefault();

                    objAR.ToString().ShouldBeEquivalentTo(objER.ToString());
                }
            });
        }


        //ChangeStatus
        [Fact]
        public async Task ChangeStatus_WrongBonusId_EntityNotFoundException()
        {

            await WithUnitOfWorkAsync(async () =>
            {
                long bonusId = 0;

                await Assert.ThrowsAsync<EntityNotFoundException>(async () =>
                {
                    var AR = await _bonusManager.ChangeStatus(bonusId);
                });
            });
        }

        [Fact]
        public async Task ChangeStatus_RightBonusId_Success()
        {
            long bonusId = 61;

            await WithUnitOfWorkAsync(async () =>
            {
                var result = await _bonusManager.ChangeStatus(bonusId);
                result.ShouldBeEquivalentTo(bonusId);
            });

            WithUnitOfWork(() =>
            {
                var entity = _workScope.GetAsync<Bonus>(bonusId).Result;
                Assert.Equal(bonusId, entity.Id);
                Assert.False(entity.IsActive);

            });
        }


        //GetBonusDetail
        [Fact]
        public async Task GetBonusDetail_WrongBonusId_ShouldNull()
        {

            await WithUnitOfWorkAsync(async () =>
            {
                long bonusId = 0;

                var AR = await _bonusManager.GetBonusDetail(bonusId);
                Assert.Null(AR);
            });
        }

        [Fact]
        public async Task GetBonusDetail_RightBonusId_ReturnRightListItem()
        {

            GetDetailBonusDto bonusDetailExpected = new GetDetailBonusDto()
            {
                Id = 60,
                Name = "Bonus thưởng dự án 02/2023",
                IsActive = false,
                ApplyMonth = DateTime.Parse("2023-02-15 00:00:00"),
                CreationTime = DateTime.Parse("2022-12-19 13:47:33.02682"),
                LastModificationTime = DateTime.Parse("2022-12-19 14:12:25.820548"),
                FullNameCreation = "admin admin",
                FullNameModification = "admin admin",
            };

            await WithUnitOfWorkAsync(async () =>
            {
                int bonusId = 60;
                var AR = await _bonusManager.GetBonusDetail(bonusId);

                Assert.NotNull(AR);

                AR.Id.ShouldBeEquivalentTo(bonusDetailExpected.Id);
                AR.Name.ShouldBeEquivalentTo(bonusDetailExpected.Name);
                AR.IsActive.ShouldBeEquivalentTo(bonusDetailExpected?.IsActive);
                Assert.Equal(AR.ApplyMonth.ToString(), bonusDetailExpected?.ApplyMonth.ToString());
                Assert.Equal(AR.CreationTime.ToString(), bonusDetailExpected?.CreationTime.ToString());
                Assert.Equal(AR.LastModificationTime.ToString(), bonusDetailExpected?.LastModificationTime.ToString());
                AR.FullNameCreation.ShouldBeEquivalentTo(bonusDetailExpected?.FullNameCreation);
                AR.FullNameModification.ShouldBeEquivalentTo(bonusDetailExpected?.FullNameModification);
            });
        }


        //GetAllBonusEmployee
        [Fact]
        public async Task GetAllBonusEmployee_WrongBonusId_NewDto_NullReferenceException()
        {

            long bonusId = 0;
            var dto = new GetBonusEmployeeInputDto();

            await WithUnitOfWorkAsync(async () =>
            {

                await Assert.ThrowsAsync<NullReferenceException>(async () =>
                {
                    await _bonusManager.GetAllBonusEmployee(bonusId, dto);
                });

            });
        }

        //GetAllBonusEmployee
        [Fact]
        public async Task GetAllBonusEmployee_WrongBonusId_DtoMissGridParam_NullReferenceException()
        {
            // Wrong BonusId
            // Right Dto with StatusId, missing GridParam

            long bonusId = 0;

            var statusIds = new List<EmployeeStatus> {
                EmployeeStatus.MaternityLeave
            };
            var dto = new GetBonusEmployeeInputDto
            {
                StatusIds = statusIds,
            };

            await WithUnitOfWorkAsync(async () =>
            {

                await Assert.ThrowsAsync<NullReferenceException>(async () =>
                {
                    await _bonusManager.GetAllBonusEmployee(bonusId, dto);
                });

            });
        }

        [Fact]
        public async Task GetAllBonusEmployee_WrongBonusId_RightDtoStatusIds_ReturnEmptyList()
        {
            // Wrong BonusId
            // Right Dto(2/10 fields) include StatusIds, Right gridParam

            long bonusId = 0;
            var gridParam = new GridParam();
            var statusIds = new List<EmployeeStatus> {
                EmployeeStatus.Working,
                EmployeeStatus.Pausing,
                EmployeeStatus.MaternityLeave
            };
            var dto = new GetBonusEmployeeInputDto
            {
                StatusIds = statusIds,
                GridParam = gridParam
            };

            await WithUnitOfWorkAsync(async () =>
            {

                var result = await _bonusManager.GetAllBonusEmployee(bonusId, dto);
                Assert.Equal(0, result.Items.Count);
            });
        }

        [Fact]
        public async Task GetAllBonusEmployee_RightBonusId_RightDtoStatusIds_ReturnGridResult_1()
        {
            // Right BonusId
            // Right Dto(2/10 fields) include StatusIds, Right gridParam => case 1 StatusIds
            long bonusId = 54;
            var gridParam = new GridParam();

            var statusIds = new List<EmployeeStatus> {
                EmployeeStatus.Pausing,
            };

            var dto = new GetBonusEmployeeInputDto
            {
                StatusIds = statusIds,
                GridParam = gridParam
            };


            await WithUnitOfWorkAsync(async () =>
            {
                var result = await _bonusManager.GetAllBonusEmployee(bonusId, dto);
                var expectSize = 0;
                Assert.NotNull(result);
                Assert.Equal(expectSize, result.TotalCount);
            });
        }

        [Fact]
        public async Task GetAllBonusEmployee_RightBonusId_RightDtooStatusIds_ReturnGridResult_2()
        {
            // Right BonusId
            // Right Dto(2/10 fields) include StatusIds, gridParam => case multi StatusIds
            long bonusId = 54;
            var gridParam = new GridParam();

            var statusIds = new List<EmployeeStatus> {
                EmployeeStatus.Working,
                EmployeeStatus.Pausing,
            };

            var dto = new GetBonusEmployeeInputDto
            {
                StatusIds = statusIds,
                GridParam = gridParam
            };


            await WithUnitOfWorkAsync(async () =>
            {
                var result = await _bonusManager.GetAllBonusEmployee(bonusId, dto);
                var expectSize = 5;
                Assert.NotNull(result);
                Assert.Equal(expectSize, result.TotalCount);
            });
        }

        [Fact]
        public async Task GetAllBonusEmployee_RightBonusId_RightDtoBranchIds_ReturnGridResult_1()
        {
            // Right BonusId
            // Right Dto(2/10 fields) include BranchIds, gridParam => case 1 BranchIds

            long bonusId = 54;

            var gridParam = new GridParam();
            var branchIds = new List<long> { 100 };

            var dto = new GetBonusEmployeeInputDto
            {
                BranchIds = branchIds,
                GridParam = gridParam
            };

            var listExpected = new List<GetBonusEmployeeDto>
            {
                new GetBonusEmployeeDto
                {
                    BonusId = 54,
                    EmployeeId = 887,
                    Money = 300000.0,
                    Note = "Thưởng sinh nhât tháng 12",
                    BonusName = "Thưởng sinh nhât tháng 12",
                    ApplyDate = DateTime.Parse("2022-12-15 00:00:00"),
                    LastModificationTime = DateTime.Parse("2022-12-15 13:49:59.97897"),
                    FullNameModification = "admin admin",
                },
            };


            await WithUnitOfWorkAsync(async () =>
            {
                var listActual = await _bonusManager.GetAllBonusEmployee(bonusId, dto);

                Assert.NotNull(listActual);
                Assert.Equal(listExpected.Count, listActual.TotalCount);

                helpCheckRightItemForGetAllBonusEmployee(listActual, listExpected);
            });
        }

        [Fact]
        public async Task GetAllBonusEmployee_RightBonusId_RightDtoBranchIds_ReturnGridResult_2()
        {
            // Right BonusId
            // Right Dto(2/10 fields) include BranchIds, gridParam => case multi BranchIds

            long bonusId = 54;

            var gridParam = new GridParam();
            var branchIds = new List<long> { 100, 93 };

            var dto = new GetBonusEmployeeInputDto
            {
                BranchIds = branchIds,
                GridParam = gridParam
            };

            var listExpected = new List<GetBonusEmployeeDto>
            {
                new GetBonusEmployeeDto
                {
                    BonusId = 54,
                    EmployeeId = 887,
                    Money = 300000.0,
                    Note = "Thưởng sinh nhât tháng 12",
                    BonusName = "Thưởng sinh nhât tháng 12",
                    ApplyDate = DateTime.Parse("2022-12-15 00:00:00"),
                    LastModificationTime = DateTime.Parse("2022-12-15 13:49:59.97897"),
                    FullNameModification = "admin admin",
                },
                new GetBonusEmployeeDto
                {
                    BonusId = 54,
                    EmployeeId = 890,
                    Money = 300000.0,
                    Note = "Thưởng sinh nhât tháng 12",
                    BonusName = "Thưởng sinh nhât tháng 12",
                    ApplyDate = DateTime.Parse("2022-12-15 00:00:00"),
                    LastModificationTime = DateTime.Parse("2022-12-15 13:49:59.976587"),
                    FullNameModification = "admin admin",
                },
                new GetBonusEmployeeDto
                {
                    BonusId = 54,
                    EmployeeId = 889,
                    Money = 300000.0,
                    Note = "Thưởng sinh nhât tháng 12",
                    BonusName = "Thưởng sinh nhât tháng 12",
                    ApplyDate = DateTime.Parse("2022-12-15 00:00:00"),
                    LastModificationTime = DateTime.Parse("2022-12-15 13:49:59.973999"),
                    FullNameModification = "admin admin",
                },

            };


            await WithUnitOfWorkAsync(async () =>
            {
                var listActual = await _bonusManager.GetAllBonusEmployee(bonusId, dto);

                Assert.NotNull(listActual);
                Assert.Equal(listExpected.Count, listActual.TotalCount);

                helpCheckRightItemForGetAllBonusEmployee(listActual, listExpected);
            });
        }

        [Fact]
        public async Task GetAllBonusEmployee_RightBonusId_RightDtoUserTypes_ReturnGridResult_1()
        {
            // Right BonusId
            // Right Dto(2/10 fields) include UserTypes, gridParam => case 1 UserTypes

            long bonusId = 54;

            var gridParam = new GridParam();
            var userTypes = new List<UserType> { UserType.Staff };

            var dto = new GetBonusEmployeeInputDto
            {
                UserTypes = userTypes,
                GridParam = gridParam
            };

            var listExpected = new List<GetBonusEmployeeDto>
            {
                new GetBonusEmployeeDto
                {
                    BonusId = 54,
                    EmployeeId = 887,
                    Money = 300000.0,
                    Note = "Thưởng sinh nhât tháng 12",
                    BonusName = "Thưởng sinh nhât tháng 12",
                    ApplyDate = DateTime.Parse("2022-12-15 00:00:00"),
                    LastModificationTime = DateTime.Parse("2022-12-15 13:49:59.97897"),
                    FullNameModification = "admin admin",
                },
            };


            await WithUnitOfWorkAsync(async () =>
            {
                var listActual = await _bonusManager.GetAllBonusEmployee(bonusId, dto);

                Assert.NotNull(listActual);
                Assert.Equal(listExpected.Count, listActual.TotalCount);

                helpCheckRightItemForGetAllBonusEmployee(listActual, listExpected);
            });
        }

        [Fact]
        public async Task GetAllBonusEmployee_RightBonusId_RightDtoUserTypes_ReturnGridResult_2()
        {
            // Right BonusId
            // Right Dto(2/10 fields) include UserTypes, gridParam => case multi UserTypes

            long bonusId = 54;

            var gridParam = new GridParam();
            var userTypes = new List<UserType> { UserType.Staff, UserType.ProbationaryStaff };

            var dto = new GetBonusEmployeeInputDto
            {
                UserTypes = userTypes,
                GridParam = gridParam
            };

            var listExpected = new List<GetBonusEmployeeDto>
            {
                new GetBonusEmployeeDto
                {
                    BonusId = 54,
                    EmployeeId = 887,
                    Money = 300000.0,
                    Note = "Thưởng sinh nhât tháng 12",
                    BonusName = "Thưởng sinh nhât tháng 12",
                    ApplyDate = DateTime.Parse("2022-12-15 00:00:00"),
                    LastModificationTime = DateTime.Parse("2022-12-15 13:49:59.97897"),
                    FullNameModification = "admin admin",
                },
                new GetBonusEmployeeDto
                {
                    BonusId = 54,
                    EmployeeId = 890,
                    Money = 300000.0,
                    Note = "Thưởng sinh nhât tháng 12",
                    BonusName = "Thưởng sinh nhât tháng 12",
                    ApplyDate = DateTime.Parse("2022-12-15 00:00:00"),
                    LastModificationTime = DateTime.Parse("2022-12-15 13:49:59.976587"),
                    FullNameModification = "admin admin",
                },
                new GetBonusEmployeeDto
                {
                    BonusId = 54,
                    EmployeeId = 889,
                    Money = 300000.0,
                    Note = "Thưởng sinh nhât tháng 12",
                    BonusName = "Thưởng sinh nhât tháng 12",
                    ApplyDate = DateTime.Parse("2022-12-15 00:00:00"),
                    LastModificationTime = DateTime.Parse("2022-12-15 13:49:59.973999"),
                    FullNameModification = "admin admin",
                },
            };


            await WithUnitOfWorkAsync(async () =>
            {
                var listActual = await _bonusManager.GetAllBonusEmployee(bonusId, dto);

                Assert.NotNull(listActual);
                Assert.Equal(listExpected.Count, listActual.TotalCount);

                helpCheckRightItemForGetAllBonusEmployee(listActual, listExpected);
            });
        }

        [Fact]
        public async Task GetAllBonusEmployee_RightBonusId_RightDtoLevelIds_ReturnGridResult_1()
        {
            // Right BonusId
            // Right Dto(2/10 fields) include LevelIds, gridParam => case 1 LevelIds

            long bonusId = 54;

            var gridParam = new GridParam();
            var userTypes = new List<long> { 317 };

            var dto = new GetBonusEmployeeInputDto
            {
                LevelIds = userTypes,
                GridParam = gridParam
            };

            var listExpected = new List<GetBonusEmployeeDto>
            {
                new GetBonusEmployeeDto
                {
                    BonusId = 54,
                    EmployeeId = 887,
                    Money = 300000.0,
                    Note = "Thưởng sinh nhât tháng 12",
                    BonusName = "Thưởng sinh nhât tháng 12",
                    ApplyDate = DateTime.Parse("2022-12-15 00:00:00"),
                    LastModificationTime = DateTime.Parse("2022-12-15 13:49:59.97897"),
                    FullNameModification = "admin admin",
                },
            };


            await WithUnitOfWorkAsync(async () =>
            {
                var listActual = await _bonusManager.GetAllBonusEmployee(bonusId, dto);

                Assert.NotNull(listActual);
                Assert.Equal(listExpected.Count, listActual.TotalCount);

                helpCheckRightItemForGetAllBonusEmployee(listActual, listExpected);
            });
        }

        [Fact]
        public async Task GetAllBonusEmployee_RightBonusId_RightDtoLevelIds_ReturnGridResult_2()
        {
            // Right BonusId,
            // Right Dto(2/10 fields) include LevelIds, gridParam => case multi LevelIds

            long bonusId = 54;

            var gridParam = new GridParam();
            var userTypes = new List<long> { 317, 316 };

            var dto = new GetBonusEmployeeInputDto
            {
                LevelIds = userTypes,
                GridParam = gridParam
            };

            var listExpected = new List<GetBonusEmployeeDto>
            {
                new GetBonusEmployeeDto
                {
                    BonusId = 54,
                    EmployeeId = 887,
                    Money = 300000.0,
                    Note = "Thưởng sinh nhât tháng 12",
                    BonusName = "Thưởng sinh nhât tháng 12",
                    ApplyDate = DateTime.Parse("2022-12-15 00:00:00"),
                    LastModificationTime = DateTime.Parse("2022-12-15 13:49:59.97897"),
                    FullNameModification = "admin admin",
                },
                new GetBonusEmployeeDto
                {
                    BonusId = 54,
                    EmployeeId = 890,
                    Money = 300000.0,
                    Note = "Thưởng sinh nhât tháng 12",
                    BonusName = "Thưởng sinh nhât tháng 12",
                    ApplyDate = DateTime.Parse("2022-12-15 00:00:00"),
                    LastModificationTime = DateTime.Parse("2022-12-15 13:49:59.976587"),
                    FullNameModification = "admin admin",
                },
            };


            await WithUnitOfWorkAsync(async () =>
            {
                var listActual = await _bonusManager.GetAllBonusEmployee(bonusId, dto);

                Assert.NotNull(listActual);
                Assert.Equal(listExpected.Count, listActual.TotalCount);

                helpCheckRightItemForGetAllBonusEmployee(listActual, listExpected);
            });
        }

        [Fact]
        public async Task GetAllBonusEmployee_RightBonusId_RightDtoJobPositionIds_ReturnGridResult_1()
        {
            // Right BonusId,
            // Right Dto(2/10 fields) include JobPositionIds, gridParam => case 1 JobPositionIds

            long bonusId = 54;

            var gridParam = new GridParam();
            var jobPositionIds = new List<long> { 48 };

            var dto = new GetBonusEmployeeInputDto
            {
                JobPositionIds = jobPositionIds,
                GridParam = gridParam

            };

            var listExpected = new List<GetBonusEmployeeDto>
            {
                new GetBonusEmployeeDto
                {
                    BonusId = 54,
                    EmployeeId = 890,
                    Money = 300000.0,
                    Note = "Thưởng sinh nhât tháng 12",
                    BonusName = "Thưởng sinh nhât tháng 12",
                    ApplyDate = DateTime.Parse("2022-12-15 00:00:00"),
                    LastModificationTime = DateTime.Parse("2022-12-15 13:49:59.976587"),
                    FullNameModification = "admin admin",
                },
            };


            await WithUnitOfWorkAsync(async () =>
            {
                var listActual = await _bonusManager.GetAllBonusEmployee(bonusId, dto);

                Assert.NotNull(listActual);
                Assert.Equal(listExpected.Count, listActual.TotalCount);
                helpCheckRightItemForGetAllBonusEmployee(listActual, listExpected);
            });
        }

        [Fact]
        public async Task GetAllBonusEmployee_RightBonusId_RightDtoJobPositionIds_ReturnGridResult_2()
        {
            // Right BonusId
            // Right Dto(2/10 fields) include JobPositionIds, gridParam => case multi JobPositionIds

            long bonusId = 54;

            var gridParam = new GridParam();
            var jobPositionIds = new List<long> { 48, 55 };

            var dto = new GetBonusEmployeeInputDto
            {
                JobPositionIds = jobPositionIds,
                GridParam = gridParam
            };

            var listExpected = new List<GetBonusEmployeeDto>
            {
                new GetBonusEmployeeDto
                {
                    BonusId = 54,
                    EmployeeId = 890,
                    Money = 300000.0,
                    Note = "Thưởng sinh nhât tháng 12",
                    BonusName = "Thưởng sinh nhât tháng 12",
                    ApplyDate = DateTime.Parse("2022-12-15 00:00:00"),
                    LastModificationTime = DateTime.Parse("2022-12-15 13:49:59.976587"),
                    FullNameModification = "admin admin",
                },
                new GetBonusEmployeeDto
                {
                    BonusId = 54,
                    EmployeeId = 891,
                    Money = 300000.0,
                    Note = "Thưởng sinh nhât tháng 12",
                    BonusName = "Thưởng sinh nhât tháng 12",
                    ApplyDate = DateTime.Parse("2022-12-15 00:00:00"),
                    LastModificationTime = DateTime.Parse("2022-12-15 13:49:59.916976"),
                    FullNameModification = "admin admin",
                },
            };


            await WithUnitOfWorkAsync(async () =>
            {
                var listActual = await _bonusManager.GetAllBonusEmployee(bonusId, dto);

                Assert.NotNull(listActual);
                Assert.Equal(listExpected.Count, listActual.TotalCount);
                helpCheckRightItemForGetAllBonusEmployee(listActual, listExpected);
            });
        }

        [Fact]
        public async Task GetAllBonusEmployee_RightBonusId_RightDtoTeamIds_ReturnGridResult_1()
        {
            // Right BonusId
            // Right Dto(2/10 fields) include TeamIds, gridParam => case 1 TeamIds

            long bonusId = 54;

            var gridParam = new GridParam();
            var teamIds = new List<long> { 42 };

            var dto = new GetBonusEmployeeInputDto
            {
                TeamIds = teamIds,
                GridParam = gridParam
            };

            var listExpected = new List<GetBonusEmployeeDto>
            {
                new GetBonusEmployeeDto
                {
                    BonusId = 54,
                    EmployeeId = 887,
                    Money = 300000.0,
                    Note = "Thưởng sinh nhât tháng 12",
                    BonusName = "Thưởng sinh nhât tháng 12",
                    ApplyDate = DateTime.Parse("2022-12-15 00:00:00"),
                    LastModificationTime = DateTime.Parse("2022-12-15 13:49:59.97897"),
                    FullNameModification = "admin admin",
                },
            };


            await WithUnitOfWorkAsync(async () =>
            {
                var listActual = await _bonusManager.GetAllBonusEmployee(bonusId, dto);

                Assert.NotNull(listActual);
                Assert.Equal(listExpected.Count, listActual.TotalCount);
                helpCheckRightItemForGetAllBonusEmployee(listActual, listExpected);
            });
        }

        [Fact]
        public async Task GetAllBonusEmployee_RightBonusId_RightDtoTeamIds_ReturnGridResult_2()
        {
            // Right BonusId,
            // Right Dto(2/10 fields) include TeamIds, gridParam => case multi TeamIds

            long bonusId = 54;

            var gridParam = new GridParam();
            var teamIds = new List<long> { 41, 42, 43, 45 };

            var dto = new GetBonusEmployeeInputDto
            {
                TeamIds = teamIds,
                GridParam = gridParam
            };

            var listExpected = new List<GetBonusEmployeeDto>
            {
                new GetBonusEmployeeDto
                {
                    BonusId = 54,
                    EmployeeId = 887,
                    Money = 300000.0,
                    Note = "Thưởng sinh nhât tháng 12",
                    BonusName = "Thưởng sinh nhât tháng 12",
                    ApplyDate = DateTime.Parse("2022-12-15 00:00:00"),
                    LastModificationTime = DateTime.Parse("2022-12-15 13:49:59.97897"),
                    FullNameModification = "admin admin",
                },
            };


            await WithUnitOfWorkAsync(async () =>
            {
                var listActual = await _bonusManager.GetAllBonusEmployee(bonusId, dto);

                Assert.NotNull(listActual);
                Assert.Equal(listExpected.Count, listActual.TotalCount);
                helpCheckRightItemForGetAllBonusEmployee(listActual, listExpected);
            });
        }

        [Fact]
        public async Task GetAllBonusEmployee_RightBonusId_RightDtoTeamIds_ReturnGridResult_3()
        {
            // Right BonusId,
            // Right Dto(3/10 fields) include TeamIds, gridParam => case multi TeamIds, IsAndCondition false => should query one

            long bonusId = 54;

            var gridParam = new GridParam();
            var teamIds = new List<long> { 41, 42, 43, 45 };

            var dto = new GetBonusEmployeeInputDto
            {
                TeamIds = teamIds,
                GridParam = gridParam,
                IsAndCondition = false
            };

            var listExpected = new List<GetBonusEmployeeDto>
            {
                new GetBonusEmployeeDto
                {
                    BonusId = 54,
                    EmployeeId = 887,
                    Money = 300000.0,
                    Note = "Thưởng sinh nhât tháng 12",
                    BonusName = "Thưởng sinh nhât tháng 12",
                    ApplyDate = DateTime.Parse("2022-12-15 00:00:00"),
                    LastModificationTime = DateTime.Parse("2022-12-15 13:49:59.97897"),
                    FullNameModification = "admin admin",
                },
            };


            await WithUnitOfWorkAsync(async () =>
            {
                var listActual = await _bonusManager.GetAllBonusEmployee(bonusId, dto);

                Assert.NotNull(listActual);
                Assert.Equal(listExpected.Count, listActual.TotalCount);
                helpCheckRightItemForGetAllBonusEmployee(listActual, listExpected);
            });
        }

        [Fact]
        public async Task GetAllBonusEmployee_RightBonusId_RightDtoTeamIds_ReturnListEmpty()
        {
            // Right BonusId, Right Dto(2/10 fields) include TeamIds, Right gridParam => case 1 TeamIds
            // Wrong teamdId

            long bonusId = 54;

            var gridParam = new GridParam();
            var teamIds = new List<long> { -1 };

            var dto = new GetBonusEmployeeInputDto
            {
                TeamIds = teamIds,
                GridParam = gridParam
            };


            await WithUnitOfWorkAsync(async () =>
            {
                var listActual = await _bonusManager.GetAllBonusEmployee(bonusId, dto);

                Assert.NotNull(listActual);
                Assert.Equal(0, listActual.TotalCount);
            });
        }

        [Fact]
        public async Task GetAllBonusEmployee_RightParams_ReturnGridResult_1()
        {
            // Right BonusId,
            // Right Dto(3/10 fields) include 1 branchIds, 1 UserTypes

            long bonusId = 54;

            var gridParam = new GridParam();
            var userTypes = new List<UserType> { UserType.Staff };
            var branchIds = new List<long> { 100 };

            var dto = new GetBonusEmployeeInputDto
            {
                BranchIds = branchIds,
                UserTypes = userTypes,
                GridParam = gridParam
            };

            var listExpected = new List<GetBonusEmployeeDto>
            {
                new GetBonusEmployeeDto
                {
                    BonusId = 54,
                    EmployeeId = 887,
                    Money = 300000.0,
                    Note = "Thưởng sinh nhât tháng 12",
                    BonusName = "Thưởng sinh nhât tháng 12",
                    ApplyDate = DateTime.Parse("2022-12-15 00:00:00"),
                    LastModificationTime = DateTime.Parse("2022-12-15 13:49:59.97897"),
                    FullNameModification = "admin admin",
                },
            };


            await WithUnitOfWorkAsync(async () =>
            {
                var listActual = await _bonusManager.GetAllBonusEmployee(bonusId, dto);

                Assert.NotNull(listActual);
                Assert.Equal(listExpected.Count, listActual.TotalCount);

                helpCheckRightItemForGetAllBonusEmployee(listActual, listExpected);
            });
        }

        [Fact]
        public async Task GetAllBonusEmployee_RightParams_ReturnGridResult_2()
        {
            // Right BonusId,
            // Right Dto(3/10 fields) include 1 branchIds, 2 UserTypes

            long bonusId = 54;

            var gridParam = new GridParam();
            var userTypes = new List<UserType> { UserType.Staff, UserType.ProbationaryStaff };
            var branchIds = new List<long> { 100 };

            var dto = new GetBonusEmployeeInputDto
            {
                BranchIds = branchIds,
                UserTypes = userTypes,
                GridParam = gridParam
            };

            var listExpected = new List<GetBonusEmployeeDto>
            {
                new GetBonusEmployeeDto
                {
                    BonusId = 54,
                    EmployeeId = 887,
                    Money = 300000.0,
                    Note = "Thưởng sinh nhât tháng 12",
                    BonusName = "Thưởng sinh nhât tháng 12",
                    ApplyDate = DateTime.Parse("2022-12-15 00:00:00"),
                    LastModificationTime = DateTime.Parse("2022-12-15 13:49:59.97897"),
                    FullNameModification = "admin admin",
                },
            };


            await WithUnitOfWorkAsync(async () =>
            {
                var listActual = await _bonusManager.GetAllBonusEmployee(bonusId, dto);

                Assert.NotNull(listActual);
                Assert.Equal(listExpected.Count, listActual.TotalCount);

                helpCheckRightItemForGetAllBonusEmployee(listActual, listExpected);
            });
        }

        [Fact]
        public async Task GetAllBonusEmployee_RightParams_ReturnGridResult_3()
        {
            // Right BonusId,
            // Right Dto(3/10 fields) include 2 branchIds, 2 UserTypes

            long bonusId = 54;

            var gridParam = new GridParam();
            var userTypes = new List<UserType> { UserType.Staff, UserType.ProbationaryStaff };
            var branchIds = new List<long> { 100, 93 };

            var dto = new GetBonusEmployeeInputDto
            {
                BranchIds = branchIds,
                UserTypes = userTypes,
                GridParam = gridParam
            };

            var listExpected = new List<GetBonusEmployeeDto>
            {
                new GetBonusEmployeeDto
                {
                    BonusId = 54,
                    EmployeeId = 887,
                    Money = 300000.0,
                    Note = "Thưởng sinh nhât tháng 12",
                    BonusName = "Thưởng sinh nhât tháng 12",
                    ApplyDate = DateTime.Parse("2022-12-15 00:00:00"),
                    LastModificationTime = DateTime.Parse("2022-12-15 13:49:59.97897"),
                    FullNameModification = "admin admin",
                },
                new GetBonusEmployeeDto
                {
                    BonusId = 54,
                    EmployeeId = 890,
                    Money = 300000.0,
                    Note = "Thưởng sinh nhât tháng 12",
                    BonusName = "Thưởng sinh nhât tháng 12",
                    ApplyDate = DateTime.Parse("2022-12-15 00:00:00"),
                    LastModificationTime = DateTime.Parse("2022-12-15 13:49:59.976587"),
                    FullNameModification = "admin admin",
                },
                new GetBonusEmployeeDto
                {
                    BonusId = 54,
                    EmployeeId = 889,
                    Money = 300000.0,
                    Note = "Thưởng sinh nhât tháng 12",
                    BonusName = "Thưởng sinh nhât tháng 12",
                    ApplyDate = DateTime.Parse("2022-12-15 00:00:00"),
                    LastModificationTime = DateTime.Parse("2022-12-15 13:49:59.973999"),
                    FullNameModification = "admin admin",
                },
            };


            await WithUnitOfWorkAsync(async () =>
            {
                var listActual = await _bonusManager.GetAllBonusEmployee(bonusId, dto);

                Assert.NotNull(listActual);
                Assert.Equal(listExpected.Count, listActual.TotalCount);

                helpCheckRightItemForGetAllBonusEmployee(listActual, listExpected);
            });
        }

        [Fact]
        public async Task GetAllBonusEmployee_RightParams_ReturnGridResult_4()
        {
            // Right BonusId,
            // Right Dto(4/10 fields) include 2 branchIds, 2 UserTypes, 2 StatusId

            long bonusId = 54;

            var gridParam = new GridParam();
            var userTypes = new List<UserType> { UserType.Staff, UserType.ProbationaryStaff }; 
            var statusId = new List<EmployeeStatus> { EmployeeStatus.Pausing, EmployeeStatus.Working };
            var branchIds = new List<long> { 100, 93 };

            var dto = new GetBonusEmployeeInputDto
            {
                BranchIds = branchIds,
                UserTypes = userTypes,
                GridParam = gridParam,
                StatusIds = statusId,
            };

            var listExpected = new List<GetBonusEmployeeDto>
            {
                new GetBonusEmployeeDto
                {
                    BonusId = 54,
                    EmployeeId = 887,
                    Money = 300000.0,
                    Note = "Thưởng sinh nhât tháng 12",
                    BonusName = "Thưởng sinh nhât tháng 12",
                    ApplyDate = DateTime.Parse("2022-12-15 00:00:00"),
                    LastModificationTime = DateTime.Parse("2022-12-15 13:49:59.97897"),
                    FullNameModification = "admin admin",
                },
                new GetBonusEmployeeDto
                {
                    BonusId = 54,
                    EmployeeId = 890,
                    Money = 300000.0,
                    Note = "Thưởng sinh nhât tháng 12",
                    BonusName = "Thưởng sinh nhât tháng 12",
                    ApplyDate = DateTime.Parse("2022-12-15 00:00:00"),
                    LastModificationTime = DateTime.Parse("2022-12-15 13:49:59.976587"),
                    FullNameModification = "admin admin",
                },
                new GetBonusEmployeeDto
                {
                    BonusId = 54,
                    EmployeeId = 889,
                    Money = 300000.0,
                    Note = "Thưởng sinh nhât tháng 12",
                    BonusName = "Thưởng sinh nhât tháng 12",
                    ApplyDate = DateTime.Parse("2022-12-15 00:00:00"),
                    LastModificationTime = DateTime.Parse("2022-12-15 13:49:59.973999"),
                    FullNameModification = "admin admin",
                },
            };


            await WithUnitOfWorkAsync(async () =>
            {
                var listActual = await _bonusManager.GetAllBonusEmployee(bonusId, dto);

                Assert.NotNull(listActual);
                Assert.Equal(listExpected.Count, listActual.TotalCount);

                helpCheckRightItemForGetAllBonusEmployee(listActual, listExpected);
            });
        }

        [Fact]
        public async Task GetAllBonusEmployee_RightParams_ReturnGridResult_5()
        {
            // Right BonusId,
            // Right Dto(4/10 fields) include 2 branchIds, 2 UserTypes, 1 StatusId

            long bonusId = 54;

            var gridParam = new GridParam();
            var userTypes = new List<UserType> { UserType.Staff, UserType.ProbationaryStaff };
            var statusId = new List<EmployeeStatus> { EmployeeStatus.Pausing };
            var branchIds = new List<long> { 100, 93 };

            var dto = new GetBonusEmployeeInputDto
            {
                BranchIds = branchIds,
                UserTypes = userTypes,
                GridParam = gridParam,
                StatusIds = statusId,
            };

            await WithUnitOfWorkAsync(async () =>
            {
                var listActual = await _bonusManager.GetAllBonusEmployee(bonusId, dto);

                Assert.NotNull(listActual);
                Assert.Equal(0, listActual.TotalCount);
            });
        }

        [Fact]
        public async Task GetAllBonusEmployee_CheckPaging_MaxResultParam()
        {
            // Right BonusId,
            // Right Dto(4/10 fields) include 2 branchIds, 2 UserTypes, 2 StatusId

            long bonusId = 54;

            var gridParam = new GridParam()
            {
                MaxResultCount = 2,
            };
            var userTypes = new List<UserType> { UserType.Staff, UserType.ProbationaryStaff };
            var statusId = new List<EmployeeStatus> { EmployeeStatus.Working };
            var branchIds = new List<long> { 100, 93 };

            var dto = new GetBonusEmployeeInputDto
            {
                BranchIds = branchIds,
                UserTypes = userTypes,
                GridParam = gridParam,
                StatusIds = statusId,
            };

            var listExpected = new List<GetBonusEmployeeDto>
            {
                new GetBonusEmployeeDto
                {
                    BonusId = 54,
                    EmployeeId = 887,
                    Money = 300000.0,
                    Note = "Thưởng sinh nhât tháng 12",
                    BonusName = "Thưởng sinh nhât tháng 12",
                    ApplyDate = DateTime.Parse("2022-12-15 00:00:00"),
                    LastModificationTime = DateTime.Parse("2022-12-15 13:49:59.97897"),
                    FullNameModification = "admin admin",
                },
                new GetBonusEmployeeDto
                {
                    BonusId = 54,
                    EmployeeId = 890,
                    Money = 300000.0,
                    Note = "Thưởng sinh nhât tháng 12",
                    BonusName = "Thưởng sinh nhât tháng 12",
                    ApplyDate = DateTime.Parse("2022-12-15 00:00:00"),
                    LastModificationTime = DateTime.Parse("2022-12-15 13:49:59.976587"),
                    FullNameModification = "admin admin",
                },
            };


            await WithUnitOfWorkAsync(async () =>
            {
                var listActual = await _bonusManager.GetAllBonusEmployee(bonusId, dto);

                Assert.NotNull(listActual);
                var totalCount = 3;
                Assert.Equal(totalCount, listActual.TotalCount);
                Assert.Equal(listExpected.Count, listActual.Items.Count);

                helpCheckRightItemForGetAllBonusEmployee(listActual, listExpected);
            });
        }

        [Fact]
        public async Task GetAllBonusEmployee_CheckPaging_SkipParam()
        {
            // Right BonusId,
            // Right Dto(4/10 fields) include 2 branchIds, 2 UserTypes, 1 StatusId

            long bonusId = 54;

            var gridParam = new GridParam()
            {
               SkipCount = 2,
            };
            var userTypes = new List<UserType> { UserType.Staff, UserType.ProbationaryStaff };
            var statusId = new List<EmployeeStatus> { EmployeeStatus.Working };
            var branchIds = new List<long> { 100, 93 };

            var dto = new GetBonusEmployeeInputDto
            {
                BranchIds = branchIds,
                UserTypes = userTypes,
                GridParam = gridParam,
                StatusIds = statusId,
            };

            var listExpected = new List<GetBonusEmployeeDto>
            {
                new GetBonusEmployeeDto
                {
                    BonusId = 54,
                    EmployeeId = 889,
                    Money = 300000.0,
                    Note = "Thưởng sinh nhât tháng 12",
                    BonusName = "Thưởng sinh nhât tháng 12",
                    ApplyDate = DateTime.Parse("2022-12-15 00:00:00"),
                    LastModificationTime = DateTime.Parse("2022-12-15 13:49:59.973999"),
                    FullNameModification = "admin admin",
                },
            };


            await WithUnitOfWorkAsync(async () =>
            {
                var listActual = await _bonusManager.GetAllBonusEmployee(bonusId, dto);

                Assert.NotNull(listActual);
                var totalCount = 3;
                Assert.Equal(totalCount, listActual.TotalCount);
                Assert.Equal(listExpected.Count, listActual.Items.Count);

                helpCheckRightItemForGetAllBonusEmployee(listActual, listExpected);
            });
        }

        [Fact]
        public async Task GetAllBonusEmployee_CheckPaging_SearchParam()
        {
            // Right BonusId,
            // Right Dto(4/10 fields) include 2 branchIds, 2 UserTypes, 1 StatusId

            long bonusId = 54;

            var gridParam = new GridParam()
            {
                SearchText = "", //Need replace "Thưởng sinh nhât tháng 12" after update
            };
            var userTypes = new List<UserType> { UserType.Staff, UserType.ProbationaryStaff };
            var statusId = new List<EmployeeStatus> { EmployeeStatus.Working };
            var branchIds = new List<long> { 100, 93 };

            var dto = new GetBonusEmployeeInputDto
            {
                BranchIds = branchIds,
                UserTypes = userTypes,
                GridParam = gridParam,
                StatusIds = statusId,
            };

            var listExpected = new List<GetBonusEmployeeDto>
            {
                new GetBonusEmployeeDto
                {
                    BonusId = 54,
                    EmployeeId = 887,
                    Money = 300000.0,
                    Note = "Thưởng sinh nhât tháng 12",
                    BonusName = "Thưởng sinh nhât tháng 12",
                    ApplyDate = DateTime.Parse("2022-12-15 00:00:00"),
                    LastModificationTime = DateTime.Parse("2022-12-15 13:49:59.97897"),
                    FullNameModification = "admin admin",
                },
                new GetBonusEmployeeDto
                {
                    BonusId = 54,
                    EmployeeId = 890,
                    Money = 300000.0,
                    Note = "Thưởng sinh nhât tháng 12",
                    BonusName = "Thưởng sinh nhât tháng 12",
                    ApplyDate = DateTime.Parse("2022-12-15 00:00:00"),
                    LastModificationTime = DateTime.Parse("2022-12-15 13:49:59.976587"),
                    FullNameModification = "admin admin",
                },
                new GetBonusEmployeeDto
                {
                    BonusId = 54,
                    EmployeeId = 889,
                    Money = 300000.0,
                    Note = "Thưởng sinh nhât tháng 12",
                    BonusName = "Thưởng sinh nhât tháng 12",
                    ApplyDate = DateTime.Parse("2022-12-15 00:00:00"),
                    LastModificationTime = DateTime.Parse("2022-12-15 13:49:59.973999"),
                    FullNameModification = "admin admin",
                },
            };


            await WithUnitOfWorkAsync(async () =>
            {
                var listActual = await _bonusManager.GetAllBonusEmployee(bonusId, dto);

                Assert.NotNull(listActual);
                var totalCount = 3;
                Assert.Equal(totalCount, listActual.TotalCount);
                Assert.Equal(listExpected.Count, listActual.Items.Count);

                helpCheckRightItemForGetAllBonusEmployee(listActual, listExpected);
            });
        }

        [Fact]
        public async Task GetAllBonusEmployee_CheckPaging_SortParam()
        {
            // Right BonusId,
            // Right Dto(4/10 fields) include 2 branchIds, 2 UserTypes, 1 StatusId

            long bonusId = 54;

            var gridParam = new GridParam()
            {
                SortDirection = SortDirection.DESC,
                Sort = "employeeId",
            };
            var userTypes = new List<UserType> { UserType.Staff, UserType.ProbationaryStaff };
            var statusId = new List<EmployeeStatus> { EmployeeStatus.Working };
            var branchIds = new List<long> { 100, 93 };

            var dto = new GetBonusEmployeeInputDto
            {
                BranchIds = branchIds,
                UserTypes = userTypes,
                GridParam = gridParam,
                StatusIds = statusId,
            };

            var listExpected = new List<GetBonusEmployeeDto>
            {
                new GetBonusEmployeeDto
                {
                    BonusId = 54,
                    EmployeeId = 890,
                    Money = 300000.0,
                    Note = "Thưởng sinh nhât tháng 12",
                    BonusName = "Thưởng sinh nhât tháng 12",
                    ApplyDate = DateTime.Parse("2022-12-15 00:00:00"),
                    LastModificationTime = DateTime.Parse("2022-12-15 13:49:59.976587"),
                    FullNameModification = "admin admin",
                },
                new GetBonusEmployeeDto
                {
                    BonusId = 54,
                    EmployeeId = 889,
                    Money = 300000.0,
                    Note = "Thưởng sinh nhât tháng 12",
                    BonusName = "Thưởng sinh nhât tháng 12",
                    ApplyDate = DateTime.Parse("2022-12-15 00:00:00"),
                    LastModificationTime = DateTime.Parse("2022-12-15 13:49:59.973999"),
                    FullNameModification = "admin admin",
                },
                new GetBonusEmployeeDto
                {
                    BonusId = 54,
                    EmployeeId = 887,
                    Money = 300000.0,
                    Note = "Thưởng sinh nhât tháng 12",
                    BonusName = "Thưởng sinh nhât tháng 12",
                    ApplyDate = DateTime.Parse("2022-12-15 00:00:00"),
                    LastModificationTime = DateTime.Parse("2022-12-15 13:49:59.97897"),
                    FullNameModification = "admin admin",
                },
            };


            await WithUnitOfWorkAsync(async () =>
            {
                var listActual = await _bonusManager.GetAllBonusEmployee(bonusId, dto);

                Assert.NotNull(listActual);
                var totalCount = 3;
                Assert.Equal(totalCount, listActual.TotalCount);
                Assert.Equal(listExpected.Count, listActual.Items.Count);

                helpCheckRightItemForGetAllBonusEmployee(listActual, listExpected);
            });
        }


        //QuickAddEmployeeToBonus
        [Fact]
        public async Task QuickAddEmployeeToBonus_BonusIdNotActive_UserFriendlyException()
        {
            var addEmployeeToBonusDto = new AddEmployeeToBonusDto()
            {
                BonusId = 60,
                Money = 400,
                Note = "Thuong lucky draw",
                EmployeeIds = new List<long> { 899, 898 }

            };
            await WithUnitOfWorkAsync(async () =>
            {
                var exception = await Assert.ThrowsAsync<UserFriendlyException>(async () =>
                {
                    await _bonusManager.QuickAddEmployeeToBonus(addEmployeeToBonusDto);
                });

                var expectedErrorMsg = "Bonus not active";
                Assert.Equal(expectedErrorMsg, exception.Message);

            });
        }

        [Fact]
        public async Task QuickAddEmployeeToBonus_WrongBonusId_EntityNotFoundException()
        {
            var addEmployeeToBonusDto = new AddEmployeeToBonusDto()
            {
                BonusId = -1,
                Money = 400,
                Note = "Thuong lucky draw",
                EmployeeIds = new List<long> { 899, 898 }

            };
            await WithUnitOfWorkAsync(async () =>
            {
                await Assert.ThrowsAsync<EntityNotFoundException>(async () =>
                {
                    await _bonusManager.QuickAddEmployeeToBonus(addEmployeeToBonusDto);
                });
            });
        }

        [Fact]
        public async Task QuickAddEmployeeToBonus_NewListEmployees_Success()
        {
            var bonusEmployees = new List<BonusEmployee>();
            long countBef = 0;

            await WithUnitOfWorkAsync(async () =>
            {
                bonusEmployees = _workScope.GetAll<BonusEmployee>().ToList();
                countBef = bonusEmployees.Count;

                var addEmployeeToBonusDto = new AddEmployeeToBonusDto()
                {
                     BonusId = 61,
                     Money =  400,
                     Note = "Thuong lucky draw",
                     EmployeeIds = new List<long> { 899, 898}
    
                };
                var result = await _bonusManager.QuickAddEmployeeToBonus(addEmployeeToBonusDto);

                Assert.NotNull(result);
                Assert.Equal(addEmployeeToBonusDto.Money, result.Money);
                Assert.Equal(addEmployeeToBonusDto.Note, result.Note);
                Assert.Equal(addEmployeeToBonusDto.EmployeeIds, result.EmployeeIds);
            });

            WithUnitOfWork(() =>
            {
                bonusEmployees = _workScope.GetAll<BonusEmployee>().ToList();

                long countAft = bonusEmployees.Count;
                countAft.ShouldBeEquivalentTo(countBef + 2);
            });
        }

        [Fact]
        public async Task QuickAddEmployeeToBonus_ExistEmployeeId_UserFriendlyException()
        {
            // Check add one already exist Id 899
            var bonusEmployees = new List<BonusEmployee>();
            long countBef = 0;

            var addEmployeeToBonusDto = new AddEmployeeToBonusDto()
            {
                BonusId = 61,
                Money = 400000,
                Note = "Thuong lucky draw",
                EmployeeIds = new List<long> { 899, 898 }

            };

            await WithUnitOfWorkAsync(async () =>
            {
                bonusEmployees = _workScope.GetAll<BonusEmployee>().ToList();
                countBef = bonusEmployees.Count;

                
                var result = await _bonusManager.QuickAddEmployeeToBonus(addEmployeeToBonusDto);

                Assert.NotNull(result);
                Assert.Equal(addEmployeeToBonusDto.Money, result.Money);
                Assert.Equal(addEmployeeToBonusDto.Note, result.Note);
                Assert.Equal(addEmployeeToBonusDto.EmployeeIds, result.EmployeeIds);
            });

            await WithUnitOfWorkAsync(async () =>
            {
                var exception = await Assert.ThrowsAsync<UserFriendlyException>(async () =>
                {
                   //Check Exist
                   addEmployeeToBonusDto.EmployeeIds = new List<long> { 899 };
                   await _bonusManager.QuickAddEmployeeToBonus(addEmployeeToBonusDto);
                });

                var expectedErrorMsg = "This User Is Already Exist";
                Assert.Equal(expectedErrorMsg, exception.Message);
            });
        }

        [Fact]
        public async Task QuickAddEmployeeToBonus_AddOne_CheckNotePropertyNull()
        {
            var bonusEmployees = new List<BonusEmployee>();
            long countBef = 0;

            await WithUnitOfWorkAsync(async () =>
            {
                bonusEmployees = _workScope.GetAll<BonusEmployee>().ToList();
                countBef = bonusEmployees.Count;

                var addEmployeeToBonusDto = new AddEmployeeToBonusDto()
                {
                    BonusId = 61,
                    Money = 400,
                    EmployeeIds = new List<long> { 899 }

                };
                var result = await _bonusManager.QuickAddEmployeeToBonus(addEmployeeToBonusDto);

                Assert.NotNull(result);
                Assert.Equal(addEmployeeToBonusDto.Money, result.Money);
                Assert.Equal(addEmployeeToBonusDto.EmployeeIds, result.EmployeeIds);
            });

            WithUnitOfWork(() =>
            {
                bonusEmployees = _workScope.GetAll<BonusEmployee>().ToList();
                long countAft = bonusEmployees.Count;
                countAft.ShouldBeEquivalentTo(countBef + 1);

                var entityAdded = _workScope.GetAll<BonusEmployee>().ToList().LastOrDefault();
                Assert.Equal("1", entityAdded?.Note);
            });
        }

        [Fact]
        public async Task QuickAddEmployeeToBonus_ExistEmployeeId_ShouldUpdate()
        {
            // Check add list with already exist employeeIds
            var bonusEmployees = new List<BonusEmployee>();
            long countBef = 0;

            await WithUnitOfWorkAsync(async () =>
            {
                var addEmployeeToBonusDto1 = new AddEmployeeToBonusDto()
                {
                    BonusId = 61,
                    Money = 400000,
                    Note = "Thuong lucky draw",
                    EmployeeIds = new List<long> { 899, 898 }
                };

                bonusEmployees = _workScope.GetAll<BonusEmployee>().ToList();
                countBef = bonusEmployees.Count;


                var result = await _bonusManager.QuickAddEmployeeToBonus(addEmployeeToBonusDto1);

                Assert.NotNull(result);
                Assert.Equal(addEmployeeToBonusDto1.Money, result.Money);
                Assert.Equal(addEmployeeToBonusDto1.Note, result.Note);
                Assert.Equal(addEmployeeToBonusDto1.EmployeeIds, result.EmployeeIds);
            });


            var addEmployeeToBonusDto2 = new AddEmployeeToBonusDto()
            {
                BonusId = 61,
                Money = 500000,
                Note = "Thuong merry chrismast",
                EmployeeIds = new List<long> { 899, 898 }
            };
            await WithUnitOfWorkAsync(async () =>
            {
                var result = await _bonusManager.QuickAddEmployeeToBonus(addEmployeeToBonusDto2);
                Assert.NotNull(result);
            });

            WithUnitOfWork(() =>
            {
                bonusEmployees = _workScope.GetAll<BonusEmployee>().ToList();
                long countAft = bonusEmployees.Count;
                countAft.ShouldBeEquivalentTo(countBef + 2);

                //Check update new value
                var listUpdated = bonusEmployees.Skip(bonusEmployees.Count - 2);
                foreach(var item in listUpdated)
                {
                    item.Note.ShouldBeEquivalentTo(addEmployeeToBonusDto2.Note);
                    item.Money.ShouldBeEquivalentTo(addEmployeeToBonusDto2.Money);
                    item.BonusId.ShouldBeEquivalentTo(addEmployeeToBonusDto2.BonusId);
                }
            }); 
        }


        //AddBonusForEmployee
        [Fact]
        public async Task AddBonusForEmployee_ExistEmployeeInBonus_UserFriendlyException()
        {
            var addBonusForEmployeeDto = new AddBonusForEmployeeDto()
            {
                EmployeeId = 891,
                BonusId = 54,
                Money = 400,
                Note = "Add bonus for emp",
            };
            await WithUnitOfWorkAsync(async () =>
            {
                var exception = await Assert.ThrowsAsync<UserFriendlyException>(async () =>
                {
                    await _bonusManager.AddBonusForEmployee(addBonusForEmployeeDto);
                });

                var expectedErrorMsg = "Employee already has this bonus";
                Assert.Equal(expectedErrorMsg, exception.Message);

            });
        }

        [Fact]
        public async Task AddBonusForEmployee_WrongBonusId_NullReferenceException()
        {
            var addBonusForEmployeeDto = new AddBonusForEmployeeDto()
            {
                EmployeeId = 891,
                BonusId = 0,
                Money = 400,
                Note = "Add bonus for emp",
            };
            await WithUnitOfWorkAsync(async () =>
            {
                await Assert.ThrowsAsync<NullReferenceException>(async () =>
                {
                    await _bonusManager.AddBonusForEmployee(addBonusForEmployeeDto);
                });
            });
        }

        [Fact]
        public async Task AddBonusForEmployee_RightDto_Success()
        {
            var addBonusForEmployeeDto = new AddBonusForEmployeeDto()
            {
                EmployeeId = 891,
                BonusId = 61,
                Money = 400,
                Note = "Add bonus for emp",
            };

            var bonusEmployees = new List<BonusEmployee>();
            long countBef = 0;

            await WithUnitOfWorkAsync(async () =>
            {
                bonusEmployees = _workScope.GetAll<BonusEmployee>().ToList();
                countBef = bonusEmployees.Count;

                var result = await _bonusManager.AddBonusForEmployee(addBonusForEmployeeDto);
                Assert.NotNull(result);
            });

            WithUnitOfWork(() =>
            {
                bonusEmployees = _workScope.GetAll<BonusEmployee>().ToList();

                long countAft = bonusEmployees.Count;
                countAft.ShouldBeEquivalentTo(countBef + 1);
            });
        }


        //RemoveBonusFromEmployee
        [Fact]
        public async Task RemoveBonusFromEmployee_WrongId_NoDelete()
        {
            var bonusEmployees = new List<BonusEmployee>();
            long countBef = 0;

            await WithUnitOfWorkAsync(async () =>
            {
                bonusEmployees = _workScope.GetAll<BonusEmployee>().ToList();
                countBef = bonusEmployees.Count;

                long id = -1;
                var result = await _bonusManager.RemoveBonusFromEmployee(id);

                Assert.Equal(result, id);
            });

            WithUnitOfWork(() =>
            {
                bonusEmployees = _workScope.GetAll<BonusEmployee>().ToList();

                long countAft = bonusEmployees.Count;
                countAft.ShouldBeEquivalentTo(countBef);
            });
        }

        [Fact]
        public async Task RemoveBonusFromEmployee_Right_DeleteSuccess()
        {
            var bonusEmployees = new List<BonusEmployee>();
            long countBef = 0;

            await WithUnitOfWorkAsync(async () =>
            {
                bonusEmployees = _workScope.GetAll<BonusEmployee>().ToList();
                countBef = bonusEmployees.Count;

                long id = 966;
                var result = await _bonusManager.RemoveBonusFromEmployee(id);

                Assert.Equal(result, id);
            });

            WithUnitOfWork(() =>
            {
                bonusEmployees = _workScope.GetAll<BonusEmployee>().ToList();

                long countAft = bonusEmployees.Count;
                countAft.ShouldBeEquivalentTo(countBef - 1);
            });
        }


        //MultipleAddEmployeeToBonus
        [Fact]
        public async Task MultipleAddEmployeeToBonus_BonusIdNotActive_UserFriendlyException()
        {
            var addEmployeeToBonusDto = new AddEmployeeToBonusDto()
            {
                BonusId = 60,
                Money = 400,
                Note = "MultipleAddEmployeeToBonus",
                EmployeeIds = new List<long> { 899, 898 }

            };
            await WithUnitOfWorkAsync(async () =>
            {
                var exception = await Assert.ThrowsAsync<UserFriendlyException>(async () =>
                {
                    await _bonusManager.MultipleAddEmployeeToBonus(addEmployeeToBonusDto);
                });

                var expectedErrorMsg = "Bonus not active";
                Assert.Equal(expectedErrorMsg, exception.Message);

            });
        }

        [Fact]
        public async Task MultipleAddEmployeeToBonus_WrongBonusId_UserFriendlyException()
        {
            var addEmployeeToBonusDto = new AddEmployeeToBonusDto()
            {
                BonusId = -1,
                Money = 400,
                Note = "MultipleAddEmployeeToBonus",
                EmployeeIds = new List<long> { 899, 898 }

            };
            await WithUnitOfWorkAsync(async () =>
            {
                await Assert.ThrowsAsync<UserFriendlyException>(async () =>
                {
                    await _bonusManager.MultipleAddEmployeeToBonus(addEmployeeToBonusDto);
                });
            });
        }

        [Fact]
        public async Task MultipleAddEmployeeToBonus_NewListEmployees_Success()
        {
            var bonusEmployees = new List<BonusEmployee>();
            long countBef = 0;

            await WithUnitOfWorkAsync(async () =>
            {
                bonusEmployees = _workScope.GetAll<BonusEmployee>().ToList();
                countBef = bonusEmployees.Count;

                var addEmployeeToBonusDto = new AddEmployeeToBonusDto()
                {
                    BonusId = 61,
                    Money = 3000000,
                    Note = "MultipleAddEmployeeToBonus",
                    EmployeeIds = new List<long> { 899, 898 }

                };
                var result = await _bonusManager.MultipleAddEmployeeToBonus(addEmployeeToBonusDto);

                Assert.NotNull(result);
                Assert.Equal(addEmployeeToBonusDto.Money, result.Money);
                Assert.Equal(addEmployeeToBonusDto.Note, result.Note);
                Assert.Equal(addEmployeeToBonusDto.EmployeeIds, result.EmployeeIds);
            });

            WithUnitOfWork(() =>
            {
                bonusEmployees = _workScope.GetAll<BonusEmployee>().ToList();

                long countAft = bonusEmployees.Count;
                countAft.ShouldBeEquivalentTo(countBef + 2);
            });
        }

        [Fact]
        public async Task MultipleAddEmployeeToBonus_ExistEmployeeId_UserFriendlyException()
        {
            // Check add one already exist Id 899
            var bonusEmployees = new List<BonusEmployee>();
            long countBef = 0;

            var addEmployeeToBonusDto = new AddEmployeeToBonusDto()
            {
                BonusId = 61,
                Money = 400000,
                Note = "MultipleAddEmployeeToBonus",
                EmployeeIds = new List<long> { 899, 898 }

            };

            await WithUnitOfWorkAsync(async () =>
            {
                bonusEmployees = _workScope.GetAll<BonusEmployee>().ToList();
                countBef = bonusEmployees.Count;


                var result = await _bonusManager.MultipleAddEmployeeToBonus(addEmployeeToBonusDto);

                Assert.NotNull(result);
                Assert.Equal(addEmployeeToBonusDto.Money, result.Money);
                Assert.Equal(addEmployeeToBonusDto.Note, result.Note);
                Assert.Equal(addEmployeeToBonusDto.EmployeeIds, result.EmployeeIds);
            });

            await WithUnitOfWorkAsync(async () =>
            {
                var exception = await Assert.ThrowsAsync<UserFriendlyException>(async () =>
                {
                    //Check Exist
                    addEmployeeToBonusDto.EmployeeIds = new List<long> { 899 };
                    await _bonusManager.MultipleAddEmployeeToBonus(addEmployeeToBonusDto);
                });

                var expectedErrorMsg = "This User Is Already Exist";
                Assert.Equal(expectedErrorMsg, exception.Message);
            });
        }

        [Fact]
        public async Task MultipleAddEmployeeToBonus_AddOne_CheckNotePropertyNull()
        {
            var bonusEmployees = new List<BonusEmployee>();
            long countBef = 0;

            await WithUnitOfWorkAsync(async () =>
            {
                bonusEmployees = _workScope.GetAll<BonusEmployee>().ToList();
                countBef = bonusEmployees.Count;

                var addEmployeeToBonusDto = new AddEmployeeToBonusDto()
                {
                    BonusId = 61,
                    Money = 400,
                    EmployeeIds = new List<long> { 899 }

                };
                var result = await _bonusManager.MultipleAddEmployeeToBonus(addEmployeeToBonusDto);

                Assert.NotNull(result);
                Assert.Equal(addEmployeeToBonusDto.Money, result.Money);
                Assert.Equal(addEmployeeToBonusDto.EmployeeIds, result.EmployeeIds);
            });

            WithUnitOfWork(() =>
            {
                bonusEmployees = _workScope.GetAll<BonusEmployee>().ToList();
                long countAft = bonusEmployees.Count;
                countAft.ShouldBeEquivalentTo(countBef + 1);

                var entityAdded = _workScope.GetAll<BonusEmployee>().ToList().LastOrDefault();
                Assert.Equal("1", entityAdded?.Note);
            });
        }

        [Fact]
        public async Task MultipleAddEmployeeToBonus_ExistEmployeeId_ShouldUpdate()
        {
            // Check add list with already exist employeeIds
            var bonusEmployees = new List<BonusEmployee>();
            long countBef = 0;

            await WithUnitOfWorkAsync(async () =>
            {
                var addEmployeeToBonusDto1 = new AddEmployeeToBonusDto()
                {
                    BonusId = 61,
                    Money = 400000,
                    Note = "MultipleAddEmployeeToBonus",
                    EmployeeIds = new List<long> { 899, 898 }
                };

                bonusEmployees = _workScope.GetAll<BonusEmployee>().ToList();
                countBef = bonusEmployees.Count;


                var result = await _bonusManager.MultipleAddEmployeeToBonus(addEmployeeToBonusDto1);

                Assert.NotNull(result);
                Assert.Equal(addEmployeeToBonusDto1.Money, result.Money);
                Assert.Equal(addEmployeeToBonusDto1.Note, result.Note);
                Assert.Equal(addEmployeeToBonusDto1.EmployeeIds, result.EmployeeIds);
            });


            var addEmployeeToBonusDto2 = new AddEmployeeToBonusDto()
            {
                BonusId = 61,
                Money = 500000,
                Note = "Thuong merry chrismast",
                EmployeeIds = new List<long> { 899, 898 }
            };
            await WithUnitOfWorkAsync(async () =>
            {
                var result = await _bonusManager.MultipleAddEmployeeToBonus(addEmployeeToBonusDto2);
                Assert.NotNull(result);
            });

            WithUnitOfWork(() =>
            {
                bonusEmployees = _workScope.GetAll<BonusEmployee>().ToList();
                long countAft = bonusEmployees.Count;
                countAft.ShouldBeEquivalentTo(countBef + 2);

                //Check update new value
                var listUpdated = bonusEmployees.Skip(bonusEmployees.Count - 2);
                foreach (var item in listUpdated)
                {
                    item.Note.ShouldBeEquivalentTo(addEmployeeToBonusDto2.Note);
                    item.Money.ShouldBeEquivalentTo(addEmployeeToBonusDto2.Money);
                    item.BonusId.ShouldBeEquivalentTo(addEmployeeToBonusDto2.BonusId);
                }
            });
        }


        //UpdateEmployeeInBonus
        [Fact]
        public async Task UpdateEmployeeInBonus_BonusIdNotActive_UserFriendlyException()
        {
            var editEmployeeToBonusDto = new EditEmployeeToBonusDto()
            {
                Id = 0,
                BonusId = 60,
                Money = 400,
                Note = "UpdateEmployeeInBonus"
            };
            await WithUnitOfWorkAsync(async () =>
            {
                var exception = await Assert.ThrowsAsync<UserFriendlyException>(async () =>
                {
                    await _bonusManager.UpdateEmployeeInBonus(editEmployeeToBonusDto);
                });

                var expectedErrorMsg = "Bonus not active";
                Assert.Equal(expectedErrorMsg, exception.Message);
            });
        }

        [Fact]
        public async Task UpdateEmployeeInBonus_RightBonusId_WrongId_EntityNotFoundException()
        {
            var editEmployeeToBonusDto = new EditEmployeeToBonusDto()
            {
                Id = 0,
                BonusId = 61,
                Money = 400,
                Note = "UpdateEmployeeInBonus"
            };
            await WithUnitOfWorkAsync(async () =>
            {
               await Assert.ThrowsAsync<EntityNotFoundException>(async () =>
               {
                   await _bonusManager.UpdateEmployeeInBonus(editEmployeeToBonusDto);
               });
            });
        }

        [Fact]
        public async Task UpdateEmployeeInBonus_RightDto_EntityNotFoundException()
        {
            var editEmployeeToBonusDto = new EditEmployeeToBonusDto()
            {
                Id = 1031,
                BonusId = 61,
                Money = 123456,
                Note = "UpdateEmployeeInBonus",
            };

            //Because no data test => add first
            await WithUnitOfWorkAsync(async () =>
            {
                var addEmployeeToBonusDto = new AddEmployeeToBonusDto()
                {
                    BonusId = 61,
                    Money = 400,
                    EmployeeIds = new List<long> { 899 }

                };
                var result = await _bonusManager.QuickAddEmployeeToBonus(addEmployeeToBonusDto);
                Assert.NotNull(result);
          
            });

            await WithUnitOfWorkAsync(async () =>
            {
                var result = await _bonusManager.UpdateEmployeeInBonus(editEmployeeToBonusDto);
                Assert.NotNull(result);
            });

            WithUnitOfWork(() =>
            {
                var entity = _workScope.GetAsync<BonusEmployee>(editEmployeeToBonusDto.Id).Result;

                Assert.Equal(editEmployeeToBonusDto.Id, entity.Id);
                Assert.Equal(editEmployeeToBonusDto.BonusId, entity.BonusId);
                Assert.Equal(editEmployeeToBonusDto.Money, entity.Money);
                Assert.Equal(editEmployeeToBonusDto.Note, entity.Note);
            });
        }


        //DeleteEmployeeFromBonus
        [Fact]
        public async Task DeleteEmployeeFromBonus_BonusIdNotActive_UserFriendlyException()
        {
            long id = 0;
            long bonusId = 60;

            await WithUnitOfWorkAsync(async () =>
            {
                var exception = await Assert.ThrowsAsync<UserFriendlyException>(async () =>
                {
                    await _bonusManager.DeleteEmployeeFromBonus(id, bonusId);
                });

                var expectedErrorMsg = "Bonus not active";
                Assert.Equal(expectedErrorMsg, exception.Message);
            });
        }

        [Fact]
        public async Task DeleteEmployeeFromBonus_RightBonusId_WrongId_EntityNotFoundException()
        {
            long id = -1;
            long bonusId = 61;

            await WithUnitOfWorkAsync(async () =>
            {
               await Assert.ThrowsAsync<EntityNotFoundException>(async () =>
                {
                    await _bonusManager.DeleteEmployeeFromBonus(id, bonusId);
                });

            });
        }

        [Fact]
        public async Task DeleteEmployeeFromBonus_RightAllId_DeleteSuccess()
        {
            var bonusEmployees = new List<BonusEmployee>();
            long countBef = 0;


            //Because no data for bonusId 61 => add first
            await WithUnitOfWorkAsync(async () =>
            {
                var addEmployeeToBonusDto = new AddEmployeeToBonusDto()
                {
                    BonusId = 61,
                    Money = 400,
                    EmployeeIds = new List<long> { 899 }

                };
                var result = await _bonusManager.QuickAddEmployeeToBonus(addEmployeeToBonusDto);
                Assert.NotNull(result);

            });

            await WithUnitOfWorkAsync(async () =>
            {
                bonusEmployees = _workScope.GetAll<BonusEmployee>().ToList();
                countBef = bonusEmployees.Count;

                long id = 1031;
                long bonusId = 61;
                var result = await _bonusManager.DeleteEmployeeFromBonus(id, bonusId);

                Assert.Equal(result, id);
            });

            WithUnitOfWork(() =>
            {
                bonusEmployees = _workScope.GetAll<BonusEmployee>().ToList();

                long countAft = bonusEmployees.Count;
                countAft.ShouldBeEquivalentTo(countBef - 1);
            });
        }


        //GetAllEmployeeInBonus
        [Fact]
        public void GetAllEmployeeInBonus_WrongBonusId_ReturnEmptyList()
        {

            WithUnitOfWork(() =>
            {
                int bonusId = 0;
                var AR = _bonusManager.GetAllEmployeeInBonus(bonusId);
                AR.Result.Count.ShouldBeEquivalentTo(0);
            });
        }

        [Fact]
        public void GetAllEmployeeInBonus_RightBonusId_CheckListReturn()
        {
            var listExpected = new List<long> { 897, 888, 887, 886, 889  };
            WithUnitOfWork(() =>
            {
                int bonusId = 57;
                var AR = _bonusManager.GetAllEmployeeInBonus(bonusId);
                AR.Result.Count.ShouldBeEquivalentTo(listExpected.Count);

                for (int i = 0; i < AR.Result.Count; i++)
                {
                    var idAR = AR.Result.Skip(i).Take(1).FirstOrDefault();
                    var idER = listExpected.Skip(i).Take(1).FirstOrDefault();

                    idAR.ShouldBeEquivalentTo(idER);
                }
            });
        }


        //GetListMonthFilterOfEmployee
        [Fact]
        public void GetListMonthFilterOfEmployee_WrongEmployeeId_ReturnEmptyList()
        {

            WithUnitOfWork(() =>
            {
                int employeeId = 0;
                var AR = _bonusManager.GetListMonthFilterOfEmployee(employeeId);
                AR.Result.Count.ShouldBeEquivalentTo(0);
            });
        }

        [Fact]
        public void GetListMonthFilterOfEmployee_RightEmployeeId_CheckListReturn()
        {
            var listExpected = new List<DateTime> { 
                DateTime.Parse("2023-02-15 00:00:00"), 
                DateTime.Parse("2023-01-15 00:00:00"), 
                DateTime.Parse("2022-12-15 00:00:00"),
            };

            WithUnitOfWork(() =>
            {
                int employeeId = 887;
                var AR = _bonusManager.GetListMonthFilterOfEmployee(employeeId);
                AR.Result.Count.ShouldBeEquivalentTo(listExpected.Count);

                for (int i = 0; i < AR.Result.Count; i++)
                {
                    var dateAR = AR.Result.Skip(i).Take(1).FirstOrDefault();
                    var dateER = listExpected.Skip(i).Take(1).FirstOrDefault();

                    Assert.Equal(dateER.ToString(), dateAR.ToString());
                }
            });
        }


        //GetBonusById
        [Fact]
        public void GetBonusById_WrongId_ShouldNull()
        {
            WithUnitOfWork(() =>
            {
                int bonusId = 0;
                var AR = _bonusManager.GetBonusById(bonusId);
                Assert.Null(AR);
            });
        }

        [Fact]
        public void GetBonusById_RightId_CheckReturn()
        {
            var dataExpected = new GetBonusDto()
            {
                Id = 60,
                Name = "Bonus thưởng dự án 02/2023",
                ApplyMonth = DateTime.Parse("2023-02-15 00:00:00"),
                IsActive = false
            };

            WithUnitOfWork(() =>
            {
                int bonusId = 60;
                var result = _bonusManager.GetBonusById(bonusId);
                Assert.NotNull(result);

                result.Id.ShouldBeEquivalentTo(dataExpected.Id);
                result.Name.ShouldBeEquivalentTo(dataExpected.Name);
                result.IsActive.ShouldBeEquivalentTo(dataExpected.IsActive);
              
                Assert.Equal(result.ApplyMonth.ToString(), dataExpected.ApplyMonth.ToString());
            });
        }


        //ImportEmployeeToBonus
        [Fact]
        public void ImportEmployeeToBonus_StandardCase_Success()
        {
            // Standard test case
            //var file = ".\\Managers\\Bonuses\\ImportFiles\\import-employee-to-bonus_1.xlsx";
       
            //var stream = new MemoryStream(File.ReadAllBytes(file).ToArray());
            //var formFile = new FormFile(stream, 0, stream.Length, "streamFile", file.Split(@"\").Last());

            var root = Directory.GetCurrentDirectory().Replace("\\bin\\Debug\\net6.0", "").Replace("/bin/Debug/net6.0", "");
            string path = Path.Combine(root, "Managers", "Bonuses", "ImportFiles", "import-employee-to-bonus_1.xlsx");

            var stream = new MemoryStream(File.ReadAllBytes(path).ToArray());
            var formFile = new FormFile(stream, 0, stream.Length, "streamFile", "import-employee-to-bonus_1.xlsx");


            var input = new ImportFileDto
            {
                File = formFile,
                BonusId = 61
            };

            WithUnitOfWork(() =>
            {
                var result =  _bonusManager.ImportEmployeeToBonus(input).Result;

                var resultProperties = result.GetType().GetProperties();
                var successList = resultProperties.First(o => o.Name == "successList").GetValue(result, null) as ICollection<BonusEmployee>;
                var failedList = resultProperties.First(o => o.Name == "failedList").GetValue(result, null) as ICollection<ResponseFailDto>;

                Assert.Equal(2, successList?.Count);
                Assert.Equal(0, failedList?.Count);
            });
        }

        [Fact]
        public async Task ImportEmployeeToBonus_WrongBonusId_NullReferenceException()
        {
            // Standard test case
            //var file = ".\\Managers\\Bonuses\\ImportFiles\\import-employee-to-bonus_1.xlsx";

            //var stream = new MemoryStream(File.ReadAllBytes(file).ToArray());
            //var formFile = new FormFile(stream, 0, stream.Length, "streamFile", file.Split(@"\").Last());
            var root = Directory.GetCurrentDirectory().Replace("\\bin\\Debug\\net6.0", "").Replace("/bin/Debug/net6.0", "");
            string path = Path.Combine(root, "Managers", "Bonuses", "ImportFiles", "import-employee-to-bonus_1.xlsx");

            var stream = new MemoryStream(File.ReadAllBytes(path).ToArray());
            var formFile = new FormFile(stream, 0, stream.Length, "streamFile", "import-employee-to-bonus_1.xlsx");
            var input = new ImportFileDto
            {
                File = formFile,
                BonusId = 0
            };

            await WithUnitOfWorkAsync(async () =>
            {
                var exception = await Assert.ThrowsAsync<NullReferenceException>(async () =>
                {
                    await _bonusManager.ImportEmployeeToBonus(input);
                });
               
            });
        }

        [Fact]
        public async Task ImportEmployeeToBonus_WrongFormat_UserFriendlyException()
        {
            // Standard test case
            //var file = ".\\Managers\\Bonuses\\ImportFiles\\import-employee-to-bonus_2.pdf";

            //var stream = new MemoryStream(File.ReadAllBytes(file).ToArray());
            //var formFile = new FormFile(stream, 0, stream.Length, "streamFile", file.Split(@"\").Last());


            var root = Directory.GetCurrentDirectory().Replace("\\bin\\Debug\\net6.0", "").Replace("/bin/Debug/net6.0", "");
            string path = Path.Combine(root, "Managers", "Bonuses", "ImportFiles", "import-employee-to-bonus_2.pdf");

            var stream = new MemoryStream(File.ReadAllBytes(path).ToArray());
            var formFile = new FormFile(stream, 0, stream.Length, "streamFile", "import-employee-to-bonus_2.pdf");


            var input = new ImportFileDto
            {
                File = formFile,
                BonusId = 61
            };

            await WithUnitOfWorkAsync(async () =>
            {
                var exception=  await Assert.ThrowsAsync<UserFriendlyException>(async () =>
                {
                    await _bonusManager.ImportEmployeeToBonus(input);
                });

                var expectedMessage = "File upload is invalid";
                Assert.Equal(expectedMessage, exception.Message);
            });
        }

        [Fact]
        public async Task ImportEmployeeToBonus_MissingColumnField_UserFriendlyException()
        {
            // Standard test case
            //var file = ".\\Managers\\Bonuses\\ImportFiles\\import-employee-to-bonus_3.xlsx";

            //var stream = new MemoryStream(File.ReadAllBytes(file).ToArray());
            //var formFile = new FormFile(stream, 0, stream.Length, "streamFile", file.Split(@"\").Last());

            var root = Directory.GetCurrentDirectory().Replace("\\bin\\Debug\\net6.0", "").Replace("/bin/Debug/net6.0", "");
            string path = Path.Combine(root, "Managers", "Bonuses", "ImportFiles", "import-employee-to-bonus_3.xlsx");

            var stream = new MemoryStream(File.ReadAllBytes(path).ToArray());
            var formFile = new FormFile(stream, 0, stream.Length, "streamFile", "import-employee-to-bonus_3.xlsx");

            var input = new ImportFileDto
            {
                File = formFile,
                BonusId = 61
            };

            await WithUnitOfWorkAsync(async () =>
            {
                var exception = await Assert.ThrowsAsync<UserFriendlyException>(async () =>
                {
                    await _bonusManager.ImportEmployeeToBonus(input);
                });

                var expectedMessage = "Number of columns < 2 => Invlid format";
                Assert.Equal(expectedMessage, exception.Message);
            });
        }

        [Fact]
        public async Task ImportEmployeeToBonus_RowTwoEmailInvalid_CheckListReturn()
        {
            // Standard test case
            //var file = ".\\Managers\\Bonuses\\ImportFiles\\import-employee-to-bonus_4.xlsx";

            //var stream = new MemoryStream(File.ReadAllBytes(file).ToArray());
            //var formFile = new FormFile(stream, 0, stream.Length, "streamFile", file.Split(@"\").Last());


            var root = Directory.GetCurrentDirectory().Replace("\\bin\\Debug\\net6.0", "").Replace("/bin/Debug/net6.0", "");
            string path = Path.Combine(root, "Managers", "Bonuses", "ImportFiles", "import-employee-to-bonus_4.xlsx");

            var stream = new MemoryStream(File.ReadAllBytes(path).ToArray());
            var formFile = new FormFile(stream, 0, stream.Length, "streamFile", "import-employee-to-bonus_4.xlsx");

            var input = new ImportFileDto
            {
                File = formFile,
                BonusId = 61
            };

            var successListExpect = new List<BonusEmployee>
            {
                new BonusEmployee
                {
                    BonusId = 61,
                    EmployeeId = 889,
                    Money = 2000,
                    Note = "Bonus thưởng dự án 5/2023",
                }
            };

            var failedListExpect = new List<ResponseFailDto>
            {
                new ResponseFailDto
                {
                    Row = 2,
                    Email = "emailinvalid@ncc.japan",
                    Money = "1000",
                    ReasonFail = "Email not found"
                }
            };

            await WithUnitOfWorkAsync(async () =>
            {
                var result = _bonusManager.ImportEmployeeToBonus(input).Result;

                var resultProperties = result.GetType().GetProperties();
                var successList = resultProperties.First(o => o.Name == "successList").GetValue(result, null) as ICollection<BonusEmployee>;
                var failedList = resultProperties.First(o => o.Name == "failedList").GetValue(result, null) as ICollection<ResponseFailDto>;

                Assert.Equal(successListExpect.Count, successList?.Count);
                Assert.Equal(failedListExpect.Count, failedList?.Count);

                //Check success obj
                var successEntity = successList?.FirstOrDefault();
                successEntity?.BonusId.ShouldBeEquivalentTo(successListExpect[0].BonusId);
                successEntity?.EmployeeId.ShouldBeEquivalentTo(successListExpect[0].EmployeeId);
                successEntity?.Money.ShouldBeEquivalentTo(successListExpect[0].Money);
                successEntity?.Note.ShouldBeEquivalentTo(successListExpect[0].Note);


                //Check failed obj
                var failedEntity = failedList?.FirstOrDefault();
                failedEntity?.Row.ShouldBeEquivalentTo(failedListExpect[0].Row);
                failedEntity?.Email.ShouldBeEquivalentTo(failedListExpect[0].Email);
                failedEntity?.Money.ShouldBeEquivalentTo(failedListExpect[0].Money);
                failedEntity?.ReasonFail.ShouldBeEquivalentTo(failedListExpect[0].ReasonFail);
            });

        }

        [Fact]
        public async Task ImportEmployeeToBonus_RowTwoEmailNull_CheckListReturn()
        {
            // Standard test case
            //var file = ".\\Managers\\Bonuses\\ImportFiles\\import-employee-to-bonus_5.xlsx";

            //var stream = new MemoryStream(File.ReadAllBytes(file).ToArray());
            //var formFile = new FormFile(stream, 0, stream.Length, "streamFile", file.Split(@"\").Last());

            var root = Directory.GetCurrentDirectory().Replace("\\bin\\Debug\\net6.0", "").Replace("/bin/Debug/net6.0", "");
            string path = Path.Combine(root, "Managers", "Bonuses", "ImportFiles", "import-employee-to-bonus_5.xlsx");

            var stream = new MemoryStream(File.ReadAllBytes(path).ToArray());
            var formFile = new FormFile(stream, 0, stream.Length, "streamFile", "import-employee-to-bonus_5.xlsx");


            var input = new ImportFileDto
            {
                File = formFile,
                BonusId = 61
            };

            var successListExpect = new List<BonusEmployee>
            {
                new BonusEmployee
                {
                    BonusId = 61,
                    EmployeeId = 889,
                    Money = 2000,
                    Note = "Bonus thưởng dự án 5/2023",
                }
            };

            var failedListExpect = new List<ResponseFailDto>
            {
                new ResponseFailDto
                {
                    Row = 2,
                    Email = null,
                    Money = "1000",
                    ReasonFail = "Email null or empty"
                }
            };

            await WithUnitOfWorkAsync(async () =>
            {
                var result = _bonusManager.ImportEmployeeToBonus(input).Result;

                var resultProperties = result.GetType().GetProperties();
                var successList = resultProperties.First(o => o.Name == "successList").GetValue(result, null) as ICollection<BonusEmployee>;
                var failedList = resultProperties.First(o => o.Name == "failedList").GetValue(result, null) as ICollection<ResponseFailDto>;

                Assert.Equal(successListExpect.Count, successList?.Count);
                Assert.Equal(failedListExpect.Count, failedList?.Count);

                //Check success obj
                var successEntity = successList?.FirstOrDefault();
                successEntity?.BonusId.ShouldBeEquivalentTo(successListExpect[0].BonusId);
                successEntity?.EmployeeId.ShouldBeEquivalentTo(successListExpect[0].EmployeeId);
                successEntity?.Money.ShouldBeEquivalentTo(successListExpect[0].Money);
                successEntity?.Note.ShouldBeEquivalentTo(successListExpect[0].Note);


                //Check failed obj
                var failedEntity = failedList?.FirstOrDefault();
                failedEntity?.Row.ShouldBeEquivalentTo(failedListExpect[0].Row);
                failedEntity?.Email.ShouldBeEquivalentTo(failedListExpect[0].Email);
                failedEntity?.Money.ShouldBeEquivalentTo(failedListExpect[0].Money);
                failedEntity?.ReasonFail.ShouldBeEquivalentTo(failedListExpect[0].ReasonFail);
            });

        }

        [Fact]
        public async Task ImportEmployeeToBonus_RowTwoEmailAlreadyImported_CheckListReturn()
        {
            // Standard test case
            //var file = ".\\Managers\\Bonuses\\ImportFiles\\import-employee-to-bonus_6.xlsx";

            //var stream = new MemoryStream(File.ReadAllBytes(file).ToArray());
            //var formFile = new FormFile(stream, 0, stream.Length, "streamFile", file.Split(@"\").Last());
            var root = Directory.GetCurrentDirectory().Replace("\\bin\\Debug\\net6.0", "").Replace("/bin/Debug/net6.0", "");
            string path = Path.Combine(root, "Managers", "Bonuses", "ImportFiles", "import-employee-to-bonus_6.xlsx");

            var stream = new MemoryStream(File.ReadAllBytes(path).ToArray());
            var formFile = new FormFile(stream, 0, stream.Length, "streamFile", "import-employee-to-bonus_6.xlsx");
            var input = new ImportFileDto
            {
                File = formFile,
                BonusId = 54
            };

            var failedListExpect = new List<ResponseFailDto>
            {
                new ResponseFailDto
                {
                    Row = 2,
                    Email = "anh.tranthaivan@ncc.asia",
                    Money = "1000",
                    ReasonFail = "Already imported"
                }
            };

            await WithUnitOfWorkAsync(async () =>
            {
                var result = _bonusManager.ImportEmployeeToBonus(input).Result;

                var resultProperties = result.GetType().GetProperties();
                var successList = resultProperties.First(o => o.Name == "successList").GetValue(result, null) as ICollection<BonusEmployee>;
                var failedList = resultProperties.First(o => o.Name == "failedList").GetValue(result, null) as ICollection<ResponseFailDto>;

                Assert.Equal(0, successList?.Count);
                Assert.Equal(failedListExpect.Count, failedList?.Count);

                //Check failed obj
                var failedEntity = failedList?.FirstOrDefault();
                failedEntity?.Row.ShouldBeEquivalentTo(failedListExpect[0].Row);
                failedEntity?.Email.ShouldBeEquivalentTo(failedListExpect[0].Email);
                failedEntity?.Money.ShouldBeEquivalentTo(failedListExpect[0].Money);
                failedEntity?.ReasonFail.ShouldBeEquivalentTo(failedListExpect[0].ReasonFail);
            });
        }


        //GetBonusTemplate
        [Fact]
        public void GetBonusTemplate_WrongBonusEmployeeId_NullReferenceException()
        {

            WithUnitOfWork(() =>
            {
                int bonusEmployeeId = 0;
                Assert.Throws<NullReferenceException>(() =>
                {
                     _bonusManager.GetBonusTemplate(bonusEmployeeId);
                });
            });
        }

        [Fact]
        public void GetBonusTemplate_RightBonusEmployeeId_Success()
        {
            WithUnitOfWork(() =>
            {
                int bonusEmployeeId = 1030;
                var result = _bonusManager.GetBonusTemplate(bonusEmployeeId);

                Assert.NotNull(result);
                Assert.NotNull(result.SendToEmail);
                Assert.NotNull(result.BodyMessage);
                Assert.NotNull(result.Subject);
            });
        }


        //SendMailToOneEmployee
        [Fact]
        public void SendMailToOneEmployee_WrongBonusEmployeeId_UserFriendlyException()
        {

            SendMailBonusDto dto = new SendMailBonusDto()
            {
                BonusEmployeeId = 0,
            };

            WithUnitOfWork(() =>
            {
                var exception = Assert.Throws<UserFriendlyException>(() =>
                {
                    _bonusManager.SendMailToOneEmployee(dto);
                });

                var expectErorMessage = "Can not found bonus employee with Id = 0";
                Assert.Equal(expectErorMessage, exception.Message);
            });
        }


        //SendMailToAllEmployee
        [Fact]
        public void SendMailToAllEmployee_WrongBonusId_CheckResult()
        {
            var gridParams = new GridParam();
            GetBonusEmployeeInputDto dto = new GetBonusEmployeeInputDto()
            {
                GridParam = gridParams
            };

            long bonusId = 0;

            WithUnitOfWork(() =>
            {
                var result =  _bonusManager.SendMailToAllEmployee(bonusId, dto);
                var expectErorMessage = "Started sending 0 email.";
                Assert.Equal(expectErorMessage, result);
            });
        }

        [Fact]
        public void SendMailToAllEmployee_RightBonusId_CheckResult()
        {
            var gridParams = new GridParam();
            GetBonusEmployeeInputDto dto = new GetBonusEmployeeInputDto()
            {
                GridParam = gridParams
            };

            long bonusId = 54;

            WithUnitOfWork(() =>
            {
                var result = _bonusManager.SendMailToAllEmployee(bonusId, dto);
                var expectErorMessage = "Started sending 5 email.";
                Assert.Equal(expectErorMessage, result);
            });
        }

        [Fact]
        public void SendMailToAllEmployee_RightBonusId_CheckSkip()
        {
            var gridParams = new GridParam()
            {
                SkipCount = 3,
            };
            GetBonusEmployeeInputDto dto = new GetBonusEmployeeInputDto()
            {
                GridParam = gridParams
            };

            long bonusId = 54;

            WithUnitOfWork(() =>
            {
                var result = _bonusManager.SendMailToAllEmployee(bonusId, dto);
                var expectErorMessage = "Started sending 2 email.";
                Assert.Equal(expectErorMessage, result);
            });
        }

        [Fact]
        public void SendMailToAllEmployee_RightBonusId_CheckResultWithBranch()
        {
            var gridParams = new GridParam();
               
            GetBonusEmployeeInputDto dto = new GetBonusEmployeeInputDto()
            {
                GridParam = gridParams,
                BranchIds = new List<long> {100, 94 }
            };

            long bonusId = 54;

            WithUnitOfWork(() =>
            {
                var result = _bonusManager.SendMailToAllEmployee(bonusId, dto);
                var expectErorMessage = "Started sending 3 email.";
                Assert.Equal(expectErorMessage, result);
            });
        }


        private void helpCheckRightItemForGetAllBonusEmployee(GridResult<GetBonusEmployeeDto> listActual, List<GetBonusEmployeeDto> listExpected)
        {
            for (int i = 0; i < listActual.TotalCount; i++)
            {
                var objAR = listActual.Items.Skip(i).Take(1).FirstOrDefault();
                var objER = listExpected.Skip(i).Take(1).FirstOrDefault();

                objAR?.BonusId.ShouldBeEquivalentTo(objER?.BonusId);
                objAR?.EmployeeId.ShouldBeEquivalentTo(objER?.EmployeeId);
                objAR?.Money.ShouldBeEquivalentTo(objER?.Money);
                objAR?.Note.ShouldBeEquivalentTo(objER?.Note);
                objAR?.BonusName.ShouldBeEquivalentTo(objER?.BonusName);
                objAR?.Note.ShouldBeEquivalentTo(objER?.Note);
                objAR?.FullNameModification.ShouldBeEquivalentTo(objER?.FullNameModification);
                Assert.Equal(objER?.ApplyDate.ToString(), objAR?.ApplyDate.ToString());
                Assert.Equal(objER?.LastModificationTime.ToString(), objAR?.LastModificationTime.ToString());
            }
        }

    }

}
