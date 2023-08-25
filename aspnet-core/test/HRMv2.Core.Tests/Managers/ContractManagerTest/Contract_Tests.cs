using Abp.Application.Editions;
using Abp.Application.Features;
using Abp.Domain.Entities;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.MultiTenancy;
using Abp.Net.Mail;
using Abp.ObjectMapping;
using Abp.Runtime.Session;
using Abp.UI;
using AutoMapper;
using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Wordprocessing;
using HRMv2.Core.Tests;
using HRMv2.Editions;
using HRMv2.Entities;
using HRMv2.Manager.Categories;
using HRMv2.Manager.Categories.JobPositions.Dto;
using HRMv2.Manager.EmployeeContracts;
using HRMv2.Manager.EmployeeContracts.Dto;
using HRMv2.Manager.Notifications.Email;
using HRMv2.Manager.Notifications.Email.Dto;
using HRMv2.Manager.Salaries.Dto;
using HRMv2.Manager.SalaryRequests.Dto;
using HRMv2.MultiTenancy;
using HRMv2.NccCore;
using HRMv2.UploadFileServices;
using HRMv2.WebServices.Timesheet;
using HRMv2.WebServices.Timesheet.Dto;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestPlatform.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting.Logging;
using Moq;
using NccCore.Paging;
using NccCore.Uitls;
using Newtonsoft.Json;
using NPOI.HPSF;
using NSubstitute;
using Shouldly;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using static HRMv2.Constants.Enum.HRMEnum;
using IObjectMapper = Abp.ObjectMapping.IObjectMapper;

namespace HRMv2.Application.Tests.APIs.ContractManagerTest
{
    public class Contract_Tests : HRMv2CoreTestBase
    {
        private readonly ContractManager _Contract;
        //private readonly UploadFileService _UploadFileService;
        //public readonly EmailManager _EmailManager;

        private readonly IWorkScope _work;

        public Contract_Tests()
        {
            _work = Resolve<IWorkScope>();


            var _iAbpSession = Resolve<IAbpSession>();

            var _edition = Resolve<IRepository<Edition>>();
            var _featureValueStore = Resolve<IAbpZeroFeatureValueStore>();
            var _unitOfWorkManager = Resolve<IUnitOfWorkManager>();

            var _editionManager = Substitute.For<EditionManager>(_edition, _featureValueStore, _unitOfWorkManager);

            var _tenant = Resolve<IRepository<Tenant>>();
            var _tenantFeatureSetting = Resolve<IRepository<TenantFeatureSetting, long>>();

            var _tenantManager = Substitute.For<TenantManager>(_tenant, _tenantFeatureSetting, _editionManager, _featureValueStore);

            var _uploadFile = Substitute.For<IUploadFileService>();
            var _uploadFileService = Substitute.For<UploadFileService>(_uploadFile, _tenantManager, _iAbpSession);
            var _emailSender = Resolve<IEmailSender>();
            var _timesheetConfig = Resolve<IOptions<TimesheetConfig>>();

            var _emailManager = Substitute.For<EmailManager>(_work, _emailSender, _timesheetConfig);

            _Contract = new ContractManager(_work, _uploadFileService, _emailManager);

            _Contract.ObjectMapper = Resolve<IObjectMapper>();
        }


        [Fact]
        public async Task GetAllTest()
        {
            await WithUnitOfWorkAsync(() =>
            {
                var Contracts = _Contract.GetAll();
                Assert.Equal(47, Contracts.Count);
                Contracts.ShouldContain(Contract => Contract.Id == 1116);
                Contracts.ShouldContain(Contract => Contract.EmployeeId == 880);
                Contracts.ShouldContain(Contract => Contract.Code == "AN.PHAMTHIEN/10/2022/HĐLĐ-NCC");
                return Task.CompletedTask;
            });

        }

