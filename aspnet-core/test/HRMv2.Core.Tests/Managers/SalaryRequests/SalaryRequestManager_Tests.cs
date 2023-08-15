using Abp.Application.Editions;
using Abp.Application.Features;
using Abp.BackgroundJobs;
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
using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Wordprocessing;
using HRMv2.Editions;
using HRMv2.Entities;
using HRMv2.Manager.Categories.JobPositions;
using HRMv2.Manager.Categories.Levels;
using HRMv2.Manager.Common.Dto;
using HRMv2.Manager.EmployeeContracts;
using HRMv2.Manager.Notifications.Email;
using HRMv2.Manager.Notifications.Email.Dto;
using HRMv2.Manager.Salaries.Dto;
using HRMv2.Manager.SalaryRequestEmployees.Dto;
using HRMv2.Manager.SalaryRequests;
using HRMv2.Manager.SalaryRequests.Dto;
using HRMv2.MultiTenancy;
using HRMv2.NccCore;
using HRMv2.UploadFileServices;
using HRMv2.Utils;
using HRMv2.WebServices.Timesheet;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using NccCore.Paging;
using NSubstitute;
using Shouldly;
using Xunit;
using static HRMv2.Constants.Enum.HRMEnum;

namespace HRMv2.Core.Tests.Managers.SalaryRequests
{
    public class SalaryRequestManager_Tests : HRMv2CoreTestBase
    {
        private readonly SalaryRequestManager _salaryRequestManager;
        private readonly IWorkScope _workScope;

        public DateTime ApplyMonth { get; private set; }

        public SalaryRequestManager_Tests()
        {
            _workScope = Resolve<IWorkScope>();

            var _httpClient = Resolve<HttpClient>();
            var _iAbpSession = Resolve<IAbpSession>();
            var _iocResovler = Resolve<IIocResolver>();

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

            var _emailManager = new EmailManager(_workScope, _emailSender, _timesheetConfig);

            // 2,2. ContractManager
            var _contractManager = Substitute.For<ContractManager>(_workScope, _uploadFileService, _emailManager);
            _contractManager.ObjectMapper = Resolve<IObjectMapper>();

            /* == 3. BACKGROUND JOB MANAGER == */
            var _jobPositionManager = Substitute.For<JobPositionManager>(_workScope);

            var _IBackgroundJobStore = Resolve<IBackgroundJobStore>();
            var _AbpAsyncTimer = Resolve<AbpAsyncTimer>();
            var _backgroundJobManager = Substitute.For<BackgroundJobManager>(_iocResovler, _IBackgroundJobStore, _AbpAsyncTimer);

            /* == 4. LEVEL MANAGER == */
            var _levelManager = Substitute.For<LevelManager>(_workScope);

            /* == 5. SALARY REQUEST MANAGER == */
            _salaryRequestManager = new SalaryRequestManager(_levelManager, _jobPositionManager, _contractManager, _workScope, _emailManager, _backgroundJobManager);
            _salaryRequestManager.ObjectMapper = Resolve<IObjectMapper>();
            _salaryRequestManager.UnitOfWorkManager = Resolve<IUnitOfWorkManager>();
        }

        [Fact]
        public void GetAll_Test1()
        {
            WithUnitOfWork(() =>
            {
                var result = _salaryRequestManager.GetAll();

                var allSalaryRequests = _workScope.GetAll<SalaryChangeRequest>().ToList();

                result.Count.ShouldBe(allSalaryRequests.Count);
                result.ShouldContain(salaryRequest => salaryRequest.Id == 143);
                result.ShouldContain(salaryRequest => salaryRequest.Name == "checkpoint");
                result.ShouldContain(salaryRequest => salaryRequest.ApplyMonth == new DateTime(2022, 12, 01));
                result.ShouldContain(salaryRequest => salaryRequest.Status == SalaryRequestStatus.Executed);
            });
        }

        [Fact]
        public async Task GetAllPaging_Test1()
        {
            var filter = new GridParam
            {
                SearchText = "checkpoint",
                MaxResultCount = 10
            };
            var expectedTotalCount = 2;
            var expectedItemsCount = 2;

            await WithUnitOfWorkAsync(async () =>
            {
                var result = await _salaryRequestManager.GetAllPaging(filter);

                result.TotalCount.ShouldBe(expectedTotalCount);
                result.Items.Count.ShouldBe(expectedItemsCount);
                result.Items.ShouldContain(request => request.Id == 143);
                result.Items.ShouldContain(request => request.Id == 153);
            });
        }

        [Fact]
        public async Task GetAllPaging_Test2()
        {
            var filter = new GridParam
            {
                SkipCount = 2,
                MaxResultCount = 10
            };
            var expectedTotalCount = 6;
            var expectedItemsCount = 4;

            await WithUnitOfWorkAsync(async () =>
            {
                var result = await _salaryRequestManager.GetAllPaging(filter);

                result.TotalCount.ShouldBe(expectedTotalCount);
                result.Items.Count.ShouldBe(expectedItemsCount);
                result.Items.ShouldContain(request => request.Id == 143);
                result.Items.ShouldContain(request => request.Id == 144);
                result.Items.ShouldContain(request => request.Id == 146);
                result.Items.ShouldContain(request => request.Id == 152);
            });
        }

        [Fact]
        public void Get_Test1()
        {
            // Standard test case
            var expectedId = 143;

            WithUnitOfWork(async () =>
            {
                var result = _salaryRequestManager.Get(expectedId);

                var expectedSalaryRequest = await _workScope.GetAsync<SalaryChangeRequest>(expectedId);

                result.Id.ShouldBe(expectedId);
                result.Name.ShouldBe(expectedSalaryRequest.Name);
                result.ApplyMonth.ShouldBe(expectedSalaryRequest.ApplyMonth);
                result.Status.ShouldBe(expectedSalaryRequest.Status);
                result.CreatorUser.ShouldBe(expectedSalaryRequest.CreatorUser.FullName);
                result.CreationTime.Equals(expectedSalaryRequest.CreationTime);
                result.LastModifyUser.ShouldBe(expectedSalaryRequest.LastModifierUser.FullName);
                result.LastModifyTime.Equals(expectedSalaryRequest.LastModificationTime);
            });
        }

        [Fact]
        public void Get_Test2()
        {
            // Non-existent SalaryRequest
            var expectedId = 1;

            WithUnitOfWork(() =>
            {
                var result = _salaryRequestManager.Get(expectedId);

                Assert.Null(result);
            });
        }

        [Fact]
        public async void Create_Test1()
        {
            // Standard test case
            var expectedSalaryRequest = new CreateSalaryRequestDto
            {
                Name = "Lên PM",
                ApplyMonth = new DateTime(2023, 01, 01)
            };
            var expectedId = 155;
            var allSalaryRequestsBeforeCreate = new List<SalaryChangeRequest>();

            WithUnitOfWork(() =>
            {
                allSalaryRequestsBeforeCreate = _workScope.GetAll<SalaryChangeRequest>().ToList();

                var result = _salaryRequestManager.Create(expectedSalaryRequest);

                result.Name.ShouldBe(expectedSalaryRequest.Name);
                result.ApplyMonth.ShouldBe(expectedSalaryRequest.ApplyMonth);
            });

            await WithUnitOfWorkAsync(async () =>
            {
                var allSalaryRequestsAfterCreate = _workScope.GetAll<SalaryChangeRequest>();
                var salaryRequest = await _workScope.GetAsync<SalaryChangeRequest>(expectedId);

                allSalaryRequestsAfterCreate.Count().ShouldBeGreaterThan(allSalaryRequestsBeforeCreate.Count);
                salaryRequest.Name.ShouldBe(expectedSalaryRequest.Name);
                salaryRequest.ApplyMonth.ShouldBe(expectedSalaryRequest.ApplyMonth);
            });
        }

        [Fact]
        public void Create_Test2()
        {
            // Existed name
            var expectedSalaryRequest = new CreateSalaryRequestDto
            {
                Name = "checkpoint",
                ApplyMonth = new DateTime(2023, 06, 01)
            };
            var expectedMessage = $"Request name is Already Exist";

            WithUnitOfWork(() =>
            {
                var exception = Assert.Throws<UserFriendlyException>(() =>
                {
                    _salaryRequestManager.Create(expectedSalaryRequest);
                });

                exception.Message.ShouldBe(expectedMessage);
            });
        }

