using Abp.BackgroundJobs;
using Abp.Domain.Repositories;
using Abp.UI;
using HRMv2.Entities;
using HRMv2.Manager.ChangeEmployeeWorkingStatuses.Dto;
using HRMv2.Manager.Common.Dto;
using HRMv2.Manager.EmployeeContracts;
using HRMv2.Manager.Employees;
using HRMv2.Manager.Employees.Dto;
using HRMv2.Manager.Histories;
using HRMv2.Manager.Histories.Dto;
using HRMv2.Manager.WarningEmployees.Dto;
using HRMv2.NccCore;
using HRMv2.Utils;
using Microsoft.EntityFrameworkCore;
using NccCore.Extension;
using NccCore.Paging;
using NccCore.Uitls;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HRMv2.Constants.Enum.HRMEnum;

namespace HRMv2.Manager.WarningEmployees
{
    public class WarningEmployeeManager : BaseManager
    {
        public readonly EmployeeManager _employeeManager;
        private readonly IRepository<BackgroundJobInfo, long> _storeJob;
        private readonly IBackgroundJobManager _backgroundJobInfosManager;
        private readonly HistoryManager _historyManager;


        public WarningEmployeeManager(IWorkScope workScope,
            EmployeeManager employeeManager,
            IBackgroundJobManager backgroundJobManager,
            IRepository<BackgroundJobInfo, long> storeJob,
            HistoryManager historyManager) : base(workScope)
        {
            _employeeManager = employeeManager;
            _storeJob = storeJob;
            _backgroundJobInfosManager = backgroundJobManager;
            _historyManager = historyManager;
        }