        [Fact]
        //get All Contract Paging with no skip, no take (bi nguoc)
        public async Task GetAllPagingTest1()
        {
            var inputTest = new GridParam();

            await WithUnitOfWorkAsync(() =>
            {

                var Contracts = _Contract.GetAllPaging(inputTest);

                Assert.Equal(10, Contracts.Result.Items.Count);
                Assert.Equal(47, Contracts.Result.TotalCount);
                Assert.NotEqual(Contracts.Result.TotalCount, Contracts.Result.Items.Count);
                Assert.Throws<ArgumentOutOfRangeException>(() => Contracts.Result.Items[10]);

                Contracts.Result.Items.ShouldContain(Contract => Contract.Id == 1237);
                Contracts.Result.Items.ShouldContain(Contract => Contract.EmployeeId == 888);
                Contracts.Result.Items.ShouldContain(Contract => Contract.Code == "TRAN.DANGHUYEN/1/2023/HĐTV-NCC");
                Contracts.Result.Items.ShouldNotContain(Contract => Contract.Id == 1126);
                return Task.CompletedTask;
            });
        }

        [Fact]
        //get All Contract Paging with skip = 2
        public async Task GetAllPagingTest2()
        {
            var inputTest = new GridParam
            {
                SkipCount = 2,
            };


            await WithUnitOfWorkAsync(() =>
            {

                var Contracts = _Contract.GetAllPaging(inputTest);

                Assert.Equal(10, Contracts.Result.Items.Count);
                Assert.Equal(45, Contracts.Result.TotalCount - 2);
                Assert.Equal(1181, Contracts.Result.Items[9].Id);

                Contracts.Result.Items.ShouldContain(Contract => Contract.Id == 1235);
                Contracts.Result.Items.ShouldContain(Contract => Contract.EmployeeId == 888);
                Contracts.Result.Items.ShouldContain(Contract => Contract.Code == "TRAN.DANGHUYEN/1/2023/HĐTV-NCC");
                Contracts.Result.Items.ShouldNotContain(Contract => Contract.Id == 1236);
                return Task.CompletedTask;
            });
        }

        [Fact]
        //get All Contract Paging with take = 2
        public async Task GetAllPagingTest3()
        {
            var inputTest = new GridParam
            {
                MaxResultCount = 2,
            };


            await WithUnitOfWorkAsync(() =>
            {

                var Contracts = _Contract.GetAllPaging(inputTest);

                Assert.Equal(2, Contracts.Result.Items.Count);

                Contracts.Result.Items.ShouldContain(Contract => Contract.Id == 1236);
                Contracts.Result.Items.ShouldContain(Contract => Contract.EmployeeId == 885);
                Contracts.Result.Items.ShouldContain(Contract => Contract.Code == "KHOA.VOTIEN/2/2023/HĐLĐ-NCC");
                Contracts.Result.Items.ShouldNotContain(Contract => Contract.Id == 1235);

                return Task.CompletedTask;
            });
        }

        [Fact]
        //get All Contract Paging with skip = 5, take = 7
        public async Task GetAllPagingTest4()
        {
            var inputTest = new GridParam
            {
                SkipCount = 5,
                MaxResultCount = 7,
            };


            await WithUnitOfWorkAsync(() =>
            {

                var Contracts = _Contract.GetAllPaging(inputTest);

                Assert.Equal(7, Contracts.Result.Items.Count);
                Assert.Equal(1232, Contracts.Result.Items[0].Id);

                Contracts.Result.Items.ShouldContain(Contract => Contract.Id == 1232);
                Contracts.Result.Items.ShouldContain(Contract => Contract.EmployeeId == 889);
                Contracts.Result.Items.ShouldContain(Contract => Contract.Code == "PHUC.LEHOANG/1/2023/HĐĐT-NCC");
                Contracts.Result.Items.ShouldNotContain(Contract => Contract.Id == 1180);

                return Task.CompletedTask;
            });
        }

        [Fact]
        //get All Contract Paging with skip >47 (max = 47)
        public async Task GetAllPagingTest5()
        {
            var inputTest = new GridParam
            {
                SkipCount = 48,
            };

            await WithUnitOfWorkAsync(() =>
            {
                var Contracts = _Contract.GetAllPaging(inputTest);

                Assert.Equal(0, Contracts.Result.Items.Count);
                return Task.CompletedTask;
            });
        }

