using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abp.Dependency;
using Abp.Runtime.Session;
using Abp.UI;
using HRMv2.Entities;
using HRMv2.Manager.Timesheet;
using HRMv2.Manager.Timesheet.Dto;
using HRMv2.NccCore;
using HRMv2.WebServices.Timesheet;
using Xunit;
using Moq;
using NccCore.Uitls;
using static HRMv2.Constants.Enum.HRMEnum;
using HRMv2.Core.Tests;
using Abp.Domain.Uow;
using static HRMv2.Manager.Timesheet.Dto.InpuReviewInternFromTSDto;
using NccCore.Extension;
using Shouldly;
using DocumentFormat.OpenXml.Spreadsheet;
using HRMv2.Manager.Salaries.Dto;
using NPOI.SS.Formula.Functions;

namespace HRMv2.Application.Tests.APIs.TimesheetManagerTest
{
    public class TimesheetManager_Tests : HRMv2CoreTestBase
    {
        private readonly TimesheetManager _timesheetManager;
        private readonly IWorkScope _work;
        public TimesheetManager_Tests()
        {
            var moqHttpClient = Resolve<HttpClient>();
            var moqAbpSession = Resolve<IAbpSession>();
            var moqIocResolver = Resolve<IIocResolver>();
            var moqTimesheetWebService = new Mock<TimesheetWebService>(moqHttpClient, moqAbpSession, moqIocResolver);
            //moqTimesheetWebService.Setup(p => p.GetSettingOffDates(2022, 11))
            //                    .Returns(value: null);
            //moqTimesheetWebService.Setup(p => p.GetSettingOffDates(2022, 12))
            //                   .Returns(new HashSet<DateTime>());
            _work = Resolve<IWorkScope>();

            _timesheetManager = new TimesheetManager(moqTimesheetWebService.Object, _work);

            _timesheetManager.UnitOfWorkManager = Resolve<IUnitOfWorkManager>();
            
        }

        // Test Function UpdateAvatarFromTimesheet
        [Fact]
        public async Task Should_Allow_Update_Valid_Avatar()
        {
            var input = new AvatarDto
            {
                AvatarPath = "/path/img.jpg",
                EmailAddress = "an.phamthien@ncc.asia"
            };
            await WithUnitOfWorkAsync(async () =>
            {
                _timesheetManager.UpdateAvatarFromTimesheet(input);
            });
            await WithUnitOfWorkAsync(async () =>
            {
                var employee = _work.GetAll<Employee>()
                    .Where(x => x.Email.ToLower().Trim() == input.EmailAddress.ToLower().Trim())
                    .FirstOrDefault();
                Assert.Equal(input.AvatarPath, employee.Avatar);
            });
        }

        //Test Function GetCompanyWorkingDay
        //[Fact]
        //public void Should_Exception_With_Month_Year_Invalid()
        //{
        //    var inputMonth = 11;
        //    var inputYear = 2022;
        //    var expectedMsg = "Can not collect dayoff from timesheet";
        //    WithUnitOfWork(async () =>
        //    {
        //        var exception = await Assert.ThrowsAsync<UserFriendlyException>(async () =>
        //        {
        //            _timesheetManager.GetCompayWorkingDay(inputYear, inputMonth);
        //        });
        //        Assert.Equal(expectedMsg, exception.Message);
        //    });
        //}

        //[Fact]
        //public async Task Should_Allow_Get_Company_Working_Day_With_Month_Year_Valid()
        //{
        //    await WithUnitOfWorkAsync(async () =>
        //    {
        //        var expectedResult = 22;
        //        var result = _timesheetManager.GetCompayWorkingDay(2022, 12);
        //       Assert.Equal(expectedResult.ToString(), result.ToString());
        //    });
        //}