        public IQueryable<GetEmployeeContractDto> QueryAllEmployeesToUpdateContract()
        {

            var qcontract = WorkScope.GetAll<EmployeeContract>()
                .Select(x => new
                {
                    x.EmployeeId,
                    x.Code,
                    x.StartDate,
                    x.EndDate
                })
            .GroupBy(x => x.EmployeeId)
            .Select(x => new EmployeeToUpdateContractDto
            {
                EmployeeId = x.Key,
                StartDate = x.OrderByDescending(s => s.StartDate).Select(s => s.StartDate).FirstOrDefault(),
                EndDate = x.OrderByDescending(s => s.StartDate).Select(s => s.EndDate).FirstOrDefault(),
                Code = x.OrderByDescending(s => s.StartDate).Select(s => s.Code).FirstOrDefault(),
            });

            var listEmployee = from e in WorkScope.GetAll<Employee>() where e.Status == EmployeeStatus.Working
                               join ec in qcontract
                               on e.Id equals ec.EmployeeId
                               select new GetEmployeeContractDto
                               {
                                   Id = e.Id,
                                   JobPositionId = e.JobPositionId,
                                   Status = e.Status,
                                   UpdatedTime = e.LastModificationTime,
                                   UpdatedUser = e.LastModifierUser.FullName,
                                   StartWorkingDate = e.StartWorkingDate,
                                   FullName = e.FullName,
                                   Avatar = e.Avatar,
                                   Sex = e.Sex,
                                   Email = e.Email,
                                   BranchId = e.BranchId,
                                   LevelId = e.LevelId,
                                   UserType = e.UserType,
                                   ContractCode = ec.Code,
                                   ContractStartDate = ec.StartDate,
                                   ContractEndDate = ec.EndDate,
                                   BranchInfo = new BadgeInfoDto
                                   {
                                       Name = e.Branch.Name,
                                       Color = e.Branch.Color
                                   },
                                   LevelInfo = new BadgeInfoDto
                                   {
                                       Name = e.Level.Name,
                                       Color = e.Level.Color
                                   },
                                   JobPositionInfo = new BadgeInfoDto
                                   {
                                       Name = e.JobPosition.Name,
                                       Color = e.JobPosition.Color
                                   },
                               };
            return listEmployee;
        }
        public async Task<GridResult<GetEmployeeContractDto>> GetAllEmployeesToUpdateContract(InputMultiFilterEmployeePagingDto input)
        {
            var query = QueryAllEmployeesToUpdateContract();

            if (input.BranchIds != null && input.BranchIds.Count == 1) query = query.Where(x => input.BranchIds[0] == x.BranchId);
            else if (input.BranchIds != null && input.BranchIds.Count > 1) query = query.Where(x => input.BranchIds.Contains(x.BranchId));

            if (input.Usertypes != null && input.Usertypes.Count == 1) query = query.Where(x => input.Usertypes[0] == x.UserType);
            else if (input.Usertypes != null && input.Usertypes.Count > 1) query = query.Where(x => input.Usertypes.Contains(x.UserType));

            if (input.JobPositionIds != null && input.JobPositionIds.Count == 1) query = query.Where(x => input.JobPositionIds[0] == x.JobPositionId);
            else if (input.JobPositionIds != null && input.JobPositionIds.Count > 1) query = query.Where(x => input.JobPositionIds.Contains(x.JobPositionId));


            if (input.DaysLeftContractEndDate != null)
            {
                query = query.Where(x => DateTime.Now.AddDays((int)input.DaysLeftContractEndDate) >= x.ContractEndDate);
            }

            return await query.GetGridResult(query, input.GridParam);
        }
        public async Task<GridResult<GetEmployeeBackToWork>> GetAllEmployeesBackToWork(InputMultiFilterEmployeePagingDto input)
        {
            var getEmployeeWorkingHistory = WorkScope.GetAll<EmployeeWorkingHistory>()
                                                    .GroupBy(p => p.EmployeeId)
                                                    .Select(x => new GetEmployeeWorkingHistoryDto
                                                    {
                                                        EmployeeId = x.Key,
                                                        BackDate = x.OrderByDescending(x => x.DateAt).ThenByDescending(x=> x.CreationTime).FirstOrDefault().BackDate,
                                                        ApplyDate = x.OrderByDescending(x => x.DateAt).ThenByDescending(x => x.CreationTime).FirstOrDefault().DateAt
                                                    });
            var employees = WorkScope.GetAll<Employee>();

            var query = from x in employees
                        join s in getEmployeeWorkingHistory on x.Id equals s.EmployeeId
                        where x.Status == EmployeeStatus.MaternityLeave || x.Status == EmployeeStatus.Pausing
                        orderby s.BackDate
                        select new GetEmployeeBackToWork
                        {
                            Id = x.Id,
                            JobPositionId = x.JobPositionId,
                            Status = x.Status,
                            UpdatedTime = x.LastModificationTime,
                            UpdatedUser = x.LastModifierUser.FullName,
                            StartWorkingDate = x.StartWorkingDate,
                            FullName = x.FullName,
                            Avatar = x.Avatar,
                            Sex = x.Sex,
                            Email = x.Email,
                            BranchId = x.BranchId,
                            LevelId = x.LevelId,
                            UserType = x.UserType,
                            BranchInfo = new BadgeInfoDto
                            {
                                Name = x.Branch.Name,
                                Color = x.Branch.Color
                            },
                            LevelInfo = new BadgeInfoDto
                            {
                                Name = x.Level.Name,
                                Color = x.Level.Color
                            },
                            JobPositionInfo = new BadgeInfoDto
                            {
                                Name = x.JobPosition.Name,
                                Color = x.JobPosition.Color
                            },
                            BackDate = s.BackDate,
                            ApplyDate = s.ApplyDate,
                            RealSalary = x.RealSalary

                        };




            if (input.StatusIds != null && input.StatusIds.Count == 1) query = query.Where(x => input.StatusIds[0] == x.Status);
            else if (input.StatusIds != null && input.StatusIds.Count > 1) query = query.Where(x => input.StatusIds.Contains(x.Status));

            return await query.GetGridResult(query, input.GridParam);
        }
        public async Task<UpdateEmployeeBackdateDto> UpdateEmployeeBackDate(UpdateEmployeeBackdateDto input)
        {
            var entity = WorkScope.GetAll<EmployeeWorkingHistory>()
                        .Where(x => x.EmployeeId == input.EmployeeId)
                        .OrderByDescending(x => x.DateAt)
                        .ThenByDescending(x => x.CreationTime)
                        .FirstOrDefault();
            if (entity == default)
            {
                throw new UserFriendlyException("Can't found employee working history");
            }
            if (entity.BackDate != input.BackDate)
            {
                entity.BackDate = input.BackDate;

                await WorkScope.UpdateAsync(entity);
            }
            return input;

        }