        [Fact]
        //get lastest contract of employee
        public async Task GetLastestContractOfEmployeeTest1()
        {
            var employeeId = 893;
            await WithUnitOfWorkAsync(() =>
            {
                var Contracts = _Contract.GetLastestContractOfEmployee(employeeId);

                Assert.NotNull(Contracts);
                Assert.Equal("TU.TRANDIEM/10/2022/HĐLĐ-NCC", Contracts.Code);
                Assert.Equal(1129, Contracts.Id);

                return Task.CompletedTask;
            });
        }
        [Fact]
        //get lastest contract of employee(null)
        public async Task GetLastestContractOfEmployeeTest2()
        {
            var employeeId = 5000;
            await WithUnitOfWorkAsync(() =>
            {
                var Contracts = _Contract.GetLastestContractOfEmployee(employeeId);

                Assert.Null(Contracts);
                return Task.CompletedTask;
            });
        }

        [Fact]
        //get lastest contract by salary request
        public async Task GetContractBySalaryRequestTest1()
        {
            var requestEmployeeId = 1611;
            await WithUnitOfWorkAsync(() =>
            {
                var Contracts = _Contract.GetContractBySalaryRequest(requestEmployeeId);

                Assert.NotNull(Contracts);
                Assert.Equal("PHUC.LEHOANG/12/2022/HĐĐT-NCC", Contracts.Code);
                Assert.Equal(889, Contracts.EmployeeId);
                return Task.CompletedTask;
            });
        }
        [Fact]
        //get lastest contract by salary request(null)
        public async Task GetContractBySalaryRequestTest2()
        {
            var requestEmployeeId = 5;
            await WithUnitOfWorkAsync(() =>
            {
                var Contracts = _Contract.GetContractBySalaryRequest(requestEmployeeId);

                Assert.Null(Contracts);
                return Task.CompletedTask;
            });
        }

        [Fact]
        //generate Contract code
        public async Task GenerateContractCodeTest1()
        {
            await WithUnitOfWorkAsync(() =>
            {
                GenerateContractCodeDto ContractCode = new()
                {
                    EmployeeId = 885,
                    UserType = UserType.Staff,
                    JobPositionId = 50,
                    Year = 2021,
                    Month = 11,
                };
                var Result = _Contract.GenerateContractCode(ContractCode);

                Assert.NotNull(Result);
                Assert.Equal("KHOA.VOTIEN/11/2021/HĐLĐ-NCC", Result);
                return Task.CompletedTask;
            });
        }

        [Fact]
        //generate Contract code (null employeeId)
        public async Task GenerateContractCodeTest2()
        {
            await WithUnitOfWorkAsync(async () =>
            {
                GenerateContractCodeDto ContractCode = new()
                {
                    EmployeeId = 15,
                    UserType = UserType.Staff,
                    JobPositionId = 5,
                    Year = 2021,
                    Month = 11,
                };
                await Assert.ThrowsAsync<NullReferenceException>(() => {
                    _Contract.GenerateContractCode(ContractCode);

                    return Task.CompletedTask;
                });
            });
        }

        [Fact]
        //Update note 
        public async Task UpdateNoteTest1()
        {
            UpdateContractNoteDto UpdateNote = new()
            {
                ContractId = 1120,
                Note = "this is an updated note",
            };

            await WithUnitOfWorkAsync(() =>
            {
                var NoteUpdated = _Contract.UpdateNote(UpdateNote);

                Assert.NotNull(NoteUpdated);

                return Task.FromResult(Task.CompletedTask);
            });

            await WithUnitOfWorkAsync(async () =>
            {
                var UpdatedNote = await _work.GetAsync<EmployeeContract>(1120);

                UpdatedNote.Note.ShouldBe(UpdateNote.Note);
            });
        }

        [Fact]
        //Update note (null Contract Id)
        public async Task UpdateNoteTest2()
        {
            UpdateContractNoteDto UpdateNote = new()
            {
                ContractId = 10,
                Note = "this is an updated note",
            };

            await WithUnitOfWorkAsync(async () =>
            {
                await Assert.ThrowsAsync<NullReferenceException>(async () => {
                    await _Contract.UpdateNote(UpdateNote);
                });
                return Task.CompletedTask;
            });
        }