        //Test ComplainPayslipMail
        [Fact]
        public void Should_Not_Allow_Complain_With_Payslip_Not_Found()
        {
            var input = new InputcomplainPayslipDto
            {
                Email = "an.phamthien@ncc.asia",
                ComplainNote = "NOTHING",
                PayslipId = 1
            };
            var expectedResult = "Không tìm thấy phiếu lương, vui lòng kiểm tra lại email";
            WithUnitOfWork(async () =>
            {
                var result = await _timesheetManager.ComplainPayslipMail(input);
                Assert.Equal(expectedResult, result);
            });
        }

        [Fact]
        public async Task Should_Allow_Complain_Payslip_Mail_With_Information_Valid()
        {
            var input = new InputcomplainPayslipDto
            {
                Email = "an.phamthien@ncc.asia",
                ComplainNote = "NOTHING",
                PayslipId = 57333
            };
            var expectedResult = "Khiếu nại của bạn đã được gửi đi, hãy đợi kết quả từ HR nhé";
            var expectedUpdateResult = input.ComplainNote + " " + PayslipConfirmStatus.ConfirmWrong;
            await WithUnitOfWorkAsync(async () =>
            {
                var result = await _timesheetManager.ComplainPayslipMail(input);
                var payslip = _work.GetAll<Payslip>()
                    .Where(x => x.Id == input.PayslipId)
                    .Where(x => x.Employee.Email == input.Email)
                    .Where(x => x.Payroll.Status != PayrollStatus.Executed)
                    .OrderByDescending(x => x.CreationTime)
                    .FirstOrDefault();
                Assert.Equal(expectedResult, result);
                Assert.Equal(expectedUpdateResult, $"{payslip.ComplainNote} {payslip.ConfirmStatus}");
            });
        }

        //Test function ConfirmPayslipMail
        [Fact]
        public async Task Should_Not_Allow_Confirm_With_Playslip_Not_Found()
        {
            var input = new InputConfirmPayslipMailDto
            {
                Email = "an.phamthien@ncc.asia",
                PayslipId = 1
            };
            var expectedResult = "Không tìm thấy phiếu lương";
            await WithUnitOfWorkAsync(async () =>
            {
                var result = await _timesheetManager.ConfirmPayslipMail(input);
                Assert.Equal(expectedResult, result);
            });
        }

        [Fact]
        public async Task Should_Not_Allow_Confirm_With_Payslip_Status_Executed()
        {
            var input = new InputConfirmPayslipMailDto
            {
                Email = "an.phamthien@ncc.asia",
                PayslipId = 57250
            };
            var expectedResult = "Đã quá hạn complain";
            await WithUnitOfWorkAsync(async () =>
            {
                var result = await _timesheetManager.ConfirmPayslipMail(input);
                Assert.Equal(expectedResult, result);
            });
        }

        [Fact]
        public async Task Should_Not_Allow_Confirm_With_Email_Not_Equall()
        {
            var input = new InputConfirmPayslipMailDto
            {
                Email = "an.phamthien@ncc.asia",
                PayslipId = 57334
            };
            var expectedResult = "Đây không phải phiếu lương của bạn. Hệ thống đã lưu lại các thông tin để truy vết";
            await WithUnitOfWorkAsync(async () =>
            {
                var result = await _timesheetManager.ConfirmPayslipMail(input);
                Assert.Equal(expectedResult, result);
            });
        }