        public IQueryable<GetTempEmployeeTalentDto> IQGetTempEmployeeTalent()
        {
            var query = WorkScope.GetAll<TempEmployeeTalent>()
                .Select(x => new GetTempEmployeeTalentDto
                {
                    Id = x.Id,
                    FullName = x.FullName,
                    Email = x.PersonalEmail,
                    NCCEmail = x.NCCEmail,
                    DateOfBirth = x.DateOfBirth,
                    Sex = x.Gender,
                    Phone = x.Phone,
                    SkillStr = x.Skills,
                    OnboardStatus = x.Status,
                    Salary = x.Salary,
                    JobPositionInfo = x.JobPositionId.HasValue ? new BadgeInfoDto
                    {
                        Name = x.JobPosition.Name,
                        Color = x.JobPosition.Color
                    } : null,
                    BranchInfo = x.BranchId.HasValue ? new BadgeInfoDto
                    {
                        Name = x.Branch.Name,
                        Color = x.Branch.Color
                    } : null,
                    LevelInfo = x.LevelId.HasValue ? new BadgeInfoDto
                    {
                        Name = x.Level.Name,
                        Color = x.Level.Color
                    } : null,
                    UserTypeInfo = new BadgeInfoDto
                    {
                        Name = CommonUtil.GetUserTypeNameVN(x.UserType.Value),
                        Color = CommonUtil.GetUserType(x.UserType.Value).Color
                    },
                    OnboardDate = x.OnboardDate,
                    UserType = x.UserType,
                    BranchId = x.BranchId,
                    JobPositionId = x.JobPositionId,
                    LevelId = x.LevelId,
                    ProbationPercentage = x.ProbationPercentage,
                    UpdatedTime = x.LastModificationTime,
                    UpdatedUser = x.LastModifierUser.FullName,
                    CreationTime = x.CreationTime
                });

            return query;
        }

        public async Task<GridResult<GetTempEmployeeTalentDto>> GetTempEmployeeTalentPaging(InputGetTemEmployeeTalentDto input, bool hasPermissionViewSalary)
        {
            var query = IQGetTempEmployeeTalent();

            if (input.BranchIds != null && input.BranchIds.Count == 1) query = query.Where(x => x.BranchId == input.BranchIds[0]);
            else if (input.BranchIds != null && input.BranchIds.Count > 1) query = query.Where(x => x.BranchId.HasValue && input.BranchIds.Contains(x.BranchId.Value));

            if (input.Usertypes != null && input.Usertypes.Count == 1) query = query.Where(x => input.Usertypes[0] == x.UserType);
            else if (input.Usertypes != null && input.Usertypes.Count > 1) query = query.Where(x => x.UserType.HasValue && input.Usertypes.Contains(x.UserType.Value));

            if (input.JobPositionIds != null && input.JobPositionIds.Count == 1) query = query.Where(x => input.JobPositionIds[0] == x.JobPositionId);
            else if (input.JobPositionIds != null && input.JobPositionIds.Count > 1) query = query.Where(x => x.JobPositionId.HasValue && input.JobPositionIds.Contains(x.JobPositionId.Value));

            if (input.StatusIds != null && input.StatusIds.Count == 1) query = query.Where(x => input.StatusIds[0] == x.OnboardStatus);
            else if (input.StatusIds != null && input.StatusIds.Count > 1) query = query.Where(x => x.OnboardStatus.HasValue && input.StatusIds.Contains(x.OnboardStatus.Value));

            var results = await query.GetGridResult(query, input.GridParam);

            if (!hasPermissionViewSalary)
                for (var i = 0; i < results.Items.Count(); i++)
                {
                    results.Items[i].Salary = 0;
                }

            return results;
        }

