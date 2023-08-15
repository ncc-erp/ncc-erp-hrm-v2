using Abp.Domain.Uow;
using Abp.UI;
using HRMv2.Entities;
using HRMv2.Manager.Categories.Banks.Dto;
using HRMv2.Manager.Common.Dto;
using HRMv2.Manager.Timesheet.Dto;
using HRMv2.NccCore;
using HRMv2.Utils;
using HRMv2.WebServices.Timesheet;
using HRMv2.WebServices.Timesheet.Dto;
using Microsoft.EntityFrameworkCore;
using NccCore.Uitls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static HRMv2.Constants.Enum.HRMEnum;
using static HRMv2.Manager.Timesheet.Dto.InpuReviewInternFromTSDto;

namespace HRMv2.Manager.Timesheet
{
    public class TimesheetManager : BaseManager
    {

        private readonly TimesheetWebService _timesheetService;

        public TimesheetManager(TimesheetWebService timesheetService, IWorkScope workScope) : base(workScope)
        {
            _timesheetService = timesheetService;

        }
        public async Task UpdateAvatarFromTimesheet(AvatarDto input)
        {
            if (string.IsNullOrEmpty(input.AvatarPath))
            {
                Logger.Error($"{input.AvatarPath} is null or empty");
                return;
            }
            var employee = await GetEmployeeByEmail(input.EmailAddress);
            if (employee == null)
            {
                Logger.Error($"Not found employee with email {input.EmailAddress}");
                return;
            }
            employee.Avatar = input.AvatarPath;
            await WorkScope.UpdateAsync(employee);
        }

        private async Task<Employee> GetEmployeeByEmail(string emailAddress)
        {
            return await WorkScope.GetAll<Employee>()
                .Where(x => x.Email.ToLower().Trim() == emailAddress.ToLower().Trim())
                .FirstOrDefaultAsync();
        }

        public int GetCompayWorkingDay(int year, int month)
        {
            HashSet<DateTime> dayOffs = _timesheetService.GetSettingOffDates(year, month);
            if (dayOffs == null)
            {
                throw new UserFriendlyException("Can not collect dayoff from timesheet");
            }
            int standardWorkingDay = 0;

            for (var date = new DateTime(year, month, 1); date.Month == month; date = date.AddDays(1))
            {
                if (!dayOffs.Any(s => s == date) && date.DayOfWeek != DayOfWeek.Saturday && date.DayOfWeek != DayOfWeek.Sunday)
                    standardWorkingDay += 1;
            }

            return standardWorkingDay;
        }

        public async Task ReviewInternFromTimesheet(InputCreateRequestHrmv2Dto input)
        {
            var listLevel = WorkScope.GetAll<Level>()
                .Select(x => new { x.Id, x.Code }).ToList();

            var listPositonName = WorkScope.GetAll<JobPosition>()
                .Select(x => new { x.Id, x.Name }).ToList();

            var userTypeMapers = new UserType[] { UserType.Staff, UserType.Internship, UserType.Collaborators, UserType.ProbationaryStaff, UserType.Vendor };

            var empEmails = input.ListUserToUpdate.Select(x => x.NormalizedEmailAddress).ToList();
            var mapTSEmployee = (from e in WorkScope.GetAll<Employee>()
                                 .Where(x => empEmails.Contains(x.Email.ToUpper().Trim())).ToList()
                                 join t in input.ListUserToUpdate
                                 on e.Email.ToUpper().Trim() equals t.NormalizedEmailAddress
                                 select new MapTSEmployeeDto
                                 {
                                     EmployeeId = e.Id,
                                     Email = e.Email,
                                     OldUserType = e.UserType,
                                     NewUserType = userTypeMapers[(int)t.UserType] == UserType.Staff ? UserType.ProbationaryStaff : userTypeMapers[(int)t.UserType],
                                     OldLevel = e.LevelId,
                                     NewLevel = listLevel.Where(x => x.Code == t.NewLevel).Select(x => x.Id).FirstOrDefault(),
                                     OldSalary = e.RealSalary,
                                     NewSalary = t.Salary,
                                     JobPosition = e.JobPositionId,
                                     IsFullSalary = t.IsFullSalary
                                 }).ToList();

            var existChangRequest = WorkScope.GetAll<SalaryChangeRequest>()
                .Where(x => x.Name == input.RequestName)
                .FirstOrDefault();

            if (existChangRequest == default)
            {
                var newChangeRequest = new SalaryChangeRequest
                {
                    ApplyMonth = input.Applydate,
                    Name = input.RequestName,
                    Status = SalaryRequestStatus.New,
                };
                long changeRequestId = await WorkScope.InsertAndGetIdAsync(newChangeRequest);

                await CreateChangeRequestEmployeeAndContract(mapTSEmployee, changeRequestId, input.Applydate, input.CreatedBy);
            }
            else
            {
                var updateEmployeeIds = mapTSEmployee.Select(x => x.EmployeeId).ToList();
                var duplicateRqEmployees = WorkScope.GetAll<SalaryChangeRequestEmployee>()
                    .Where(x => x.SalaryChangeRequestId == existChangRequest.Id)
                    .Where(x => updateEmployeeIds.Contains(x.EmployeeId))
                    .ToList();

                if (duplicateRqEmployees != null && duplicateRqEmployees.Count > 0)
                {
                    var duplicateRqEmployeeIds = duplicateRqEmployees.Select(x => x.Id).ToList();
                    var duplicateContract = WorkScope.GetAll<EmployeeContract>()
                        .Where(x => duplicateRqEmployeeIds.Contains(x.SalaryRequestEmployeeId))
                        .ToList();

                    duplicateContract.ForEach(x => x.IsDeleted = true);
                    duplicateRqEmployees.ForEach(x => x.IsDeleted = true);
                }

                await CreateChangeRequestEmployeeAndContract(mapTSEmployee, existChangRequest.Id, existChangRequest.ApplyMonth, input.CreatedBy);
                CurrentUnitOfWork.SaveChanges();
            }
        }