        [Fact]
        public async Task Should_Allow_Confirm_Payslip_Mail_With_Information_Valid()
        {
            var input = new InputConfirmPayslipMailDto
            {
                Email = "an.phamthien@ncc.asia",
                PayslipId = 57333
            };
            await WithUnitOfWorkAsync(async () =>
            {
                var result = await _timesheetManager.ConfirmPayslipMail(input);
                var payslip = _work.GetAll<Payslip>()
                .Where(x => x.Id == input.PayslipId)
                .Select(s => new { Payslip = s, s.Payroll.Status, s.Payroll.ApplyMonth, s.Employee.Email, s.ConfirmStatus })
                .FirstOrDefault();
                var expectedResult = $"Bạn đã xác nhận phiếu lương {DateTimeUtils.ToMMYYYY(payslip.ApplyMonth)} của {payslip.Email} là chính xác";
                Assert.Equal(expectedResult, result);
            });
            await WithUnitOfWorkAsync(async () =>
            {
                var payslip = _work.GetAll<Payslip>()
                .Where(x => x.Id == input.PayslipId)
                .Select(s => new { Payslip = s, s.Payroll.Status, s.Payroll.ApplyMonth, s.Employee.Email, s.ConfirmStatus })
                .FirstOrDefault();
                Assert.Equal(PayslipConfirmStatus.ConfirmRight, payslip.ConfirmStatus);
            });
        }

        //Test function GetUserInfoByEmail
        [Fact]
        public async Task Should_Not_Allow_Get_User_Info_With_Employee_Not_Found()
        {
            await WithUnitOfWorkAsync(async () => {
                var input = "employeenotexist@ncc.asia";
                var expectedMsg = $"Can't found employee with email = {input}";
                var exception = await Assert.ThrowsAsync<UserFriendlyException>(async () =>
                {
                    _timesheetManager.GetUserInfoByEmail(input);
                });
                Assert.Equal(expectedMsg, exception.Message);
            });
        }

        [Fact]
        public async Task Should_Allow_Get_User_Info_With_Employee_Exist()
        {
            await WithUnitOfWorkAsync(async () =>
            {
                var input = "an.phamthien@ncc.asia";
                var userInfo = _timesheetManager.GetUserInfoByEmail(input);
                var expectedResult = "Phạm Thiên An an.phamthien@ncc.asia";
                Assert.Equal(expectedResult, $"{userInfo.FullName} {userInfo.Email}");
                Assert.Equal("Phạm Thiên An", userInfo.FullName);
                Assert.Equal("an.phamthien@ncc.asia", userInfo.Email);
                Assert.Equal("0956060566", userInfo.Phone);
                Assert.Equal(new DateTime(1995,7,5), userInfo.Birthday);
                Assert.Equal(EmployeeStatus.Working, userInfo.Status);
                Assert.Equal(UserType.Staff, userInfo.UserType);
                Assert.Equal("HN1", userInfo.Branch);
                Assert.Equal("Fresher-", userInfo.Level);
                Assert.Equal("Tester", userInfo.JobPosition);
                Assert.Equal("Techcombank", userInfo.Bank);
                Assert.Equal(1, userInfo.SkillNames.Count());
                Assert.Equal(1, userInfo.Teams.Count());
                Assert.Equal("19000585825", userInfo.BankAccountNumber);
                Assert.Equal("036360254512", userInfo.IdCard);
                Assert.Equal(InsuranceStatus.BHXH, userInfo.InsuranceStatus);
                Assert.Equal("184482", userInfo.TaxCode);
                Assert.Equal("Thường Tín - Hà Nội", userInfo.Address);
                Assert.Equal("Thường Tín - Hà Nội", userInfo.PlaceOfPermanent);
                Assert.Equal("CỤC CẢNH SÁT QUẢN LÝ HÀNH CHÍNH VỀ TRẬT TỰ XÃ HỘI", userInfo.IssuedBy);
                Assert.Equal(new DateTime(2022,7,18), userInfo.IssuedOn);
                Assert.Equal(0, userInfo.RemainLeaveDay);
                Assert.Equal(32, userInfo.BankId);
            });
        }

        //Test function GetInfoToUpdate
        [Fact]
        public async Task Should_Not_Allow_Get_Info_With_Employee_Not_Exist()
        {
            await WithUnitOfWorkAsync(async () =>
            {
                var input = "usernotexist@gmail.com";
                var expectedMsg = $"Can't found employee with email = {input}";
                var exception = await Assert.ThrowsAsync<UserFriendlyException>(async () =>
                {
                    _timesheetManager.GetInfoToUpdate(input);
                });
                Assert.Equal(expectedMsg, exception.Message);
            });
        }