        [Fact]
        public async Task Update_Test1()
        {
            // Standard test case
            var expectedSalaryRequest = new UpdateSalaryRequestDto
            {
                Id = 143,
                Name = "Lên PM tháng 01/2023",
                ApplyMonth = new DateTime(2023, 01, 01),
                Status = SalaryRequestStatus.Pending
            };

            await WithUnitOfWorkAsync(async() =>
            {
                var result = await _salaryRequestManager.Update(expectedSalaryRequest);

                result.Id.ShouldBe(expectedSalaryRequest.Id);
                result.Name.ShouldBe(expectedSalaryRequest.Name);
                result.ApplyMonth.ShouldBe(expectedSalaryRequest.ApplyMonth);
                result.Status.ShouldBe(expectedSalaryRequest.Status);
            });

            await WithUnitOfWorkAsync(async () =>
            {
                var salaryRequest = await _workScope.GetAsync<SalaryChangeRequest>(expectedSalaryRequest.Id);

                salaryRequest.Name.ShouldBe(expectedSalaryRequest.Name);
                salaryRequest.ApplyMonth.ShouldBe(expectedSalaryRequest.ApplyMonth);
                salaryRequest.Status.ShouldBe(expectedSalaryRequest.Status);
            });
        }

        [Fact]
        public async Task Update_Test2()
        {
            // Existed name
            var expectedSalaryRequest = new UpdateSalaryRequestDto
            {
                Id = 144,
                Name = "checkpoint",
                ApplyMonth = new DateTime(2023, 06, 01),
                Status = SalaryRequestStatus.Approved
            };
            var expectedMessage = $"Request name is Already Exist";

            await WithUnitOfWorkAsync(async() =>
            {
                var exception = await Assert.ThrowsAsync<UserFriendlyException>(async() =>
                {
                    await _salaryRequestManager.Update(expectedSalaryRequest);
                });

                exception.Message.ShouldBe(expectedMessage);
            });
        }

        [Fact]
        public async void Delete_Test1()
        {
            // Standard test case
            var requestId = 144;

            var allSalaryRequestsBeforeDelete = new List<SalaryChangeRequest>();

            WithUnitOfWork(() =>
            {
                allSalaryRequestsBeforeDelete = _workScope.GetAll<SalaryChangeRequest>().ToList();

                var result = _salaryRequestManager.Delete(requestId);
                result.ShouldBe(requestId);
            });

            await WithUnitOfWorkAsync(async () =>
            {
                var allSalaryRequestsAfterDelete = _workScope.GetAll<SalaryChangeRequest>();
                allSalaryRequestsAfterDelete.Count().ShouldBeLessThan(allSalaryRequestsBeforeDelete.Count);

                await Assert.ThrowsAsync<EntityNotFoundException>(async () =>
                {
                    await _workScope.GetAsync<SalaryChangeRequest>(requestId);
                });
            });
        }

        [Fact]
        public void Delete_Test2()
        {
            // Non-existent request
            var requestId = 1;
            var expectedMessage = $"Can't find request with id {requestId}";

            WithUnitOfWork(() =>
            {
                var exception = Assert.Throws<UserFriendlyException>(() =>
                {
                    _salaryRequestManager.Delete(requestId);
                });

                exception.Message.ShouldBe(expectedMessage);
            });
        }

        [Fact]
        public void GetListDateFromSalaryRequest_Test1()
        {
            var expectedCount = 3;

            WithUnitOfWork(() =>
            {
                var result = _salaryRequestManager.GetListDateFromSalaryRequest();

                result.Count.ShouldBe(expectedCount);
                result.ShouldContain(new DateTime(2022, 12, 1));
                result.ShouldContain(new DateTime(2023, 1, 1));
                result.ShouldContain(new DateTime(2023, 2, 1));
            });
        }

        [Fact]
        public void GetEmployeeNotInRequest_Test1()
        {
            var requestId = 146;
            var expectedCount = 17;

            WithUnitOfWork(() =>
            {
                var result = _salaryRequestManager.GetEmployeeNotInRequest(requestId);

                result.Count.ShouldBe(expectedCount);
                result.ShouldNotContain(x => x.EmployeeId == 902);
                result.ShouldNotContain(x => x.EmployeeId == 903);
                result.ShouldNotContain(x => x.EmployeeId == 904);
                result.ShouldNotContain(x => x.EmployeeId == 888);
                result.ShouldNotContain(x => x.EmployeeId == 889);
                result.ShouldNotContain(x => x.EmployeeId == 890);
                result.ShouldNotContain(x => x.EmployeeId == 905);
            });
        }

        [Fact]
        public void QueryAllRequestEmployee_Test1()
        {
            var expectedCount = 58;

            WithUnitOfWork(() =>
            {
                var result = _salaryRequestManager.QueryAllRequestEmployee();

                result.Count().ShouldBe(expectedCount);
                result.ShouldContain(requestEmployee => requestEmployee.Id == 1635);
                result.ShouldContain(requestEmployee => requestEmployee.SalaryChangeRequestId == 143);
                result.ShouldContain(requestEmployee => requestEmployee.EmployeeId == 880);
                result.ShouldContain(requestEmployee => requestEmployee.LevelInfo.Name == "Fresher-");
                result.ShouldContain(requestEmployee => requestEmployee.LevelInfo.Color == "#60b8ff");
                result.ShouldContain(requestEmployee => requestEmployee.ToLevelId == 315);
                result.ShouldContain(requestEmployee => requestEmployee.FromUserType == UserType.Staff);
                result.ShouldContain(requestEmployee => requestEmployee.ToUserType == UserType.Staff);
                result.ShouldContain(requestEmployee => requestEmployee.JobPositionInfo.Name == "Tester");
                result.ShouldContain(requestEmployee => requestEmployee.JobPositionInfo.Color == "#d20f0f");
                result.ShouldContain(requestEmployee => requestEmployee.ToJobPositionId == 48);
                result.ShouldContain(requestEmployee => requestEmployee.Salary == 6000000);
                result.ShouldContain(requestEmployee => requestEmployee.ToSalary == 7000000);
                result.ShouldContain(requestEmployee => requestEmployee.ApplyMonth == new DateTime(2022, 12, 1));
            });
        }

        [Fact]
        public void GetRequestEmployeeById_Test1()
        {
            // Standard test case
            var expectedRequestEmployee = new GetRequestEmployeeDto
            {
                Id = 1635,
                SalaryChangeRequestId = 143,
                EmployeeId = 880,
                LevelInfo = new BadgeInfoDto {
                    Name = "Fresher-",
                    Color = "#60b8ff"
                },
                ToLevelId = 315,
                FromUserType = UserType.Staff,
                ToUserType = UserType.Staff,
                JobPositionInfo = new BadgeInfoDto
                {
                    Name = "Tester",
                    Color = "#d20f0f"
                },
                ToJobPositionId = 48,
                Salary = 6000000,
                ToSalary = 7000000,
                ApplyMonth = new DateTime(2022, 12, 1)
            };

            WithUnitOfWork(() =>
            {
                var result = _salaryRequestManager.GetRequestEmployeeById(expectedRequestEmployee.Id);

                result.SalaryChangeRequestId.ShouldBe(expectedRequestEmployee.SalaryChangeRequestId);
                result.EmployeeId.ShouldBe(expectedRequestEmployee.EmployeeId);
                result.LevelInfo.Name.ShouldBe(expectedRequestEmployee.LevelInfo.Name);
                result.LevelInfo.Color.ShouldBe(expectedRequestEmployee.LevelInfo.Color);
                result.ToLevelId.ShouldBe(expectedRequestEmployee.ToLevelId);
                result.FromUserType.ShouldBe(expectedRequestEmployee.FromUserType);
                result.ToUserType.ShouldBe(expectedRequestEmployee.ToUserType);
                result.JobPositionInfo.Name.ShouldBe(expectedRequestEmployee.JobPositionInfo.Name);
                result.JobPositionInfo.Color.ShouldBe(expectedRequestEmployee.JobPositionInfo.Color);
                result.ToJobPositionId.ShouldBe(expectedRequestEmployee.ToJobPositionId);
                result.Salary.ShouldBe(expectedRequestEmployee.Salary);
                result.ToSalary.ShouldBe(expectedRequestEmployee.ToSalary);
                result.ApplyMonth.ShouldBe(expectedRequestEmployee.ApplyMonth);
            });
        }

        [Fact]
        public void GetRequestEmployeeById_Test2()
        {
            // Non-existent RequestEmployee
            var expectedRequestEmployeeId = 1;

            WithUnitOfWork(() =>
            {
                var result = _salaryRequestManager.GetRequestEmployeeById(expectedRequestEmployeeId);

                result.ShouldBeNull();
            });
        }