        public async Task<UpdateTempEmployeeTalentDto> UpdateTempEmployeeTalent(UpdateTempEmployeeTalentDto input)
        {
            if (input == null) throw new UserFriendlyException($"Invalid request");
            var updateTempEmployee = await WorkScope.GetAsync<TempEmployeeTalent>(input.Id) ?? throw new UserFriendlyException($"Can't find temp employee with id {input.Id}");
            ObjectMapper.Map(input, updateTempEmployee);
            await WorkScope.UpdateAsync(updateTempEmployee);
            return input;
        }

        public async Task<long> DeleteTempEmployeeTalent(long id)
        {
            var deleteTempEmployee = WorkScope.GetAll<TempEmployeeTalent>()
                .Where(x => x.Id == id)
                .FirstOrDefault();
            
            if(deleteTempEmployee == default)
            {
                throw new UserFriendlyException($"Can't find temp employee with id {id}");
            }

            await WorkScope.DeleteAsync(deleteTempEmployee);

            return id;
        }

        public GetTempEmployeeTalentDto GetTempEmployeeTalentById(long id)
        {
            return IQGetTempEmployeeTalent()
                .Where(x => x.Id == id)
                .FirstOrDefault();
        }

        public async Task<GridResult<GetRequestUpdateInfoDto>> GetRequestUpdateInfo(InputMultiFilterRequestDto input)
        {
            var qrequest = WorkScope.GetAll<TempEmployeeTS>();

            var qemployee = WorkScope.GetAll<Employee>();

            var query = from r in qrequest
                        join e in qemployee
                        on r.EmployeeId equals e.Id
                        select new GetRequestUpdateInfoDto
                        {
                            Id = r.Id,
                            EmployeeId = r.EmployeeId,
                            FullName = e.FullName,
                            Email = e.Email,
                            Sex = e.Sex,
                            BranchInfo = new BadgeInfoDto
                            {
                                Name = e.Branch.Name,
                                Color = e.Branch.Color
                            },
                            LevelInfo = new BadgeInfoDto
                            {
                                Name = e.Level.Name,
                                Color = e.Level.Color
                            },
                            JobPositionInfo = new BadgeInfoDto
                            {
                                Name = e.JobPosition.Name,
                                Color = e.JobPosition.Color
                            },
                            Avatar = e.Avatar,
                            UserType = e.UserType,
                            RequestStatus = r.RequestStatus,
                            UpdatedTime = r.LastModificationTime,
                            UpdatedUser = r.LastModifierUser.FullName,
                            CreationTime = r.CreationTime
                        };
            if (input.RequestStatuses != null && input.RequestStatuses.Count == 1) query = query.Where(x => input.RequestStatuses[0] == x.RequestStatus);
            else if (input.RequestStatuses != null && input.RequestStatuses.Count > 1) query = query.Where(x => input.RequestStatuses.Contains(x.RequestStatus));
            return await query.GetGridResult(query, input.GridParam);
        }