        [Fact]
        //Update Contract
        public async Task UpdateTest1()
        {
            AddOrUpdateEmployeeRequestDto UpdateData = new()
            {
                ToLevelId = 317,
                ToSalary = 7000000,
                ToJobPositionId = 47,
                ToUserType = UserType.Staff,
                ContractEndDate = new DateTime(2023, 04, 11),
                ProbationPercentage = 50,
                ApplyDate = new DateTime(2023, 01, 25),
                Id = 1603,
                ContractCode = "new code",
                BasicSalary = 6000000,
            };

            await WithUnitOfWorkAsync(() =>
            {


                var ContractNeedToUpdate = _work.GetAll<EmployeeContract>()
                .Where(x => x.Id == 1116)
                .FirstOrDefault();
                var Result = _Contract.Update(UpdateData, ContractNeedToUpdate);

                Assert.NotNull(Result);

                return Task.FromResult(Task.CompletedTask);
            });


            await WithUnitOfWorkAsync(async () =>
            {
                var UpdatedContract = await _work.GetAsync<EmployeeContract>(1116);

                Assert.Equal(UpdateData.ApplyDate, UpdatedContract.StartDate);
                Assert.Equal(UpdateData.ToUserType, UpdatedContract.UserType);
                Assert.Equal(UpdateData.ContractEndDate, UpdatedContract.EndDate);
                Assert.Equal(UpdateData.ToLevelId, UpdatedContract.LevelId);
                Assert.Equal(UpdateData.ToSalary, UpdatedContract.RealSalary);
                Assert.Equal(UpdateData.ToJobPositionId, UpdatedContract.JobPositionId);
                Assert.Equal(UpdateData.ProbationPercentage, UpdatedContract.ProbationPercentage);
                Assert.Equal(UpdateData.Id, UpdatedContract.SalaryRequestEmployeeId);
                Assert.Equal(UpdateData.BasicSalary, UpdatedContract.BasicSalary);

                UpdatedContract.Code.ShouldBe(UpdateData.ContractCode);
            });
        }

        [Fact]
        //Update Contract (current contract null)
        public async Task UpdateTest2()
        {


            await WithUnitOfWorkAsync(async () =>
            {
                var ContractNeedToUpdate = _work.GetAll<EmployeeContract>()
                .Where(x => x.Id == 20)
                .FirstOrDefault();

                AddOrUpdateEmployeeRequestDto UpdateData = new()
                {
                    LevelId = 317,
                    ToSalary = 7000000,
                    JobPositionId = 47,
                    ToUserType = UserType.Staff,
                    ContractEndDate = new DateTime(2023, 04, 11),
                    ProbationPercentage = 50,
                    ApplyDate = new DateTime(2023, 01, 25),
                    Id = 1603,
                    ContractCode = "new code",
                    BasicSalary = 6000000,
                };

                await Assert.ThrowsAsync<NullReferenceException>(async () => {
                    await _Contract.Update(UpdateData, ContractNeedToUpdate);
                });
                return Task.CompletedTask;
            });
        }

        [Fact]
        //Update Contract
        public async Task UpdateContracEndDateTest1()
        {
            int employeeId = 903;
            var newEndDate = new DateTime(2024, 01, 15, 0,0,0);
            await WithUnitOfWorkAsync(() =>
            {

                _Contract.UpdateContracEndDate(employeeId, newEndDate);

                return Task.FromResult(Task.CompletedTask);
            });

            await WithUnitOfWorkAsync(() => {
                var UpdatedContract = _work.GetAsync<EmployeeContract>(1178);

                //UpdatedContract.EndDate.ShouldBe(newEndDate);
                Assert.Equal(UpdatedContract.Result.EndDate, newEndDate);
                return Task.CompletedTask;
            });
        }

        [Fact]
        //Update Contract (null employeeId)
        public async Task UpdateContracEndDateTest2()
        {
            await WithUnitOfWorkAsync(() =>
            {
                int employeeId = 8900;
                DateTime newEndDate = new DateTime(2024, 01, 15);

                Assert.ThrowsAsync<NullReferenceException>(() =>
                {
                    _Contract.UpdateContracEndDate(employeeId, newEndDate);
                    return Task.CompletedTask;
                });

                return Task.FromResult(Task.CompletedTask);
            });
        }