        [Fact]
        public async Task AddEmployeeTosalaryRequest_Test1()
        {
            // Standard test case
            var input = new AddOrUpdateEmployeeRequestDto
            {
                Id = 1858,
                SalaryChangeRequestId = 154,
                EmployeeId = 900,
                LevelId = 314,
                ToLevelId = 315,
                FromUserType = UserType.Internship,
                ToUserType = UserType.Collaborators,
                JobPositionId = 47,
                ToJobPositionId = 47,
                Salary = 4000000,
                ToSalary = 6000000,
                ApplyDate = new DateTime(2023, 1, 15),
                ContractEndDate = new DateTime(2023, 12, 31),
                ProbationPercentage = 0,
                BasicSalary = 6000000,
                Note = "Lên CTV",
                ContractCode = "HOA.NGUYENTHIQUYNH/1/2023/HĐCTV-NCC",
                HasContract = true,
                Type = SalaryRequestType.Change,
            };
            var expectedContractId = 1238;

            var allRequestEmployeesBeforeAdd = new List<SalaryChangeRequestEmployee>();

            await WithUnitOfWorkAsync(async () =>
            {
                allRequestEmployeesBeforeAdd = _workScope.GetAll<SalaryChangeRequestEmployee>().ToList();

                var result = await _salaryRequestManager.AddEmployeeTosalaryRequest(input);

                result.ShouldBe(input.Id);
            });

            await WithUnitOfWorkAsync(async () =>
            {
                var allRequestEmployeesAfterAdd = _workScope.GetAll<SalaryChangeRequestEmployee>().ToList();
                var requestEmployee = await _workScope.GetAsync<SalaryChangeRequestEmployee>(input.Id);
                var contract = await _workScope.GetAsync<EmployeeContract>(expectedContractId);

                allRequestEmployeesAfterAdd.Count.ShouldBeGreaterThan(allRequestEmployeesBeforeAdd.Count);

                requestEmployee.ShouldNotBeNull();
                requestEmployee.EmployeeId.ShouldBe(input.EmployeeId);
                requestEmployee.LevelId.ShouldBe(input.LevelId);
                requestEmployee.ToLevelId.ShouldBe(input.ToLevelId);
                requestEmployee.JobPositionId.ShouldBe(input.JobPositionId);
                requestEmployee.ToJobPositionId.ShouldBe(input.ToJobPositionId);
                requestEmployee.Salary.ShouldBe(input.Salary);
                requestEmployee.ToSalary.ShouldBe(input.ToSalary);
                requestEmployee.ApplyDate.ShouldBe(input.ApplyDate);
                requestEmployee.Note.ShouldBe(input.Note);
                requestEmployee.HasContract.ShouldBe(input.HasContract);
                Assert.Equal(input.Type, requestEmployee.Type);
                Assert.Equal(input.FromUserType, requestEmployee.FromUserType);
                Assert.Equal(input.ToUserType, requestEmployee.ToUserType);

                contract.StartDate.ShouldBe(input.ApplyDate);
                contract.EndDate.ShouldBe(input.ContractEndDate);
                contract.ProbationPercentage.ShouldBe(input.ProbationPercentage);
                contract.Code.ShouldBe(input.ContractCode);
                contract.BasicSalary.ShouldBe(input.BasicSalary);
                contract.SalaryRequestEmployeeId.ShouldBe(input.Id);
                contract.JobPositionId.ShouldBe(input.ToJobPositionId);
                contract.LevelId.ShouldBe(input.ToLevelId);
                contract.EmployeeId.ShouldBe(input.EmployeeId);
            });
        }

        [Fact]
        public async Task AddEmployeeTosalaryRequest_Test2()
        {
            // Employee already exist in salary request
            var input = new AddOrUpdateEmployeeRequestDto
            {
                SalaryChangeRequestId = 154,
                EmployeeId = 889
            };
            var expectedMessage = $"Employee with Id= {input.EmployeeId} is already in this salary change request";

            await WithUnitOfWorkAsync(async () =>
            {
                var exception = await Assert.ThrowsAsync<UserFriendlyException>(async () =>
                {
                    await _salaryRequestManager.AddEmployeeTosalaryRequest(input);
                });

                exception.Message.ShouldBe(expectedMessage);
            });
        }

        [Fact]
        public async Task UpdateSalaryRequestEmployee_Test1()
        {
            // HasContract is true and contract existed
            var input = new AddOrUpdateEmployeeRequestDto
            {
                Id = 1757,
                SalaryChangeRequestId = 154,
                EmployeeId = 900,
                LevelId = 314,
                ToLevelId = 315,
                FromUserType = UserType.Internship,
                ToUserType = UserType.Collaborators,
                JobPositionId = 47,
                ToJobPositionId = 47,
                Salary = 4000000,
                ToSalary = 6000000,
                ApplyDate = new DateTime(2023, 1, 15),
                ContractEndDate = new DateTime(2023, 12, 31),
                Note = "Lên CTV",
                ContractCode = "HOA.NGUYENTHIQUYNH/1/2023/HĐCTV-NCC",
                HasContract = true,
                Type = SalaryRequestType.Initial,
            };
            var expectedContractId = 1237;

            await WithUnitOfWorkAsync(async () =>
            {
                var result = await _salaryRequestManager.UpdateSalaryRequestEmployee(input);

                result.Id.ShouldBe(input.Id);
                result.SalaryChangeRequestId.ShouldBe(input.SalaryChangeRequestId);
                result.EmployeeId.ShouldBe(input.EmployeeId);
                result.LevelId.ShouldBe(input.LevelId);
                result.ToLevelId.ShouldBe(input.ToLevelId);
                result.FromUserType.ShouldBe(input.FromUserType);
                result.ToUserType.ShouldBe(input.ToUserType);
                result.JobPositionId.ShouldBe(input.JobPositionId);
                result.ToJobPositionId.ShouldBe(input.ToJobPositionId);
                result.Salary.ShouldBe(input.Salary);
                result.ToSalary.ShouldBe(input.ToSalary);
                result.ApplyDate.ShouldBe(input.ApplyDate);
                result.ContractEndDate.ShouldBe(input.ContractEndDate);
                result.Note.ShouldBe(input.Note);
                result.ContractCode.ShouldBe(input.ContractCode);
                result.HasContract.ShouldBe(input.HasContract);
                result.Type.ShouldBe(input.Type);
            });

            await WithUnitOfWorkAsync(async () =>
            {
                var salaryChangeRequestEmployee = await _workScope.GetAsync<SalaryChangeRequestEmployee>(input.Id);
                salaryChangeRequestEmployee.SalaryChangeRequestId.ShouldBe(input.SalaryChangeRequestId);
                salaryChangeRequestEmployee.EmployeeId.ShouldBe(input.EmployeeId);
                salaryChangeRequestEmployee.LevelId.ShouldBe(input.LevelId);
                salaryChangeRequestEmployee.ToLevelId.ShouldBe(input.ToLevelId);
                salaryChangeRequestEmployee.FromUserType.ShouldBe(input.FromUserType);
                salaryChangeRequestEmployee.ToUserType.ShouldBe(input.ToUserType);
                salaryChangeRequestEmployee.JobPositionId.ShouldBe(input.JobPositionId);
                salaryChangeRequestEmployee.ToJobPositionId.ShouldBe(input.ToJobPositionId);
                salaryChangeRequestEmployee.Salary.ShouldBe(input.Salary);
                salaryChangeRequestEmployee.ToSalary.ShouldBe(input.ToSalary);
                salaryChangeRequestEmployee.ApplyDate.ShouldBe(input.ApplyDate);
                salaryChangeRequestEmployee.Note.ShouldBe(input.Note);
                salaryChangeRequestEmployee.HasContract.ShouldBe(input.HasContract);
                Assert.Equal(input.Type, salaryChangeRequestEmployee.Type);

                var contract = await _workScope.GetAsync<EmployeeContract>(expectedContractId);
                contract.Code.ShouldBe(input.ContractCode);
                contract.EndDate.ShouldBe(input.ContractEndDate);
            });
        }