        [Fact]
        public async Task Should_Allow_Get_Info_To_Update_Have_Status_Pending()
        {
            await WithUnitOfWorkAsync(async () =>
            {
                var input = "an.phamthien@ncc.asia";
                var expected = RequestStatus.Pending;
                var result = _timesheetManager.GetInfoToUpdate(input);
                Assert.Equal("0956060566", result.Phone);
                Assert.Equal(new DateTime(1995, 7, 5), result.Birthday);
                Assert.Equal(32, result.BankId);
                Assert.Equal("19000585825", result.BankAccountNumber);
                Assert.Equal("036360254512", result.IdCard);
                Assert.Equal("184482", result.TaxCode);
                Assert.Equal("Thường Tín - Hà Nội", result.Address);
                Assert.Equal("Thường Tín - Hà Nội", result.PlaceOfPermanent);
                Assert.Equal("CỤC CẢNH SÁT QUẢN LÝ HÀNH CHÍNH VỀ TRẬT TỰ XÃ HỘI", result.IssuedBy);
                Assert.Equal(new DateTime(2022, 7, 18), result.IssuedOn);
                Assert.Equal(expected, result.RequestStatus);
            });
        }

        [Fact]
        public async Task Should_Allow_Get_Info_To_Update_Not_Have_Status_Pending()
        {
            await WithUnitOfWorkAsync(async () =>
            {
                var input = "thong.nguyenba@ncc.asia";
                var result = _timesheetManager.GetInfoToUpdate(input);
                var expectedResult = 0;
                Assert.Equal("0965656568", result.Phone);
                Assert.Equal(new DateTime(2000,12,21), result.Birthday);
                Assert.Equal(32, result.BankId);
                Assert.Equal("190079896532", result.BankAccountNumber);
                Assert.Equal("02020220000", result.IdCard);
                Assert.Equal("196522", result.TaxCode);
                Assert.Equal("Yên Hòa-Hưng Yên", result.Address);
                Assert.Equal("Yên Hòa-Hưng Yên", result.PlaceOfPermanent);
                Assert.Equal("CỤC CẢNH SÁT QUẢN LÝ HÀNH CHÍNH VỀ TRẬT TỰ XÃ HỘI", result.IssuedBy);
                Assert.Equal(new DateTime(2022, 10, 11), result.IssuedOn);
                Assert.Equal(expectedResult, (int)result.RequestStatus);
            });
        }


        // Test function GetAllBanks
        [Fact]
        public async Task Should_Get_All_Banks()
        {
            await WithUnitOfWorkAsync(async () =>
            {
                var expectedResult = 8;
                var result = _timesheetManager.GetAllBanks();
                Assert.Equal(expectedResult, result.Count);
                result.ShouldContain(x => x.Id == 32);
                result.ShouldContain(x => x.Name == "Techcombank");

            });
        }

        // Test function CheckExistUser
        [Fact]
        public async Task Should_User_Not_Exist_Check_Exist_User()
        {
            await WithUnitOfWorkAsync(async () => {
                var input = "employeenotexist@gmail.com";
                var expectedMsg = $"Can't found employee with email = {input}";
                var exception = await Assert.ThrowsAsync<UserFriendlyException>(async () =>
                {
                    _timesheetManager.CheckExistUser(input);
                });
                Assert.Equal(expectedMsg, exception.Message);
            });
        }