        [Fact]
        //Delete Contract
        public async Task DeleteContractTest1()
        {
            await WithUnitOfWorkAsync(() =>
            {
                int Id = 1125;

                var deletedContract = _Contract.DeleteContract(Id);

                var CheckSalaryRequest = _work.GetAll<SalaryChangeRequestEmployee>()
                .Where(x => x.EmployeeId == 889)
                .FirstOrDefault();
                Assert.False(CheckSalaryRequest.HasContract);

                return Task.FromResult(Task.CompletedTask);
            });

            await WithUnitOfWorkAsync(async () =>
            {

                var getAllContractCount = _work.GetAll<EmployeeContract>().Count();

                Assert.Equal(46, getAllContractCount);

                await Assert.ThrowsAsync<EntityNotFoundException>(async () =>
                {
                    await _work.GetAsync<EmployeeContract>(1125);
                });




                return Task.FromResult(Task.CompletedTask);
            });
        }

        [Fact]
        //Delete Contract with UserFriendlyException
        public async Task DeleteContractTest2()
        {
            await WithUnitOfWorkAsync(async () =>
            {
                int Id = 30;

                var exception = await Assert.ThrowsAsync<UserFriendlyException>(async () =>
                {
                    await _Contract.DeleteContract(Id);
                });
                Assert.Equal($"not found contract with id {Id}", exception.Message);

                return Task.FromResult(Task.CompletedTask);
            });
        }

        [Fact]
        //Get Contract Template 
        public async Task GetContractTemplateTest1()
        {
            await WithUnitOfWorkAsync(() =>
            {
                long ContractId = 57373;

                var type = MailFuncEnum.Payslip;

                var template = _Contract.GetContractTemplate(ContractId, type);

                Assert.NotNull(template);

                Assert.Equal("thong.nguyenba@ncc.asia", template.SendToEmail);
                Assert.Empty(template.CCs);
                Assert.NotNull(template.BodyMessage);

                Assert.Equal(MailFuncEnum.Payslip, template.MailFuncType);
                Assert.Equal("[NCC][Nguyễn Bá Thông] THÔNG BÁO CHI TIẾT LƯƠNG THÁNG 2/2023", template.Subject);
                Assert.Equal(417, template.TemplateId);

                return Task.FromResult(Task.CompletedTask);
            });
        }

        [Fact]
        //Get Contract Template (null ContractId)
        public async Task GetContractTemplateTest2()
        {
            await WithUnitOfWorkAsync(() =>
            {
                long ContractId = 11350;

                var type = MailFuncEnum.Payslip;

                Assert.ThrowsAsync<NullReferenceException>(() =>
                {
                    _Contract.GetContractTemplate(ContractId, type);
                    return Task.CompletedTask;
                });


                return Task.FromResult(Task.CompletedTask);
            });
        }

        [Fact]
        //Create Contract By Salary Request 
        public async Task CreateContractBySalaryRequestTest1()
        {
            AddOrUpdateEmployeeRequestDto EmployeeRequest = new()
            {
                EmployeeId = 890,
                ToLevelId = 2,
                ContractEndDate = new DateTime(2023, 02, 12),
                ApplyDate = new DateTime(2022, 12, 28),
                LevelId = 317,
                ToSalary = 7000000,
                ToJobPositionId = 47,
                ToUserType = UserType.Staff,
                ProbationPercentage = 50,
                Id = 1603,
                ContractCode = "new code",
                BasicSalary = 6000000,
            };

            await WithUnitOfWorkAsync(() =>
            {

                var Result = _Contract.CreateContractBySalaryRequest(EmployeeRequest);

                Assert.NotNull(Result);

                return Task.FromResult(Task.CompletedTask);
            });

            await WithUnitOfWorkAsync(async () =>
            {
                var allContract = _work.GetAll<EmployeeContract>();
                var newContract = await _work.GetAsync<EmployeeContract>(1238);

                allContract.Count().ShouldBe(48);
                newContract.Id.ShouldBe(1238);
                newContract.EmployeeId.ShouldBe(EmployeeRequest.EmployeeId);
                allContract.Where(x => x.Id == 1238).ShouldNotBeNull();
            });

        }