        [Fact]
        public async Task UpdateSalaryRequestEmployee_Test2()
        {
            // HasContract is true and no contract existed
            var input = new AddOrUpdateEmployeeRequestDto
            {
                Id = 1631,
                SalaryChangeRequestId = 154,
                EmployeeId = 900,
                LevelId = 314,
                ToLevelId = 315,
                FromUserType = UserType.Internship,
                ToUserType = UserType.Collaborators,
                JobPositionId = 47,
                ToJobPositionId = 47,
                Salary = 4000000,
                ToSalary = 6000000,
                ProbationPercentage = 100,
                ApplyDate = new DateTime(2023, 1, 15),
                ContractEndDate = new DateTime(2023, 12, 31),
                ContractCode = "HOA.NGUYENTHIQUYNH/1/2023/HĐCTV-NCC",
                Note = "Lên CTV",
                HasContract = true,
                Type = SalaryRequestType.Initial,
            };

            var expectedContract = new EmployeeContract
            {
                Id = 1238,
                Code = input.ContractCode,
                EmployeeId = input.EmployeeId,
                BasicSalary = input.BasicSalary,
                RealSalary = input.ToSalary,
                JobPositionId = input.ToJobPositionId,
                LevelId = input.ToLevelId,
                UserType = input.ToUserType,
                ProbationPercentage = input.ProbationPercentage,
                EndDate = input.ContractEndDate,
                StartDate = input.ApplyDate,
                SalaryRequestEmployeeId = input.Id
            };

            await WithUnitOfWorkAsync(async () =>
            {
                var result = await _salaryRequestManager.UpdateSalaryRequestEmployee(input);

                result.Id.ShouldBe(input.Id);
                result.SalaryChangeRequestId.ShouldBe(input.SalaryChangeRequestId);
                result.EmployeeId.ShouldBe(input.EmployeeId);
                result.LevelId.ShouldBe(input.LevelId);
                result.ToLevelId.ShouldBe(input.ToLevelId);
                result.FromUserType.ShouldBe(input.FromUserType);
                result.ToUserType.ShouldBe(input.ToUserType);
                result.JobPositionId.ShouldBe(input.JobPositionId);
                result.ToJobPositionId.ShouldBe(input.ToJobPositionId);
                result.Salary.ShouldBe(input.Salary);
                result.ToSalary.ShouldBe(input.ToSalary);
                result.ApplyDate.ShouldBe(input.ApplyDate);
                result.ContractEndDate.ShouldBe(input.ContractEndDate);
                result.Note.ShouldBe(input.Note);
                result.ContractCode.ShouldBe(input.ContractCode);
                result.HasContract.ShouldBe(input.HasContract);
                result.Type.ShouldBe(input.Type);
            });

            await WithUnitOfWorkAsync(async () =>
            {
                var salaryChangeRequestEmployee = await _workScope.GetAsync<SalaryChangeRequestEmployee>(input.Id);
                salaryChangeRequestEmployee.SalaryChangeRequestId.ShouldBe(input.SalaryChangeRequestId);
                salaryChangeRequestEmployee.EmployeeId.ShouldBe(input.EmployeeId);
                salaryChangeRequestEmployee.LevelId.ShouldBe(input.LevelId);
                salaryChangeRequestEmployee.ToLevelId.ShouldBe(input.ToLevelId);
                salaryChangeRequestEmployee.FromUserType.ShouldBe(input.FromUserType);
                salaryChangeRequestEmployee.ToUserType.ShouldBe(input.ToUserType);
                salaryChangeRequestEmployee.JobPositionId.ShouldBe(input.JobPositionId);
                salaryChangeRequestEmployee.ToJobPositionId.ShouldBe(input.ToJobPositionId);
                salaryChangeRequestEmployee.Salary.ShouldBe(input.Salary);
                salaryChangeRequestEmployee.ToSalary.ShouldBe(input.ToSalary);
                salaryChangeRequestEmployee.ApplyDate.ShouldBe(input.ApplyDate);
                salaryChangeRequestEmployee.Note.ShouldBe(input.Note);
                salaryChangeRequestEmployee.HasContract.ShouldBe(input.HasContract);
                Assert.Equal(input.Type, salaryChangeRequestEmployee.Type);

                var contract = await _workScope.GetAsync<EmployeeContract>(expectedContract.Id);
                contract.Code.ShouldBe(expectedContract.Code);
                contract.EmployeeId.ShouldBe(expectedContract.EmployeeId);
                contract.BasicSalary.ShouldBe(expectedContract.BasicSalary);
                contract.RealSalary.ShouldBe(expectedContract.RealSalary);
                contract.JobPositionId.ShouldBe(expectedContract.JobPositionId);
                contract.LevelId.ShouldBe(expectedContract.LevelId);
                contract.UserType.ShouldBe(expectedContract.UserType);
                contract.ProbationPercentage.ShouldBe(expectedContract.ProbationPercentage);
                contract.EndDate.ShouldBe(expectedContract.EndDate);
                contract.StartDate.ShouldBe(expectedContract.StartDate);
                contract.SalaryRequestEmployeeId.ShouldBe(expectedContract.SalaryRequestEmployeeId);
            });
        }

        [Fact]
        public async Task UpdateSalaryRequestEmployee_Test3()
        {
            // HasContract is false and contract existed
            var input = new AddOrUpdateEmployeeRequestDto
            {
                Id = 1757,
                SalaryChangeRequestId = 154,
                EmployeeId = 900,
                LevelId = 314,
                ToLevelId = 315,
                FromUserType = UserType.Internship,
                ToUserType = UserType.Collaborators,
                JobPositionId = 47,
                ToJobPositionId = 47,
                Salary = 4000000,
                ToSalary = 6000000,
                ApplyDate = new DateTime(2023, 1, 15),
                Note = "Lên CTV",
                HasContract = false,
                Type = SalaryRequestType.Initial,
            };

            var allContractsBeforeDelete = new List<EmployeeContract>();

            await WithUnitOfWorkAsync(async () =>
            {
                allContractsBeforeDelete = _workScope.GetAll<EmployeeContract>().ToList();

                var result = await _salaryRequestManager.UpdateSalaryRequestEmployee(input);

                result.Id.ShouldBe(input.Id);
                result.SalaryChangeRequestId.ShouldBe(input.SalaryChangeRequestId);
                result.EmployeeId.ShouldBe(input.EmployeeId);
                result.LevelId.ShouldBe(input.LevelId);
                result.ToLevelId.ShouldBe(input.ToLevelId);
                result.FromUserType.ShouldBe(input.FromUserType);
                result.ToUserType.ShouldBe(input.ToUserType);
                result.JobPositionId.ShouldBe(input.JobPositionId);
                result.ToJobPositionId.ShouldBe(input.ToJobPositionId);
                result.Salary.ShouldBe(input.Salary);
                result.ToSalary.ShouldBe(input.ToSalary);
                result.ApplyDate.ShouldBe(input.ApplyDate);
                result.Note.ShouldBe(input.Note);
                result.HasContract.ShouldBe(input.HasContract);
                result.Type.ShouldBe(input.Type);
            });

            await WithUnitOfWorkAsync(async () =>
            {
                var salaryChangeRequestEmployee = await _workScope.GetAsync<SalaryChangeRequestEmployee>(input.Id);
                salaryChangeRequestEmployee.SalaryChangeRequestId.ShouldBe(input.SalaryChangeRequestId);
                salaryChangeRequestEmployee.EmployeeId.ShouldBe(input.EmployeeId);
                salaryChangeRequestEmployee.LevelId.ShouldBe(input.LevelId);
                salaryChangeRequestEmployee.ToLevelId.ShouldBe(input.ToLevelId);
                salaryChangeRequestEmployee.FromUserType.ShouldBe(input.FromUserType);
                salaryChangeRequestEmployee.ToUserType.ShouldBe(input.ToUserType);
                salaryChangeRequestEmployee.JobPositionId.ShouldBe(input.JobPositionId);
                salaryChangeRequestEmployee.ToJobPositionId.ShouldBe(input.ToJobPositionId);
                salaryChangeRequestEmployee.Salary.ShouldBe(input.Salary);
                salaryChangeRequestEmployee.ToSalary.ShouldBe(input.ToSalary);
                salaryChangeRequestEmployee.ApplyDate.ShouldBe(input.ApplyDate);
                salaryChangeRequestEmployee.Note.ShouldBe(input.Note);
                salaryChangeRequestEmployee.HasContract.ShouldBe(input.HasContract);
                Assert.Equal(input.Type, salaryChangeRequestEmployee.Type);

                var allContractsAfterDelete = _workScope.GetAll<EmployeeContract>().ToList();
                allContractsAfterDelete.Count.ShouldBeLessThan(allContractsBeforeDelete.Count);
            });
        }

        [Fact]
        public async Task UpdateRequestStatus_Test1()
        {
            // Request status is not Executed
            var expectedChangeRequest = new UpdateChangeRequestDto
            {
                RequestId = 154,
                Status = SalaryRequestStatus.Approved
            };

            await WithUnitOfWorkAsync(async () =>
            {
                await _salaryRequestManager.UpdateRequestStatus(expectedChangeRequest);
            });

            await WithUnitOfWorkAsync(async () =>
            {
                var salaryRequest = await _workScope.GetAsync<SalaryChangeRequest>(expectedChangeRequest.RequestId);

                salaryRequest.Status.ShouldBe(expectedChangeRequest.Status);
            });
        }