        public async Task ApproveRequestUpdateInfo(ApproveChangeInfoDto input)
        {
            var request = WorkScope.GetAll<TempEmployeeTS>()
                .Where(x => x.Id == input.Id)
                .FirstOrDefault();
            if (request == default)
            {
                throw new UserFriendlyException($"Can not found any request with Id = {input.Id}");
            }
            request.RequestStatus = RequestStatus.Approved;

            var employee = WorkScope.GetAll<Employee>()
                .Where(x => x.Id == request.EmployeeId)
                .FirstOrDefault();
            employee.Phone = input.Phone;
            employee.Birthday = input.Birthday;
            employee.BankId = input.BankId;
            employee.BankAccountNumber = input.BankAccountNumber;
            employee.TaxCode = input.TaxCode;
            employee.IdCard = input.IdCard;
            employee.Address = input.Address;
            employee.PlaceOfPermanent = input.PlaceOfPermanent;
            employee.IssuedOn = input.IssuedOn;
            employee.IssuedBy = input.IssuedBy;

            await WorkScope.UpdateAsync(request);
            await WorkScope.UpdateAsync(employee);


        }
        public async Task RejectRequestUpdateInfo(RejectChangeInfoDto input)
        {
            var request = WorkScope.GetAll<TempEmployeeTS>()
                .Where(x => x.Id == input.Id)
                .FirstOrDefault();
            if (request == default)
            {
                throw new UserFriendlyException($"Can not found any request with Id = {input.Id}");
            }
            request.RequestStatus = RequestStatus.Rejected;
            await WorkScope.UpdateAsync(request);

        }

        public GetRequestDetailDto GetRequestDetailById(long id)
        {

            return WorkScope.GetAll<TempEmployeeTS>()
                .Where(x => x.Id == id)
                .Select(x => new GetRequestDetailDto
                {
                    Id = x.Id,
                    EmployeeId = x.EmployeeId,
                    BankAccountNumber = x.BankAccountNumber,
                    EmployeeBankAccountNumber = x.Employee.BankAccountNumber,
                    TaxCode = x.TaxCode,
                    EmployeeTaxCode = x.Employee.TaxCode,
                    Address = x.Address,
                    EmployeeAddress = x.Employee.Address,
                    Birthday = x.Birthday,
                    EmployeeBirthDay = x.Employee.Birthday,
                    Phone = x.Phone,
                    EmployeePhone = x.Employee.Phone,
                    IdCard = x.IdCard,
                    EmployeeIdCard = x.Employee.IdCard,
                    IssuedOn = x.IssuedOn,
                    EmployeeIssuedOn = x.Employee.IssuedOn,
                    IssuedBy = x.IssuedBy,
                    EmployeeIssuedBy = x.Employee.IssuedBy,
                    PlaceOfPermanent = x.PlaceOfPermanent,
                    EmployeePlaceOfPermanent = x.Employee.PlaceOfPermanent,
                    RequestStatus = x.RequestStatus,
                    BankId = x.BankId,
                    EmployeeBankId = x.Employee.BankId
                }).FirstOrDefault();
        }