        public async Task CreateChangeRequestEmployeeAndContract(List<MapTSEmployeeDto> employeeToUpdate, long changeRequestId, DateTime applyDate, string createdBy)
        {
            foreach (var employee in employeeToUpdate)
            {
                bool hasContract = employee.NewUserType == UserType.Staff
                    || employee.NewUserType == UserType.ProbationaryStaff
                    || employee.NewUserType == UserType.Collaborators;

                var requestEmployee = new SalaryChangeRequestEmployee
                {
                    EmployeeId = employee.EmployeeId,
                    ApplyDate = applyDate,
                    SalaryChangeRequestId = changeRequestId,
                    LevelId = employee.OldLevel,
                    ToLevelId = employee.NewLevel,
                    FromUserType = employee.OldUserType,
                    ToUserType = employee.NewUserType,
                    JobPositionId = employee.JobPosition,
                    ToJobPositionId = employee.JobPosition,
                    HasContract = hasContract,
                    Salary = employee.OldSalary,
                    ToSalary = employee.NewSalary,
                    Type = SalaryRequestType.Change,
                    Note = $"Create from Timesheet By {createdBy}",

                };
                var requestEmployeeId = await WorkScope.InsertAndGetIdAsync(requestEmployee);

                if (hasContract)
                {
                    var contractCode = CommonUtil.GenerateContractCode(employee.Email.ToUpper(), requestEmployee.ApplyDate.Month, requestEmployee.ApplyDate.Year, requestEmployee.ToUserType);

                    var probationPercentage = employee.IsFullSalary ? 100 : HRMv2Consts.TVIEC_PROBATIONPERCENTAGE;
                    var contract = new EmployeeContract
                    {
                        EmployeeId = requestEmployee.EmployeeId,
                        JobPositionId = requestEmployee.ToJobPositionId,
                        LevelId = requestEmployee.ToLevelId,
                        StartDate = requestEmployee.ApplyDate,
                        SalaryRequestEmployeeId = requestEmployeeId,
                        RealSalary = requestEmployee.ToSalary,
                        ProbationPercentage = probationPercentage,
                        BasicSalary = requestEmployee.ToSalary * 100 / probationPercentage,
                        UserType = requestEmployee.ToUserType,
                        Code = contractCode,
                        EndDate = CommonUtil.GenerateContractEndDate(requestEmployee.ToUserType, requestEmployee.ApplyDate)
                    };

                    await WorkScope.InsertAsync(contract);
                }
            }
        }