        //Test function CreateRequestUpdateUserInfo
        [Fact]
        public async Task Should_Not_Allow_Create_With_Employee_Not_Exist()
        {
            var input = new UpdateUserInfoDto
            {
                Email = "an.phamthiennotexist@ncc.asia",
                Phone = "0956060566",
                Birthday = new DateTime(1995, 7, 5),
                IdCard = "036360254512",
                IssuedOn = new DateTime(22, 7, 18),
                PlaceOfPermanent = "Thường Tín - Hà Nội",
                Address = "Thường Tín - Hà Nội",
                BankAccountNumber = "19000585825",
                TaxCode = "184482",
                BankId = 32
            };
            var expcetedresult = new ResultUpdateInfo
            {
                IsSucess = false,
                ResultMessage = $"Can not found employee with email = {input.Email}"
            };
            await WithUnitOfWorkAsync(async () =>
            {
                var result = await _timesheetManager.CreateRequestUpdateUserInfo(input);
                Assert.Equal(expcetedresult.IsSucess, result.IsSucess);
                Assert.Equal(expcetedresult.ResultMessage, result.ResultMessage);
            });
        }

        [Fact]
        public async Task Should_Allow_Update_With_TempEmployeeTS_Exist()
        {
            var input = new UpdateUserInfoDto
            {
                Email = "an.phamthien@ncc.asia",
                Phone = "0956060566",
                Birthday = new DateTime(1995, 7, 5),
                IdCard = "036360254512",
                IssuedOn = new DateTime(22, 7, 18),
                PlaceOfPermanent = "Thanh Xuân - Hà Nội",
                Address = "Thanh Xuân - Hà Nội",
                BankAccountNumber = "19000585825",
                TaxCode = "184482",
                BankId = 32
            };
            var expcetedresult = new ResultUpdateInfo
            {
                IsSucess = true,
                ResultMessage = "Reuqest change info successful"
            };
            await WithUnitOfWorkAsync(async () =>
            {
                var result = await _timesheetManager.CreateRequestUpdateUserInfo(input);

               
                Assert.Equal(expcetedresult.IsSucess, result.IsSucess);
                Assert.Equal(expcetedresult.ResultMessage, result.ResultMessage);
            });
            await WithUnitOfWorkAsync(async () =>
            {
                var employee = _work.GetAll<Employee>()
                   .Where(x => x.Email.ToLower().Trim() == input.Email.ToLower().Trim()).FirstOrDefault();
                var requestUpdate = _work.GetAll<TempEmployeeTS>()
                    .Where(x => x.EmployeeId == employee.Id && x.RequestStatus == RequestStatus.Pending)
                    .OrderByDescending(x => x.CreationTime)
                    .FirstOrDefault();

                Assert.Equal(880, requestUpdate.EmployeeId);
                Assert.Equal(input.Address, requestUpdate.Address);
                Assert.Equal(input.BankId, requestUpdate.BankId);
            });
        }

        [Fact]
        public async Task Should_Allow_Create_With_TempEmployeeTS_Not_Exist()
        {
            var input = new UpdateUserInfoDto
            {
                Email = "thong.nguyenba@ncc.asia",
                Phone = "0956060566",
                Birthday = new DateTime(1995, 7, 5),
                IdCard = "036360254512",
                IssuedOn = new DateTime(22, 7, 18),
                PlaceOfPermanent = "Thanh Xuân - Hà Nội",
                Address = "Thanh Xuân - Hà Nội",
                BankAccountNumber = "19000585825",
                TaxCode = "184482",
                BankId = 33
            };
            var expcetedresult = new ResultUpdateInfo
            {
                IsSucess = true,
                ResultMessage = "Reuqest change info successful"
            };
            await WithUnitOfWorkAsync(async () =>
            {
                var result = await _timesheetManager.CreateRequestUpdateUserInfo(input);
                Assert.Equal(expcetedresult.IsSucess, result.IsSucess);
                Assert.Equal(expcetedresult.ResultMessage, result.ResultMessage);
            });
            await WithUnitOfWorkAsync(async () =>
            {
                var employee = _work.GetAll<Employee>()
                    .Where(x => x.Email.ToLower().Trim() == input.Email.ToLower().Trim()).FirstOrDefault();
                var tempEmployeeTSs = _work.GetAll<TempEmployeeTS>();
                var requestUpdate = _work.GetAll<TempEmployeeTS>()
                    .Where(x => x.EmployeeId == employee.Id && x.RequestStatus == RequestStatus.Pending)
                    .OrderByDescending(x => x.CreationTime)
                    .FirstOrDefault();

                Assert.Equal(23, tempEmployeeTSs.Count());
                Assert.Equal(905, requestUpdate.EmployeeId);
                Assert.Equal(input.Address, requestUpdate.Address);
                Assert.Equal(input.BankId, requestUpdate.BankId);
            });
        }