        [Fact]
        //Create Contract By Salary Request (null EmployeeId)
        public async Task CreateContractBySalaryRequestTest2()
        {
            AddOrUpdateEmployeeRequestDto EmployeeRequest = new()
            {
                EmployeeId = 8900,
                BasicSalary = 6000000,
                ToSalary = 7000000,
                ToJobPositionId = 47,
                ToLevelId = 2,
                ToUserType = UserType.Staff,
                ProbationPercentage = 50,
                ContractEndDate = new DateTime(2023, 02, 12),
                ApplyDate = new DateTime(2022, 12, 28),
                LevelId = 317,
                Id = 16030,
            };
            await WithUnitOfWorkAsync(async () =>
            {
                await Assert.ThrowsAsync<NullReferenceException>(() => {
                    var result = _Contract.CreateContractBySalaryRequest(EmployeeRequest);
                    return Task.CompletedTask;
                });

                return Task.FromResult(Task.CompletedTask);
            });
        }

        [Fact]
        //Update Employee Contract
        public async Task UpdateEmployeeContractTest1()
        {
            UpdateContractDto UpdateData = new()
            {
                StartDate = new DateTime(2023, 01, 25),
                EndDate = new DateTime(2023, 04, 11),
                LevelId = 317,
                RealSalary = 7000000,
                JobPositionId = 47,
                UserType = UserType.Collaborators,
                ProbationPercentage = 75,
                Id = 1140,
                Code = "new code",
                BasicSalary = 6000000,
            };

            await WithUnitOfWorkAsync(() =>
            {

                var EmployeeContractNeedToUpdate = _work.GetAll<EmployeeContract>()
                .Where(x => x.Id == 1140)
                .FirstOrDefault();

                var Result = _Contract.UpdateEmployeeContract(UpdateData);


                Assert.NotNull(Result);

                return Task.FromResult(Task.CompletedTask);
            });

            await WithUnitOfWorkAsync(() => {
                var UpdatedContract = _work.GetAll<EmployeeContract>()
                    .Where(x => x.Id == UpdateData.Id)
                    .FirstOrDefault();

                UpdatedContract.Code.ShouldBe(UpdateData.Code);
                UpdatedContract.StartDate.ShouldBe(UpdateData.StartDate);
                UpdatedContract.EndDate.ShouldBe(UpdateData.EndDate);
                UpdatedContract.LevelId.ShouldBe(UpdateData.LevelId);
                UpdatedContract.RealSalary.ShouldBe(UpdateData.RealSalary);
                UpdatedContract.JobPositionId.ShouldBe(UpdateData.JobPositionId);
                UpdatedContract.UserType.ShouldBe(UpdateData.UserType);
                UpdatedContract.ProbationPercentage.ShouldBe(UpdateData.ProbationPercentage);
                UpdatedContract.Id.ShouldBe(UpdateData.Id);
                UpdatedContract.BasicSalary.ShouldBe(UpdateData.BasicSalary);
                return Task.CompletedTask;
            });
        }

        [Fact]
        [DefaultValue("true")]
        //Update Employee Contract(Id null)
        public async Task UpdateEmployeeContractTest2()
        {
            await WithUnitOfWorkAsync(async () =>
            {
                UpdateContractDto UpdateData = new()
                {
                    StartDate = new DateTime(2023, 01, 25),
                    EndDate = new DateTime(2023, 04, 11),
                    LevelId = 317,
                    RealSalary = 7000000,
                    JobPositionId = 47,
                    UserType = UserType.Collaborators,
                    ProbationPercentage = 75,
                    Id = 0,
                    Code = "new code",
                    BasicSalary = 6000000,
                };

                var exception = await Assert.ThrowsAsync<EntityNotFoundException>(async () =>
                {
                    await _Contract.UpdateEmployeeContract(UpdateData);
                });
                //Assert.Equal($"Can't found employee contract with Id = {UpdateData.Id}", exception.Message);

                return Task.FromResult(Task.CompletedTask);
            });
        }