        [Fact]
        public async Task UpdateRequestStatus_Test2()
        {
            // Request status is Executed
            var expectedChangeRequest = new UpdateChangeRequestDto
            {
                RequestId = 154,
                Status = SalaryRequestStatus.Executed
            };

            var expectedEmployee = new Employee
            {
                Id = 889,
                RealSalary = 6800000.0,
                UserType = UserType.ProbationaryStaff,
                LevelId = 315,
                JobPositionId = 47,
                StartWorkingDate = new DateTime(2022, 12, 15)
            };

            await WithUnitOfWorkAsync(async () =>
            {
                await _salaryRequestManager.UpdateRequestStatus(expectedChangeRequest);
            });

            await WithUnitOfWorkAsync(async () =>
            {
                var salaryRequest = await _workScope.GetAsync<SalaryChangeRequest>(expectedChangeRequest.RequestId);
                salaryRequest.Status.ShouldBe(expectedChangeRequest.Status);

                var employee = await _workScope.GetAsync<Employee>(expectedEmployee.Id);
                employee.RealSalary.ShouldBe(expectedEmployee.RealSalary);
                employee.UserType.ShouldBe(expectedEmployee.UserType);
                employee.LevelId.ShouldBe(expectedEmployee.LevelId);
                employee.JobPositionId.ShouldBe(expectedEmployee.JobPositionId);
                employee.StartWorkingDate.ShouldBe(expectedEmployee.StartWorkingDate);
            });
        }

        [Fact]
        public async Task DeleteSalaryRequestEmployee_Test1()
        {
            // Standard test case
            var salaryRequestEmployeeId = 1757;

            var allSalaryRequestEmployeesBeforeDelete = new List<SalaryChangeRequestEmployee>();
            var allContractsBeforeDelete = new List<EmployeeContract>();

            await WithUnitOfWorkAsync(async () =>
            {
                allSalaryRequestEmployeesBeforeDelete = _workScope.GetAll<SalaryChangeRequestEmployee>().ToList();
                allContractsBeforeDelete = _workScope.GetAll<EmployeeContract>().ToList();

                await _salaryRequestManager.DeleteSalaryRequestEmployee(salaryRequestEmployeeId);
            });

            await WithUnitOfWorkAsync(async() =>
            {
                var allsalaryRequestEmployeesAfterDelete = _workScope.GetAll<SalaryChangeRequestEmployee>().ToList();
                var allContractsAfterDelete = _workScope.GetAll<EmployeeContract>().ToList();

                allsalaryRequestEmployeesAfterDelete.Count.ShouldBeLessThan(allSalaryRequestEmployeesBeforeDelete.Count);
                allContractsAfterDelete.Count.ShouldBeLessThan(allContractsBeforeDelete.Count);
                allContractsAfterDelete.ShouldNotContain(contract => contract.SalaryRequestEmployeeId == salaryRequestEmployeeId);

                await Assert.ThrowsAsync<EntityNotFoundException>(async () =>
                {
                    await _workScope.GetAsync<SalaryChangeRequestEmployee>(salaryRequestEmployeeId);
                });
            });
        }

        [Fact]
        public async Task GetEmployeesInSalaryRequest_Test1()
        {
            var requestId = 143;
            var input = new InputGetEmployeeInSalaryRequestDto
            {
                GridParam = new GridParam
                {
                    MaxResultCount = 10,
                },
                ToLevelIds = new List<long> { 315 },
                ToUsertypes = new List<UserType> { UserType.Staff },
                ToJobPositionIds = new List<long> { 48 },
                BranchIds = new List<long> { 94 }
            };

            var expectedTotalCount = 1;
            var expectedItemsCount = 1;

            await WithUnitOfWorkAsync(async () =>
            {
                var result = await _salaryRequestManager.GetEmployeesInSalaryRequest(requestId, input);

                result.TotalCount.ShouldBe(expectedTotalCount);
                result.Items.Count.ShouldBe(expectedItemsCount);
                result.Items.ShouldContain(requestEmployee => requestEmployee.Id == 1635);
                result.Items.ShouldContain(requestEmployee => requestEmployee.SalaryChangeRequestId == 143);
                result.Items.ShouldContain(requestEmployee => requestEmployee.EmployeeId == 880);
                result.Items.ShouldContain(requestEmployee => requestEmployee.ToLevelId == 315);
                result.Items.ShouldContain(requestEmployee => requestEmployee.FromUserType == UserType.Staff);
                result.Items.ShouldContain(requestEmployee => requestEmployee.ToUserType == UserType.Staff);
                result.Items.ShouldContain(requestEmployee => requestEmployee.ToJobPositionId == 48);
                result.Items.ShouldContain(requestEmployee => requestEmployee.Salary == 6000000);
                result.Items.ShouldContain(requestEmployee => requestEmployee.ToSalary == 7000000);
                result.Items.ShouldContain(requestEmployee => requestEmployee.ApplyDate == new DateTime(2022, 12, 1));
                result.Items.ShouldContain(requestEmployee => requestEmployee.Note == "checkpoint");
                result.Items.ShouldContain(requestEmployee => requestEmployee.HasContract == true);
            });
        }

        [Fact]
        public async Task UpdateRequestEmployeeInfo_Test1()
        {
            // Standard test case
            var input = new UpdateRequestEmployeeInfoDto
            {
                Id = 1604,
                SalaryChangeRequestId = 152,
                EmployeeId = 882,
                LevelId = 314,
                ToLevelId = 315,
                FromUserType = UserType.ProbationaryStaff,
                ToUserType = UserType.Staff,
                JobPositionId = 47,
                ToJobPositionId = 47,
                Salary = 4000000,
                ToSalary = 6000000,
                ApplyDate = new DateTime(2023, 1, 1),
                Note = "Lương khởi điểm",
                Type = SalaryRequestType.Initial,
                HasContract = true,
            };

            await WithUnitOfWorkAsync(async () =>
            {
                var result = await _salaryRequestManager.UpdateRequestEmployeeInfo(input);

                result.Id.ShouldBe(input.Id);
                result.SalaryChangeRequestId.ShouldBe(input.SalaryChangeRequestId);
                result.EmployeeId.ShouldBe(input.EmployeeId);
                result.LevelId.ShouldBe(input.LevelId);
                result.ToLevelId.ShouldBe(input.ToLevelId);
                result.FromUserType.ShouldBe(input.FromUserType);
                result.ToUserType.ShouldBe(input.ToUserType);
                result.JobPositionId.ShouldBe(input.JobPositionId);
                result.ToJobPositionId.ShouldBe(input.ToJobPositionId);
                result.Salary.ShouldBe(input.Salary);
                result.ToSalary.ShouldBe(input.ToSalary);
                result.ApplyDate.ShouldBe(input.ApplyDate);
                result.Note.ShouldBe(input.Note);
                result.Type.ShouldBe(input.Type);
                result.HasContract.ShouldBe(input.HasContract);
            });

            await WithUnitOfWorkAsync(async () =>
            {
                var result = await _workScope.GetAsync<SalaryChangeRequestEmployee>(input.Id);

                result.Id.ShouldBe(input.Id);
                result.SalaryChangeRequestId.ShouldBe(input.SalaryChangeRequestId);
                result.EmployeeId.ShouldBe(input.EmployeeId);
                result.LevelId.ShouldBe(input.LevelId);
                result.ToLevelId.ShouldBe(input.ToLevelId);
                result.FromUserType.ShouldBe(input.FromUserType);
                result.ToUserType.ShouldBe(input.ToUserType);
                result.JobPositionId.ShouldBe(input.JobPositionId);
                result.ToJobPositionId.ShouldBe(input.ToJobPositionId);
                result.Salary.ShouldBe(input.Salary);
                result.ToSalary.ShouldBe(input.ToSalary);
                result.ApplyDate.ShouldBe(input.ApplyDate);
                result.Note.ShouldBe(input.Note);
                result.Type.ShouldBe(input.Type);
                result.HasContract.ShouldBe(input.HasContract);
            });
        }

        [Fact]
        public async Task UpdateRequestEmployeeInfo_Test2()
        {
            // Non-existent request
            var input = new UpdateRequestEmployeeInfoDto
            {
                Id = 1
            };
            var expectedMessage = $"Can't find request with id {input.Id}";

            await WithUnitOfWorkAsync(async () =>
            {
                var exception = await Assert.ThrowsAsync<UserFriendlyException>(async () =>
                {
                    await _salaryRequestManager.UpdateRequestEmployeeInfo(input);
                });

                exception.Message.ShouldBe(expectedMessage);
            });
        }