        //Test function CreateChangeRequestEmployeeAndContract
        //usertype không giống nhau giữa TS và HRM
        [Fact]
        public async Task Should_Allow_Review_With_Salary_Change_Request_Exist()
        {
            var input = new InputCreateRequestHrmv2Dto
            {
                RequestName = "checkpoint",
                ListUserToUpdate = new List<UserToUpdateFromTSDto> {
                        new UserToUpdateFromTSDto()
                        {   //EmployeeId=880
                            //OldSalary=7000000
                            //NewSalary=8000000
                            NewLevel = "6",
                            UserType =  UserType.Staff,
                            IsFullSalary= true,
                            Salary = 8000000,
                            ApplyDate= DateTime.Now,
                            NormalizedEmailAddress="AN.PHAMTHIEN@NCC.ASIA"
                        },
                        new UserToUpdateFromTSDto()
                        {   //EmployeeId=890
                            NewLevel = "6",
                            UserType = UserType.Staff,
                            IsFullSalary= true,
                            Salary = 8000000,
                            ApplyDate= DateTime.Now,
                            NormalizedEmailAddress="TRAN.DANGHUYEN@NCC.ASIA"
                        }
                    },
                Applydate = new DateTime(2022, 12, 10),
                CreatedBy = "Salary Change",
            };
            await WithUnitOfWorkAsync(async () =>
            {
                await _timesheetManager.ReviewInternFromTimesheet(input);

            });
            await WithUnitOfWorkAsync(async () =>
            {
                var existSalaryChangeRequestEmployees = _work.GetAll<SalaryChangeRequestEmployee>();
                var existSalaryChangeRequestEmployee = _work.GetAll<SalaryChangeRequestEmployee>()
                        .Where(x => x.EmployeeId == 880)
                        .OrderByDescending(x => x.CreationTime)
                        .FirstOrDefault();
                var existEmployeeContract = _work.GetAll<EmployeeContract>()
                        .Where(x => x.EmployeeId == 880)
                        .OrderByDescending(x => x.CreationTime)
                        .FirstOrDefault();
                var expectedNewSalary = 8000000;

                Assert.Equal(59, existSalaryChangeRequestEmployees.Count());
                Assert.Equal(143, existSalaryChangeRequestEmployee.SalaryChangeRequestId);

                Assert.Equal(880, existSalaryChangeRequestEmployee.EmployeeId);
                Assert.Equal(new DateTime(2022, 12, 01), existSalaryChangeRequestEmployee.ApplyDate);
                Assert.Equal(143, existSalaryChangeRequestEmployee.SalaryChangeRequestId);
                Assert.Equal(UserType.Staff, existSalaryChangeRequestEmployee.FromUserType);
                Assert.Equal(UserType.Collaborators, existSalaryChangeRequestEmployee.ToUserType);
                Assert.Equal(7000000, existSalaryChangeRequestEmployee.Salary);
                Assert.Equal(8000000, existSalaryChangeRequestEmployee.ToSalary);
                Assert.Equal($"Create from Timesheet By {input.CreatedBy}", existSalaryChangeRequestEmployee.Note);

                Assert.Equal(880, existEmployeeContract.EmployeeId);
                Assert.Equal(new DateTime(2022, 12, 01), existEmployeeContract.StartDate);
                Assert.Equal(existSalaryChangeRequestEmployee.Id, existEmployeeContract.SalaryRequestEmployeeId);
                Assert.Equal(8000000, existEmployeeContract.RealSalary);
                Assert.Equal(UserType.Collaborators, existSalaryChangeRequestEmployee.ToUserType);

            });
        }