        public List<GetPlanQuitEmployeeDto> GetPlanQuitEmployee()
        {
            var dicPlanQuitEmployees = WorkScope.GetAll<EmployeeWorkingHistory>()
                .Where(x => x.DateAt > DateTimeUtils.GetNow())
                .Where(x => x.Status == EmployeeStatus.Quit || x.Status == EmployeeStatus.Pausing)
                .Select(x => new
                {
                    x.EmployeeId,
                    x.DateAt,
                    Status = x.Status == EmployeeStatus.Quit ? "Confirm quit" : "Confirm pause",
                    x.CreationTime,
                    x.Id
                }).ToDictionary(x => x.EmployeeId, x => x);

            var dicPlanQuitBackgroundJobs = _storeJob.GetAll()
                .Where(x => !x.IsAbandoned)
                .Where(x => x.JobType.Contains("ChangeWorkingStatusToQuit") || x.JobType.Contains("ChangeWorkingStatusToPause"))
                .ToList()
                .Select(x => new
                {
                    JobArgs = JsonConvert.DeserializeObject<ToQuitDto>(x.JobArgs),
                    CreationTime = x.CreationTime,
                    JobId = x.Id,
                    IsAbandoned = x.IsAbandoned,
                    Status = x.JobType.Contains("ChangeWorkingStatusToQuit") ? "Plan quit" : "Plan pause"
                })
                .Select(x => new
                {
                    x.JobId,
                    x.JobArgs.EmployeeId,
                    DateAt = x.JobArgs.ApplyDate,
                    x.CreationTime,
                    Status =  x.Status,
                    IsAbandoned = x.IsAbandoned,
                }).ToDictionary(x => x.EmployeeId, x => x);

            var quitEmployeeIds = dicPlanQuitEmployees.Keys.Union(dicPlanQuitBackgroundJobs.Keys);

            return WorkScope.GetAll<Employee>()
                .Where(x => quitEmployeeIds.Contains(x.Id))
                .Select(x => new GetPlanQuitEmployeeDto
                {
                    Id = x.Id,
                    FullName = x.FullName,
                    Email = x.Email,
                    Avatar = x.Avatar,
                    Sex = x.Sex,
                    UserType = x.UserType,
                    BranchInfo = new BadgeInfoDto
                    {
                        Name = x.Branch.Name,
                        Color = x.Branch.Color
                    },
                    LevelInfo = new BadgeInfoDto
                    {
                        Name = x.Level.Name,
                        Color = x.Level.Color
                    },
                    JobPositionInfo = new BadgeInfoDto
                    {
                        Name = x.JobPosition.Name,
                        Color = x.JobPosition.Color
                    },
                    WorkingHistoryId = dicPlanQuitEmployees.ContainsKey(x.Id) ? dicPlanQuitEmployees[x.Id].Id : null,
                    WorkingStatus = dicPlanQuitBackgroundJobs.ContainsKey(x.Id) ? dicPlanQuitBackgroundJobs[x.Id].Status : dicPlanQuitEmployees[x.Id].Status,
                    DateAt = dicPlanQuitBackgroundJobs.ContainsKey(x.Id) ? dicPlanQuitBackgroundJobs[x.Id].DateAt : dicPlanQuitEmployees[x.Id].DateAt,
                    CreationTime = dicPlanQuitBackgroundJobs.ContainsKey(x.Id) ? dicPlanQuitBackgroundJobs[x.Id].CreationTime : dicPlanQuitEmployees[x.Id].CreationTime,
                    JobId = dicPlanQuitBackgroundJobs.ContainsKey(x.Id) ? dicPlanQuitBackgroundJobs[x.Id].JobId : null,
                    IsAbandoned = dicPlanQuitBackgroundJobs.ContainsKey(x.Id) ? dicPlanQuitBackgroundJobs[x.Id].IsAbandoned : null,
                }).ToList();
        }

        public async Task<long> DeletePlanQuitBgJob(long id )
        {
            await _backgroundJobInfosManager.DeleteAsync(id.ToString());
            return id;
        }
        public async Task<UpdatePlanToQuitEmployeeDto> UpdatePlanQuitBgJob(UpdatePlanToQuitEmployeeDto input)
        {
           if(input.JobId != default)
           {
                var job = _storeJob.GetAll().Where(x => x.Id == input.JobId).FirstOrDefault();
                var jobArgs = JsonConvert.DeserializeObject<ToQuitDto>(job.JobArgs);
                jobArgs.ApplyDate = input.DateAt;
                job.JobArgs = JsonConvert.SerializeObject(jobArgs);
                job.NextTryTime = new DateTime(input.DateAt.Year, input.DateAt.Month, input.DateAt.Day, 5, 0, 0);
                _storeJob.Update(job);
                return input;
           }
           if(input.WorkingHistoryId != default)
           {
                var inputToUpdate = new UpdateDateWorkingHistoryDto
                {
                    Id = (long)input.WorkingHistoryId,
                    DateAt = input.DateAt
                };
                await _historyManager.UpdateDateInWorkingHistory(inputToUpdate);
                return input;
           }
           return input;
        }

    }
}