        [Fact]
        public void IsAllowUpdateEmployee_Test1()
        {
            // Update employee is allowed
            var screId = 1685;
            var employeeId = 905;

            WithUnitOfWork(() =>
            {
                var result = _salaryRequestManager.IsAllowUpdateEmployee(screId, employeeId);

                result.ShouldBe(true);
            });
        }

        [Fact]
        public void IsAllowUpdateEmployee_Test2()
        {
            // Update employee is not allowed
            var screId = 1602;
            var employeeId = 880;

            WithUnitOfWork(() =>
            {
                var result = _salaryRequestManager.IsAllowUpdateEmployee(screId, employeeId);

                result.ShouldBe(false);
            });
        }

        [Fact]
        public async Task ImportCheckpoint_Test1()
        {
            // Standard test case
            //var file = ".\\Managers\\SalaryRequests\\Files\\ImportCheckpoint-test-1.xlsx";
            //var stream = new MemoryStream(File.ReadAllBytes(file).ToArray());
            //var formFile = new FormFile(stream, 0, stream.Length, "streamFile", file.Split(@"\").Last());

            var fileName = "ImportCheckpoint-test-1.xlsx";
            var root = Directory.GetCurrentDirectory().Replace("\\bin\\Debug\\net6.0", "").Replace("/bin/Debug/net6.0", "");
            string path = Path.Combine(root, "Managers", "SalaryRequests", "Files", fileName);

            var stream = new MemoryStream(File.ReadAllBytes(path).ToArray());
            var formFile = new FormFile(stream, 0, stream.Length, "streamFile", fileName);



            var input = new ImportCheckpointDto
            {
                File = formFile,
                SalaryChangeRequestId = 143
            };

            var expectedRequestEmployee = new
            {
                Id = 1758,
                Email = "thong.nguyenba@ncc.asia",
                ToLevelId = 318,
                ToSalary = 10000000
            };

            var allRequestEmployeesBeforeImport = new List<SalaryChangeRequestEmployee>();

            await WithUnitOfWorkAsync(async () =>
            {
                allRequestEmployeesBeforeImport = _workScope.GetAll<SalaryChangeRequestEmployee>().ToList();

                var result = await _salaryRequestManager.ImportCheckpoint(input);

                var successList = result.GetType().GetProperties().First(o => o.Name == "successList").GetValue(result, null);
                var failedList = result.GetType().GetProperties().First(o => o.Name == "failedList").GetValue(result, null);

                var success = successList as ICollection<SalaryChangeRequestEmployee>;
                var failed = failedList as ICollection<FailResponeDto>;

                Assert.Equal(1, success.Count);
                Assert.Empty(failed);

                success.First().Id.ShouldBe(expectedRequestEmployee.Id);
                success.First().Employee.Email.ShouldBe(expectedRequestEmployee.Email);
                success.First().ToLevelId.ShouldBe(expectedRequestEmployee.ToLevelId);
                success.First().ToSalary.ShouldBe(expectedRequestEmployee.ToSalary);
            });

            await WithUnitOfWorkAsync(async () =>
            {
                var allRequestEmployeesAfterImport = _workScope.GetAll<SalaryChangeRequestEmployee>();
                var requestEmployee = await _workScope.GetAsync<SalaryChangeRequestEmployee>(expectedRequestEmployee.Id);
                var employee = await _workScope.GetAsync<Employee>(requestEmployee.EmployeeId);

                allRequestEmployeesAfterImport.Count().ShouldBeGreaterThan(allRequestEmployeesBeforeImport.Count);
                requestEmployee.ShouldNotBeNull();
                employee.Email.ShouldBe(expectedRequestEmployee.Email);
                requestEmployee.ToLevelId.ShouldBe(expectedRequestEmployee.ToLevelId);
                requestEmployee.ToSalary.ShouldBe(expectedRequestEmployee.ToSalary);
            });
        }

        [Fact]
        public async Task ImportCheckpoint_Test2()
        {
            // File input is null
            var input = new ImportCheckpointDto
            {
                File = null,
                SalaryChangeRequestId = 143
            };
            var expectedMessage = "File upload is invalid";

            await WithUnitOfWorkAsync(async () =>
            {
                var exception = await Assert.ThrowsAsync<UserFriendlyException>(async () =>
                {
                    await _salaryRequestManager.ImportCheckpoint(input);
                });

                exception.Message.ShouldBe(expectedMessage);
            });
        }

        [Fact]
        public async Task ImportCheckpoint_Test3()
        {
            // Wrong file extension
            //var file = ".\\Managers\\SalaryRequests\\Files\\ImportCheckpoint-test-2.pdf";
            //var stream = new MemoryStream(File.ReadAllBytes(file).ToArray());
            //var formFile = new FormFile(stream, 0, stream.Length, "streamFile", file.Split(@"\").Last());
            var fileName = "ImportCheckpoint-test-2.pdf";
            var root = Directory.GetCurrentDirectory().Replace("\\bin\\Debug\\net6.0", "").Replace("/bin/Debug/net6.0", "");
            string path = Path.Combine(root, "Managers", "SalaryRequests", "Files", fileName);

            var stream = new MemoryStream(File.ReadAllBytes(path).ToArray());
            var formFile = new FormFile(stream, 0, stream.Length, "streamFile", fileName);
            var input = new ImportCheckpointDto
            {
                File = formFile,
                SalaryChangeRequestId = 143
            };
            var expectedMessage = "File upload is invalid";

            await WithUnitOfWorkAsync(async () =>
            {
                var exception = await Assert.ThrowsAsync<UserFriendlyException>(async () =>
                {
                    await _salaryRequestManager.ImportCheckpoint(input);
                });

                exception.Message.ShouldBe(expectedMessage);
            });
        }

        [Fact]
        public async Task ImportCheckpoint_Test4()
        {
            // Some requests fail
            //var file = ".\\Managers\\SalaryRequests\\Files\\ImportCheckpoint-test-4.xlsx";
            //var stream = new MemoryStream(File.ReadAllBytes(file).ToArray());
            //var formFile = new FormFile(stream, 0, stream.Length, "streamFile", file.Split(@"\").Last());
            var fileName = "ImportCheckpoint-test-4.xlsx";
            var root = Directory.GetCurrentDirectory().Replace("\\bin\\Debug\\net6.0", "").Replace("/bin/Debug/net6.0", "");
            string path = Path.Combine(root, "Managers", "SalaryRequests", "Files", fileName);

            var stream = new MemoryStream(File.ReadAllBytes(path).ToArray());
            var formFile = new FormFile(stream, 0, stream.Length, "streamFile", fileName);
            var input = new ImportCheckpointDto
            {
                File = formFile,
                SalaryChangeRequestId = 143
            };
            var allRequestEmployeesBeforeImport = new List<SalaryChangeRequestEmployee>();

            var expectedFirstRequestEmployee = new
            {
                Id = 1758,
                Email = "thong.nguyenba@ncc.asia",
                ToLevelId = 318,
                ToSalary = 10000000
            };

            var expectedSecondRequestEmployee = new
            {
                Id = 1759,
                Email = "tho.voquoc@ncc.asia",
                ToLevelId = 315,
                ToSalary = 6000000
            };

            await WithUnitOfWorkAsync(async () =>
            {
                allRequestEmployeesBeforeImport = _workScope.GetAll<SalaryChangeRequestEmployee>().ToList();

                var result = await _salaryRequestManager.ImportCheckpoint(input);

                var successList = result.GetType().GetProperties().First(o => o.Name == "successList").GetValue(result, null);
                var failedList = result.GetType().GetProperties().First(o => o.Name == "failedList").GetValue(result, null);

                var success = successList as ICollection<SalaryChangeRequestEmployee>;
                var failed = failedList as ICollection<FailResponeDto>;

                Assert.Equal(2, success.Count);
                Assert.Equal(2, failed.Count);

                success.ElementAt(0).Id.ShouldBe(expectedFirstRequestEmployee.Id);
                success.ElementAt(0).Employee.Email.ShouldBe(expectedFirstRequestEmployee.Email);
                success.ElementAt(0).ToLevelId.ShouldBe(expectedFirstRequestEmployee.ToLevelId);
                success.ElementAt(0).ToSalary.ShouldBe(expectedFirstRequestEmployee.ToSalary);

                success.ElementAt(1).Id.ShouldBe(expectedSecondRequestEmployee.Id);
                success.ElementAt(1).Employee.Email.ShouldBe(expectedSecondRequestEmployee.Email);
                success.ElementAt(1).ToLevelId.ShouldBe(expectedSecondRequestEmployee.ToLevelId);
                success.ElementAt(1).ToSalary.ShouldBe(expectedSecondRequestEmployee.ToSalary);
            });

            await WithUnitOfWorkAsync(async () =>
            {
                var allRequestEmployeesAfterImport = _workScope.GetAll<SalaryChangeRequestEmployee>();

                var firstRequestEmployee = await _workScope.GetAsync<SalaryChangeRequestEmployee>(expectedFirstRequestEmployee.Id);
                var firstEmployee = await _workScope.GetAsync<Employee>(firstRequestEmployee.EmployeeId);
                var secondRequestEmployee = await _workScope.GetAsync<SalaryChangeRequestEmployee>(expectedSecondRequestEmployee.Id);
                var secondEmployee = await _workScope.GetAsync<Employee>(secondRequestEmployee.EmployeeId);

                allRequestEmployeesAfterImport.Count().ShouldBeGreaterThan(allRequestEmployeesBeforeImport.Count);

                firstRequestEmployee.ShouldNotBeNull();
                firstEmployee.Email.ShouldBe(expectedFirstRequestEmployee.Email);
                firstRequestEmployee.ToLevelId.ShouldBe(expectedFirstRequestEmployee.ToLevelId);
                firstRequestEmployee.ToSalary.ShouldBe(expectedFirstRequestEmployee.ToSalary);

                secondRequestEmployee.ShouldNotBeNull();
                secondEmployee.Email.ShouldBe(expectedSecondRequestEmployee.Email);
                secondRequestEmployee.ToLevelId.ShouldBe(expectedSecondRequestEmployee.ToLevelId);
                secondRequestEmployee.ToSalary.ShouldBe(expectedSecondRequestEmployee.ToSalary);
            });
        }