        //usertype không giống nhau giữa TS và HRM
        [Fact]
        public async Task Should_Allow_Review_With_New_Salary_Change_Request()
        {
            var input = new InputCreateRequestHrmv2Dto
            {
                //New SalaryChangeRequest
                RequestName = "checkpoint tháng 02/2023",
                ListUserToUpdate = new List<UserToUpdateFromTSDto> {
                        new UserToUpdateFromTSDto()
                        {   //EmployeeId=880
                            //OldSalary=7000000
                            //NewSalary=8000000
                            NewLevel = "6",
                            UserType = UserType.Staff,
                            IsFullSalary= true,
                            Salary = 8000000,
                            ApplyDate= DateTime.Now,
                            NormalizedEmailAddress="AN.PHAMTHIEN@NCC.ASIA"
                        },
                        new UserToUpdateFromTSDto()
                        {   //EmployeeId=890
                            NewLevel = "6",
                            UserType = UserType.Staff,
                            IsFullSalary= true,
                            Salary = 8000000,
                            ApplyDate= DateTime.Now,
                            NormalizedEmailAddress="TRAN.DANGHUYEN@NCC.ASIA"
                        }
                    },
                Applydate = new DateTime(2022, 12, 10),
                CreatedBy = "Salary Change",
            };
            await WithUnitOfWorkAsync(async () =>
            {
                await _timesheetManager.ReviewInternFromTimesheet(input);
            });
            await WithUnitOfWorkAsync(async () =>
            {
                var existSalaryChangeRequestEmployees = _work.GetAll<SalaryChangeRequestEmployee>();
                var existSalaryChangeRequestEmployee = _work.GetAll<SalaryChangeRequestEmployee>()
                        .Where(x => x.EmployeeId == 880)
                        .OrderByDescending(x => x.CreationTime)
                        .FirstOrDefault();
                var existEmployeeContract = _work.GetAll<EmployeeContract>()
                        .Where(x => x.EmployeeId == 880)
                        .OrderByDescending(x => x.CreationTime)
                        .FirstOrDefault();
                var existChangRequests = _work.GetAll<SalaryChangeRequest>();
              
                var existChangRequest = _work.GetAll<SalaryChangeRequest>()
                .Where(x => x.Name == input.RequestName)
                .FirstOrDefault();
                var expectedNewSalary = 8000000;

                Assert.Equal(input.RequestName, existChangRequest.Name);
                Assert.Equal(7,existChangRequests.Count());
                Assert.Equal(60, existSalaryChangeRequestEmployees.Count());
                Assert.Equal(155, existSalaryChangeRequestEmployee.SalaryChangeRequestId);

                Assert.Equal(880, existSalaryChangeRequestEmployee.EmployeeId);
                Assert.Equal(new DateTime(2022, 12, 10), existSalaryChangeRequestEmployee.ApplyDate);
                Assert.Equal(155, existSalaryChangeRequestEmployee.SalaryChangeRequestId);
                Assert.Equal(UserType.Staff, existSalaryChangeRequestEmployee.FromUserType);
                Assert.Equal(UserType.Collaborators, existSalaryChangeRequestEmployee.ToUserType);
                Assert.Equal(7000000, existSalaryChangeRequestEmployee.Salary);
                Assert.Equal(8000000, existSalaryChangeRequestEmployee.ToSalary);
                Assert.Equal($"Create from Timesheet By {input.CreatedBy}", existSalaryChangeRequestEmployee.Note);

                Assert.Equal(880, existEmployeeContract.EmployeeId);
                Assert.Equal(new DateTime(2022, 12, 10), existEmployeeContract.StartDate);
                Assert.Equal(existSalaryChangeRequestEmployee.Id, existEmployeeContract.SalaryRequestEmployeeId);
                Assert.Equal(8000000, existEmployeeContract.RealSalary);
                Assert.Equal(UserType.Collaborators, existSalaryChangeRequestEmployee.ToUserType);
            });
        }