        [Fact]
        //Upload Contract File
        public async Task UploadContractFileTest1()
        {
            await WithUnitOfWorkAsync(() =>
            {
                var content = "Hello World from a Fake File";
                var fileName = "test.txt";
                var stream = new MemoryStream();
                var writer = new StreamWriter(stream);
                writer.Write(content);
                writer.Flush();
                stream.Position = 0;
                IFormFile file = new FormFile(stream, 0, stream.Length, "id_from_form", fileName);

                /*var bytes = Encoding.UTF8.GetBytes("This is a dummy file");
                IFormFile file = new FormFile(new MemoryStream(bytes), 0, bytes.Length, "Data", "dummy.txt");*/

                ContractFileDto ContractFiles = new()
                {
                    ContractId = 1116,
                    File = file,
                };

                var Results = _Contract.UploadContractFile(ContractFiles);

                Assert.NotNull(Results);

                return Task.FromResult(Task.CompletedTask);
            });

            await WithUnitOfWorkAsync(() => {
                var Contracts = _work.GetAll<EmployeeContract>()
                   .Where(x => x.Id == 1116)
                   .FirstOrDefault();

                Contracts.File.ShouldNotBeNull();
                return Task.CompletedTask;
            });
        }

        [Fact]
        //Upload Contract File (EmployeeContract = default) (chua bat dc employeeContract == default)
        public async Task UploadContractFileTest2()
        {
            await WithUnitOfWorkAsync(async () =>
            {

                var content = "Hello World from a Fake File";
                var fileName = "test.txt";
                var stream = new MemoryStream();
                var writer = new StreamWriter(stream);
                writer.Write(content);
                writer.Flush();
                stream.Position = 0;
                IFormFile file = new FormFile(stream, 0, stream.Length, "id_from_form", fileName);

                ContractFileDto ContractFiles = new()
                {
                    ContractId = default,
                    File = file,
                };

                var exception = await Assert.ThrowsAsync<EntityNotFoundException>(async () =>
                {
                    await _Contract.UploadContractFile(ContractFiles);
                });
                //Assert.Equal($"ContractId {ContractFiles.ContractId} is NOT exist", exception.Message);


                return Task.FromResult(Task.CompletedTask);
            });
        }

        [Fact]
        //Upload Contract File (file = null)
        public async Task UploadContractFileTest3()
        {
            await WithUnitOfWorkAsync(async () =>
            {
                ContractFileDto ContractFiles = new()
                {
                    ContractId = 1116,
                    File = null,
                };

                var exception = await Assert.ThrowsAsync<UserFriendlyException>(async () =>
                {
                    await _Contract.UploadContractFile(ContractFiles);
                });
                Assert.Equal("No file upload", exception.Message);


                return Task.FromResult(Task.CompletedTask);
            });
        }

        [Fact]
        //Delete Contract File
        public async Task DeleteContractFileTest1()
        {
            await WithUnitOfWorkAsync(() =>
            {
                var Ids = 1116;

                var Result = _Contract.DeleteContractFile(Ids);

                Assert.NotNull(Result);

                return Task.FromResult(Task.CompletedTask);
            });
            await WithUnitOfWorkAsync(() =>
            {
                var Contracts = _work.GetAll<EmployeeContract>()
                .Where(x => x.Id == 1116)
                .FirstOrDefault();

                Contracts.File.ShouldBeNull();

                return Task.FromResult(Task.CompletedTask);
            });
        }

        [Fact]
        //Delete Contract File (employee contract = default) (chua bat duoc truong hop contract == default)
        public async Task DeleteContractFileTest2()
        {
            await WithUnitOfWorkAsync(async () =>
            {
                long Ids = default;

                var exception = await Assert.ThrowsAsync<EntityNotFoundException>(async () =>
                {
                    await _Contract.DeleteContractFile(Ids);
                });
                //Assert.Equal($"ContractId {Ids} is NOT exist", exception.Message);

                return Task.FromResult(Task.CompletedTask);
            });
        }

    }
}