        public async Task<string> ComplainPayslipMail(InputcomplainPayslipDto input)
        {
            using (CurrentUnitOfWork.DisableFilter(AbpDataFilters.MayHaveTenant))
            {
            var activePayslip = WorkScope.GetAll<Payslip>()
                .Where(x => x.Id == input.PayslipId)
                .Where(x => x.Employee.Email == input.Email)
                .Where(x => x.Payroll.Status != PayrollStatus.Executed)
                .OrderByDescending(x => x.CreationTime)
                .FirstOrDefault();

            if (activePayslip == default)
            {
                return "Không tìm thấy phiếu lương, vui lòng kiểm tra lại email";
            }
            activePayslip.ConfirmStatus = PayslipConfirmStatus.ConfirmWrong;
            activePayslip.ComplainNote = input.ComplainNote;

            await WorkScope.UpdateAsync(activePayslip);

            return "Khiếu nại của bạn đã được gửi đi, hãy đợi kết quả từ HR nhé";
        }
         }

        public async Task<string> ConfirmPayslipMail(InputConfirmPayslipMailDto input)
        {
            using (CurrentUnitOfWork.DisableFilter(AbpDataFilters.MayHaveTenant))
            {
            var payslip = WorkScope.GetAll<Payslip>()
                .Where(x => x.Id == input.PayslipId)
                .Select(s => new { Payslip = s, s.Payroll.Status, s.Payroll.ApplyMonth, s.Employee.Email })
                .FirstOrDefault();

            if (payslip == default)
            {
                return "Không tìm thấy phiếu lương";
            }

            if (payslip.Status == PayrollStatus.Executed)
            {
                return "Đã quá hạn complain";
            }

            if (payslip.Email.ToLower().Trim() != input.Email.Trim().ToLower())
            {
                return "Đây không phải phiếu lương của bạn. Hệ thống đã lưu lại các thông tin để truy vết";
            }

            payslip.Payslip.ConfirmStatus = PayslipConfirmStatus.ConfirmRight;

            await WorkScope.UpdateAsync(payslip.Payslip);

            return $"Bạn đã xác nhận phiếu lương {DateTimeUtils.ToMMYYYY(payslip.ApplyMonth)} của {payslip.Email} là chính xác";
        }
            }

        public GetUserInfoByEmailDto GetUserInfoByEmail(string email)
        {
            var qEmployee = WorkScope.GetAll<Employee>()
                .Where(x => x.Email.ToLower().Trim() == email.ToLower().Trim());

            if (!qEmployee.Any())
            {
                throw new UserFriendlyException($"Can't found employee with email = {email}");
            }
            var employeeInfo = qEmployee
                .Select(x => new GetUserInfoByEmailDto()
                {
                    FullName = x.FullName,
                    Email = x.Email,
                    Phone = x.Phone,
                    Sex = x.Sex,
                    Birthday = x.Birthday,
                    Status = x.Status,
                    UserType = x.UserType,
                    Branch = x.Branch.Name,
                    Level = x.Level.Name,
                    JobPosition = x.JobPosition.Name,
                    Bank = x.Bank.Name,
                    SkillNames = x.EmployeeSkills.Select(s => s.Skill.Name).ToList(),
                    Teams = x.EmployeeTeams.Select(s => s.Team.Name).ToList(),
                    BankAccountNumber = x.BankAccountNumber,
                    IdCard = x.IdCard,
                    InsuranceStatus = x.InsuranceStatus,
                    TaxCode = x.TaxCode,
                    Address = x.Address,
                    PlaceOfPermanent = x.PlaceOfPermanent,
                    IssuedBy = x.IssuedBy,
                    IssuedOn = x.IssuedOn,
                    RemainLeaveDay = x.RemainLeaveDay,
                    BankId = x.BankId,
                    TeamIds = x.EmployeeTeams.Select(x => x.TeamId).ToList()
                }).FirstOrDefault();

            return employeeInfo;
        }

        public GetInfoToUPDateProfile GetInfoToUpdate(string email)
        {
            var qEmployee = WorkScope.GetAll<Employee>()
                .Where(x => x.Email.ToLower().Trim() == email.ToLower().Trim());
            if (!qEmployee.Any())
            {
                throw new UserFriendlyException($"Can't found employee with email = {email}");
            }
            var result = GetPendingTempEmployeeTS(qEmployee.FirstOrDefault().Id);

            if (result != default)
            {
                return result;
            }

            return qEmployee.Select(x => new GetInfoToUPDateProfile
            {
                Phone = x.Phone,
                Birthday = x.Birthday,
                BankId = x.BankId,
                BankAccountNumber = x.BankAccountNumber,
                IdCard = x.IdCard,
                TaxCode = x.TaxCode,
                Address = x.Address,
                PlaceOfPermanent = x.PlaceOfPermanent,
                IssuedBy = x.IssuedBy,
                IssuedOn = x.IssuedOn,
            }).FirstOrDefault();
        }