        //Test function CreateChangeRequestEmployeeAndContract

        [Fact]
        public async Task Should_Allow_Create_Change_Request_Employee_And_Contract()
        {
            var inputEmployeeToUpdates = new List<MapTSEmployeeDto>
                {
                    new MapTSEmployeeDto
                    {
                        Email="an.phamthien@ncc.asia",
                        EmployeeId=880,
                        OldUserType = UserType.Staff,
                        NewUserType = UserType.Staff,
                        OldLevel = 4,
                        NewLevel = 5,
                        JobPosition = 48,
                        OldSalary = 7000000,
                        NewSalary = 8000000,
                        IsFullSalary = true
                    },
                    new MapTSEmployeeDto
                    {
                        Email="tran.danghuyen@ncc.asia",
                        EmployeeId=890,
                        OldUserType = UserType.ProbationaryStaff,
                        NewUserType = UserType.ProbationaryStaff,
                        OldLevel = 5,
                        NewLevel = 5,
                        JobPosition = 48,
                        OldSalary = 6800000,
                        NewSalary = 8000000,
                        IsFullSalary = true
                    }
                };
            var inputChangeRequestId = 143;
            var inputApplyDate = new DateTime(2022, 12, 1);
            var inputCreatedBy = "Salary Change";
            await WithUnitOfWorkAsync(async () =>
            {
                await _timesheetManager.CreateChangeRequestEmployeeAndContract(inputEmployeeToUpdates, inputChangeRequestId, inputApplyDate, inputCreatedBy);
            });
            await WithUnitOfWorkAsync(async () =>
            {
                var existSalaryChangeRequestEmployee = _work.GetAll<SalaryChangeRequestEmployee>()
                       .Where(x => x.EmployeeId == 880)
                       .OrderByDescending(x => x.CreationTime)
                       .FirstOrDefault();
                var existEmployeeContract = _work.GetAll<EmployeeContract>()
                        .Where(x => x.EmployeeId == 880)
                        .OrderByDescending(x => x.CreationTime)
                        .FirstOrDefault();
                var expectedNewSalary = 8000000;

                Assert.Equal(inputChangeRequestId, existSalaryChangeRequestEmployee.SalaryChangeRequestId);

                Assert.Equal(880, existSalaryChangeRequestEmployee.EmployeeId);
                Assert.Equal(new DateTime(2022, 12, 01), existSalaryChangeRequestEmployee.ApplyDate);
                Assert.Equal(143, existSalaryChangeRequestEmployee.SalaryChangeRequestId);
                Assert.Equal(UserType.Staff, existSalaryChangeRequestEmployee.FromUserType);
                Assert.Equal(UserType.Staff, existSalaryChangeRequestEmployee.ToUserType);
                Assert.Equal(7000000, existSalaryChangeRequestEmployee.Salary);
                Assert.Equal(8000000, existSalaryChangeRequestEmployee.ToSalary);
                Assert.Equal($"Create from Timesheet By {inputCreatedBy}", existSalaryChangeRequestEmployee.Note);

                Assert.Equal(880, existEmployeeContract.EmployeeId);
                Assert.Equal(new DateTime(2022, 12, 01), existEmployeeContract.StartDate);
                Assert.Equal(existSalaryChangeRequestEmployee.Id, existEmployeeContract.SalaryRequestEmployeeId);
                Assert.Equal(8000000, existEmployeeContract.RealSalary);
                Assert.Equal(UserType.Staff, existEmployeeContract.UserType);
            });
        }
    }
}