        [Fact]
        public async Task ImportCheckpoint_Test5()
        {
            // All requests fail
            //var file = ".\\Managers\\SalaryRequests\\Files\\ImportCheckpoint-test-5.xlsx";
            //var stream = new MemoryStream(File.ReadAllBytes(file).ToArray());
            //var formFile = new FormFile(stream, 0, stream.Length, "streamFile", file.Split(@"\").Last());
            var fileName = "ImportCheckpoint-test-5.xlsx";
            var root = Directory.GetCurrentDirectory().Replace("\\bin\\Debug\\net6.0", "").Replace("/bin/Debug/net6.0", "");
            string path = Path.Combine(root, "Managers", "SalaryRequests", "Files", fileName);

            var stream = new MemoryStream(File.ReadAllBytes(path).ToArray());
            var formFile = new FormFile(stream, 0, stream.Length, "streamFile", fileName);
            var input = new ImportCheckpointDto
            {
                File = formFile,
                SalaryChangeRequestId = 143
            };

            await WithUnitOfWorkAsync(async () =>
            {
                var result = await _salaryRequestManager.ImportCheckpoint(input);

                var successList = result.GetType().GetProperties().First(o => o.Name == "successList").GetValue(result, null);
                var failedList = result.GetType().GetProperties().First(o => o.Name == "failedList").GetValue(result, null);

                var success = successList as ICollection<SalaryChangeRequestEmployee>;
                var failed = failedList as ICollection<FailResponeDto>;

                Assert.Empty(success);
                Assert.Equal(2, failed.Count);
            });
        }

        //[Fact]
        //public async Task CreateChangeRequestAndContract_Test1()
        //{
        //    // Input
        //    var employee = new DicEmployeeInfoDto
        //    {
        //        Id = 880,
        //        LevelId = 315,
        //        JobPositionId = 48,
        //        ProbationPercentage = 100.0,
        //        UserType = UserType.Staff,
        //        RealSalary = 6000000
        //    };

        //    var salaryChangeRequest = new SalaryChangeRequest
        //    {
        //        Id = 143,
        //        Name = "Nâng level từ Fresher- lên Fresher",
        //        ApplyMonth = new DateTime(2023, 1, 1),
        //        Status = SalaryRequestStatus.Approved
        //    };

        //    var toSalary = 7000000;
        //    var toLevelId = 316;

        //    // Expected results
        //    var expectedSalaryRequest = new SalaryChangeRequestEmployee
        //    {
        //        Id = 1758,
        //        SalaryChangeRequestId = salaryChangeRequest.Id,
        //        EmployeeId = employee.Id,
        //        Type = SalaryRequestType.Change,
        //        LevelId = employee.LevelId,
        //        ToLevelId = toLevelId,
        //        FromUserType = employee.UserType,
        //        ToUserType = employee.UserType,
        //        Salary = employee.RealSalary,
        //        ToSalary = toSalary,
        //        ApplyDate = salaryChangeRequest.ApplyMonth,
        //        JobPositionId = employee.JobPositionId,
        //        ToJobPositionId = employee.JobPositionId,
        //        HasContract = true,
        //        Note = salaryChangeRequest.Name,
        //    };

        //    var expectedContract = new EmployeeContract
        //    {
        //        Id = 1238,
        //        UserType = employee.UserType,
        //        LevelId = toLevelId,
        //        StartDate = salaryChangeRequest.ApplyMonth,
        //        EndDate = null,
        //        RealSalary = toSalary,
        //        BasicSalary = toSalary * employee.ProbationPercentage / 100,
        //        ProbationPercentage = employee.ProbationPercentage,
        //        Code = CommonUtil.GenerateContractCode("an.phamthien@ncc.asia", salaryChangeRequest.ApplyMonth.Month, salaryChangeRequest.ApplyMonth.Year, employee.UserType),
        //        JobPositionId = employee.JobPositionId,
        //        SalaryRequestEmployeeId = expectedSalaryRequest.Id,
        //        EmployeeId = employee.Id,
        //        Note = "",
        //    };

        //    var allSalaryRequestEmployeeBeforeCreate = new List<SalaryChangeRequestEmployee>();
        //    var allContractsBeforeCreate = new List<EmployeeContract>();

        //    // Test
        //    await WithUnitOfWorkAsync(async () =>
        //    {
        //        allSalaryRequestEmployeeBeforeCreate = _workScope.GetAll<SalaryChangeRequestEmployee>().ToList();
        //        allContractsBeforeCreate = _workScope.GetAll<EmployeeContract>().ToList();

        //        var result = await _salaryRequestManager.CreateChangeRequestAndContract(employee, salaryChangeRequest, toSalary, toLevelId);

        //        result.ShouldNotBeNull();
        //        result.SalaryChangeRequestId.ShouldBe(salaryChangeRequest.Id);
        //        result.EmployeeId.ShouldBe(employee.Id);
        //        result.Type.ShouldBe(SalaryRequestType.Change);
        //        result.LevelId.ShouldBe(employee.LevelId);
        //        result.ToLevelId.ShouldBe(toLevelId);
        //        result.FromUserType.ShouldBe(employee.UserType);
        //        result.ToUserType.ShouldBe(employee.UserType);
        //        result.Salary.ShouldBe(employee.RealSalary);
        //        result.ToSalary.ShouldBe(toSalary);
        //        result.ApplyDate.ShouldBe(salaryChangeRequest.ApplyMonth);
        //        result.JobPositionId.ShouldBe(employee.JobPositionId);
        //        result.ToJobPositionId.ShouldBe(employee.JobPositionId);
        //        result.HasContract.ShouldBe(true);
        //        result.Note.ShouldBe(salaryChangeRequest.Name);
        //    });

        //    await WithUnitOfWorkAsync(async () =>
        //    {
        //        var allSalaryRequestEmployeeAfterCreate = _workScope.GetAll<SalaryChangeRequestEmployee>().ToList();
        //        var allContractsAfterCreate = _workScope.GetAll<EmployeeContract>().ToList();

        //        var salaryRequest = await _workScope.GetAsync<SalaryChangeRequestEmployee>(expectedSalaryRequest.Id);
        //        var contract = await _workScope.GetAsync<EmployeeContract>(expectedContract.Id);

        //        allSalaryRequestEmployeeAfterCreate.Count.ShouldBeGreaterThan(allSalaryRequestEmployeeBeforeCreate.Count);
        //        allContractsAfterCreate.Count.ShouldBeGreaterThan(allContractsBeforeCreate.Count);