        private GetInfoToUPDateProfile GetPendingTempEmployeeTS(long employeeId)
        {
            return WorkScope.GetAll<TempEmployeeTS>()
                .Where(x => x.EmployeeId == employeeId && x.RequestStatus == RequestStatus.Pending)
                .Select(x => new GetInfoToUPDateProfile
                {
                    Phone = x.Phone,
                    Birthday = x.Birthday,
                    BankId = x.BankId,
                    BankAccountNumber = x.BankAccountNumber,
                    IdCard = x.IdCard,
                    TaxCode = x.TaxCode,
                    Address = x.Address,
                    PlaceOfPermanent = x.PlaceOfPermanent,
                    IssuedBy = x.IssuedBy,
                    IssuedOn = x.IssuedOn,
                    RequestStatus = x.RequestStatus

                }).FirstOrDefault();
        }

        public List<ItemInfoDto> GetAllBanks()
        {
            return WorkScope.GetAll<Bank>()
                .Select(x => new ItemInfoDto
                {
                    Name = x.Name,
                    Id = x.Id
                }).ToList();
        }


        public void CheckExistUser(string email)
        {
            var qEmployee = WorkScope.GetAll<Employee>()
                .Where(x => x.Email.ToLower().Trim() == email.ToLower().Trim());

            if (!qEmployee.Any())
            {
                throw new UserFriendlyException($"Can't found employee with email = {email}");
            }
        }

        public async Task<ResultUpdateInfo> CreateRequestUpdateUserInfo(UpdateUserInfoDto input)
        {

            var employee = WorkScope.GetAll<Employee>()
                .Where(x => x.Email.ToLower().Trim() == input.Email.ToLower().Trim()).FirstOrDefault();

            if (employee == default)
            {
                return new ResultUpdateInfo
                {
                    IsSucess = false,
                    ResultMessage = $"Can not found employee with email = {input.Email}"
                };
            }
            var dictBank = WorkScope.GetAll<Bank>()
                                    .Select(s => new { Key = s.Code.ToLower(), s.Id })
                                    .ToDictionary(s => s.Key, s => s.Id);

            var requestEmp = WorkScope.GetAll<TempEmployeeTS>()
                .Where(x => x.EmployeeId == employee.Id && x.RequestStatus == RequestStatus.Pending)
                .OrderByDescending(x => x.CreationTime)
                .FirstOrDefault();

            if (requestEmp == default)
            {
                var request = new TempEmployeeTS
                {
                    EmployeeId = employee.Id,
                    BankAccountNumber = input.BankAccountNumber,
                    TaxCode = input.TaxCode,
                    Address = input.Address,
                    Birthday = input.Birthday,
                    Phone = input.Phone,
                    IdCard = input.IdCard,
                    IssuedOn = input.IssuedOn,
                    IssuedBy = input.IssuedBy,
                    PlaceOfPermanent = input.PlaceOfPermanent,
                    BankId = input.BankId,
                    RequestStatus = RequestStatus.Pending,

                };
                await WorkScope.InsertAsync<TempEmployeeTS>(request);
            }
            else
            {
                requestEmp.BankAccountNumber = input.BankAccountNumber;
                requestEmp.TaxCode = input.TaxCode;
                requestEmp.Address = input.Address;
                requestEmp.Birthday = input.Birthday;
                requestEmp.Phone = input.Phone;
                requestEmp.IdCard = input.IdCard;
                requestEmp.IssuedOn = input.IssuedOn;
                requestEmp.IssuedBy = input.IssuedBy;
                requestEmp.PlaceOfPermanent = input.PlaceOfPermanent;
                requestEmp.BankId = input.BankId;

                await WorkScope.UpdateAsync(requestEmp);
            }

            return new ResultUpdateInfo
            {
                IsSucess = true,
                ResultMessage = "Reuqest change info successful"
            };
        }



    }
}