        //        salaryRequest.SalaryChangeRequestId.ShouldBe(expectedSalaryRequest.SalaryChangeRequestId);
        //        salaryRequest.EmployeeId.ShouldBe(expectedSalaryRequest.EmployeeId);
        //        salaryRequest.Type.ShouldBe(expectedSalaryRequest.Type);
        //        salaryRequest.LevelId.ShouldBe(expectedSalaryRequest.LevelId);
        //        salaryRequest.ToLevelId.ShouldBe(expectedSalaryRequest.ToLevelId);
        //        salaryRequest.FromUserType.ShouldBe(expectedSalaryRequest.FromUserType);
        //        salaryRequest.ToUserType.ShouldBe(expectedSalaryRequest.ToUserType);
        //        salaryRequest.Salary.ShouldBe(expectedSalaryRequest.Salary);
        //        salaryRequest.ToSalary.ShouldBe(expectedSalaryRequest.ToSalary);
        //        salaryRequest.ApplyDate.ShouldBe(expectedSalaryRequest.ApplyDate);
        //        salaryRequest.JobPositionId.ShouldBe(expectedSalaryRequest.JobPositionId);
        //        salaryRequest.ToJobPositionId.ShouldBe(expectedSalaryRequest.ToJobPositionId);
        //        salaryRequest.HasContract.ShouldBe(expectedSalaryRequest.HasContract);
        //        salaryRequest.Note.ShouldBe(expectedSalaryRequest.Note);

        //        contract.UserType.ShouldBe(expectedContract.UserType);
        //        contract.LevelId.ShouldBe(expectedContract.LevelId);
        //        contract.StartDate.ShouldBe(expectedContract.StartDate);
        //        contract.EndDate.ShouldBe(expectedContract.EndDate);
        //        contract.RealSalary.ShouldBe(expectedContract.RealSalary);
        //        contract.BasicSalary.ShouldBe(expectedContract.BasicSalary);
        //        contract.ProbationPercentage.ShouldBe(expectedContract.ProbationPercentage);
        //        contract.Code.ShouldBe(expectedContract.Code);
        //        contract.JobPositionId.ShouldBe(expectedContract.JobPositionId);
        //        contract.SalaryRequestEmployeeId.ShouldBe(expectedContract.SalaryRequestEmployeeId);
        //        contract.EmployeeId.ShouldBe(expectedContract.EmployeeId);
        //        contract.Note.ShouldBe(expectedContract.Note);
        //    });
        //}

        [Fact]
        public void GetCheckpointTemplate_Test1()
        {
            var requestId = 1635;

            var expectedResult = new
            {
                CheckpointName = "checkpoint",
                EmployeeFullName = "Phạm Thiên An",
                ApplyDate = "01/12/2022",
                OldLevel = "fresher-",
                NewLevel = "fresher-",
                OldSalary = "6,000,000",
                NewSalary = "7,000,000"
            };

            var expectedMailTemplate = new MailPreviewInfoDto
            {
                TemplateId = 425,
                SendToEmail = "an.phamthien@ncc.asia",
                Subject = "[NCC][Phạm Thiên An] checkpoint ",
                MailFuncType = MailFuncEnum.Checkpoint,
                CCs = new List<string>(),
                BodyMessage = $"<div style='background:rgba(0,0,0,.15);min-height:800px;padding:40px;font-family:Inter,sans-serif'><div style='width:900px;margin:auto;background:#fff;padding:20px 60px;border-radius:15px'><div style='display:flex;justify-content:space-between'><div style='color:#d86867;margin-top:30px;margin-bottom:30px;font-size:24pt;font-weight:700'>{expectedResult.CheckpointName}</div><img src='https://do78x13wq0td.cloudfront.net/prod/avatars/host/logo-ncc.png' alt='' width='194' height='78' style='margin-top:30px'></div><div style='margin:auto;padding-top:20px;padding-bottom:20px;font-size:14pt'><div>Chào anh/chị<strong>&nbsp;{expectedResult.EmployeeFullName}</strong>,</div><div style='margin-top:20px;text-align:justify'>Trước tiên, thay mặt Ban Giám đốc, bộ phận Nhân sự cảm ơn những nỗ lực quý báu của bạn trong thời gian vừa qua, NCC tự hào vì có những nhân viên tận tụy, giỏi giang như bạn. Bộ phận nhân sự xin thông báo kết quả Checkpoint của anh/chị và sẽ được áp dụng từ ngày<strong>&nbsp;{expectedResult.ApplyDate}&nbsp;</strong>như sau:</div><div style='text-align:center;border-width:1px;border-color:#000;margin-top:20px'><table style='border-collapse:collapse;width:100%;border-width:0' border='0'><thead style='background-color:#000;color:#fff;border-width:0'><tr style='height:50px;border-width:0;text-align:center'><td style='height:50px;border-width:0;width:30%;text-align:center'><span style='font-size:14pt'><strong>Họ và tên</strong></span></td><td style='height:50px;border-width:0;width:18%;text-align:center'><span style='font-size:14pt'><strong>Level hiện tại</strong></span></td><td style='height:50px;border-width:0;width:14%;text-align:center'><span style='font-size:14pt'><strong>Level mới</strong></span></td><td style='height:50px;border-width:0;width:20%;text-align:center'><span style='font-size:14pt'><strong>Lương hiện tại</strong></span></td><td style='height:50px;border-width:0;width:18%;text-align:center'><span style='font-size:14pt'><strong>Lương mới</strong></span></td></tr></thead><tbody><tr><td style='height:50px;border-width:0;width:30%;text-align:center'><span style='font-size:14pt'><strong>{expectedResult.EmployeeFullName}</strong></span></td><td style='height:50px;border-width:0;width:18%;text-align:center'><p style='font-size:14pt;margin-top:15px'><strong>{expectedResult.OldLevel}</strong></p><td style='height:50px;border-width:0;width:14%;text-align:center'><p style='font-size:14pt;margin-top:15px'><strong>{expectedResult.NewLevel}</strong></p></td><td style='height:50px;border-width:0;width:20%;text-align:center'><p style='font-size:14pt;margin-top:15px'><strong>{expectedResult.OldSalary}</strong></p><td style='height:50px;border-width:0;width:18%;text-align:center'><p style='font-size:14pt;margin-top:15px'><strong>{expectedResult.NewSalary}</strong></p></td></tr></tbody></table></div><div style='margin-top:20px;text-align:justify'>Chúc bạn sức khỏe, hạnh phúc và tiếp tục tỏa sáng trong công việc. Hãy cùng nhau đoàn kết và cùng hướng về một mục tiêu chung đó là xây dựng đại gia đình NCC thành công và vững mạnh hơn nữa nhé!</div><div style='margin-top:20px'>Trân trọng!</div></div><h6 style='text-align:center;color:#000;margin-top:30px;margin-bottom:50px'>2022 © NCC Vietnam. All rights reserved.</h6></div></div>"
            };

            WithUnitOfWork(() =>
            {
                var result = _salaryRequestManager.GetCheckpointTemplate(requestId);
                
                result.ShouldNotBeNull();
                result.TemplateId.ShouldBe(expectedMailTemplate.TemplateId);
                result.BodyMessage.ShouldBe(expectedMailTemplate.BodyMessage);
                result.SendToEmail.ShouldBe(expectedMailTemplate.SendToEmail);
                result.Subject.ShouldBe(expectedMailTemplate.Subject);
                result.MailFuncType.ShouldBe(expectedMailTemplate.MailFuncType);
                result.CCs.ShouldBe(expectedMailTemplate.CCs);
            });
        }

        [Fact]
        public void SendMailToOneEmployee_Test1()
        {
            // Non-existent SalaryChangeRequestEmployee
            var input = new SendMailCheckpointDto
            {
                requestId = 1,
            };
            var expectedMessage = $"Can not found salary change request employee with Id = {input.requestId}";

            WithUnitOfWork(() =>
            {
                var exception = Assert.Throws<UserFriendlyException>(() =>
                {
                    _salaryRequestManager.SendMailToOneEmployee(input);
                });

                exception.Message.ShouldBe(expectedMessage);
            });
        }

        [Fact]
        public void SendMailToAllEmployee_Test1()
        {
            var requestId = 143;
            var input = new InputGetEmployeeInSalaryRequestDto
            {
                GridParam = new GridParam
                {
                    MaxResultCount = 10,
                },
                ToLevelIds = new List<long> { 315 },
                ToUsertypes = new List<UserType> { UserType.Staff },
                ToJobPositionIds = new List<long> { 48 },
                BranchIds = new List<long> { 94 }
            };

            var expectedMessage = "Started sending 1 email.";

            WithUnitOfWork(() =>
            {
                var result = _salaryRequestManager.SendMailToAllEmployee(requestId, input);

                result.ShouldBe(expectedMessage);
            });
        }

        //[Fact]
        //public async Task GetTemplateToImportCheckpoint_Test1()
        //{
        //    await WithUnitOfWorkAsync(async () =>
        //    {
        //        var result = await _salaryRequestManager.GetTemplateToImportCheckpoint();

        //        result.ShouldNotBeNull();
        //        result.FileName.ShouldBe("ImportCheckpoint");
        //    });
        //}
    }
}
