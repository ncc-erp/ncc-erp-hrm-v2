using Abp.UI;
using HRMv2.Entities;
using HRMv2.Manager.Categories;
using HRMv2.Manager.Categories.JobPositions;
using HRMv2.Manager.Categories.Levels;
using HRMv2.Manager.ChangeEmployeeWorkingStatuses;
using HRMv2.Manager.ChangeEmployeeWorkingStatuses.Dto;
using HRMv2.Manager.Common.Dto;
using HRMv2.Manager.Histories.Dto;
using HRMv2.Manager.Salaries.Payslips;
using HRMv2.NccCore;
using HRMv2.WebServices.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static HRMv2.Constants.Enum.HRMEnum;

namespace HRMv2.Manager.Histories
{
    public class HistoryManager : BaseManager
    {
        private readonly BranchManager _branchManager;
        private readonly LevelManager _levelManager;
        private readonly ChangeEmployeeWorkingStatusManager _changeEmployeeWorkingStatusManager;
        public HistoryManager(IWorkScope workScope, 
            BranchManager branchManager,
            LevelManager levelManager,
            ChangeEmployeeWorkingStatusManager changeEmployeeWorkingStatusManager) : base(workScope)
        {
            _branchManager = branchManager;
            _levelManager = levelManager;
            _changeEmployeeWorkingStatusManager = changeEmployeeWorkingStatusManager;

        }

        public IQueryable<EmployeeBranchHistoryDto> QueryAllBranchHistory()
        {
            var mapBranchIdToInfo = _branchManager.QueryAllBranch()
            .Select(x => new
            {
               Name = x.Name,
               Color = x.Color,
               Id = x.Id

            })
            .ToDictionary(x => x.Id, x => new BadgeInfoDto
            {
                Name = x.Name,
                Color = x.Color
            });
            return WorkScope.GetAll<EmployeeBranchHistory>()
                .OrderByDescending(x => x.DateAt)
                .ThenByDescending(x=> x.CreationTime)
                .Select(x => new EmployeeBranchHistoryDto
                {
                    Id = x.Id,
                    EmployeeId = x.EmployeeId,
                    BranchId = x.BranchId,
                    Note = x.Note,
                    DateAt = x.DateAt,
                    BranchInfo = new BadgeInfoDto
                    {
                        Name = mapBranchIdToInfo.ContainsKey(x.BranchId) ? mapBranchIdToInfo[x.BranchId].Name : null,
                        Color = mapBranchIdToInfo.ContainsKey(x.BranchId) ? mapBranchIdToInfo[x.BranchId].Color : null
                    },
                    UpdatedTime = x.LastModificationTime,
                    UpdatedUser = x.LastModifierUser.FullName,
                    IsNotAllowToDelete = false

                });
        }

        public IQueryable<EmployeeWorkingHistoryDto> QueryAllWorkingHistory()
        {
        
            return WorkScope.GetAll<EmployeeWorkingHistory>()
                .OrderByDescending(x => x.DateAt)
                .ThenByDescending(x=> x.CreationTime)
                .Select(x => new EmployeeWorkingHistoryDto
                {
                    Id = x.Id,
                    EmployeeId = x.EmployeeId,
                    Status = x.Status,
                    Note = x.Note,
                    DateAt = x.DateAt,
                    UpdatedTime = x.LastModificationTime,
                    UpdatedUser = x.LastModifierUser.FullName,
                    IsNotAllowToDelete = false
                });
        }


        public IQueryable<EmployeeSalaryHistoryDto> QueryAllSalaryHistory(long employeeId)
        {
            var mapLevelIdToInfo = _levelManager.QueryAllLevel()
            .Select(x => new
            {
                Name = x.Name,
                Color = x.Color,
                Id = x.Id

            })
            .ToDictionary(x => x.Id, x => new BadgeInfoDto
            {
                Name = x.Name,
                Color = x.Color
            });

            var employeeContract = WorkScope.GetAll<EmployeeContract>();

            var salaryChangeRequestEmployee = WorkScope.GetAll<SalaryChangeRequestEmployee>()
                .Where(x => x.EmployeeId == employeeId);

            var query = (from scre in salaryChangeRequestEmployee 
                        join ec in employeeContract
                        on  scre.Id equals ec.SalaryRequestEmployeeId into sh
                        from s in sh.DefaultIfEmpty()
                        select new EmployeeSalaryHistoryDto
                        {
                            Id = scre.Id,
                            EmployeeId = scre.EmployeeId,
                            FromUserType = scre.FromUserType,
                            ToUserType = scre.ToUserType,
                            FromJobPositionId = scre.JobPositionId,
                            FromLevelId = scre.LevelId,
                            ToJobPositionId = scre.ToJobPositionId,
                            FromLevelInfo = new BadgeInfoDto
                            {
                                Name = mapLevelIdToInfo.ContainsKey(scre.LevelId) ? mapLevelIdToInfo[scre.LevelId].Name : null,
                                Color = mapLevelIdToInfo.ContainsKey(scre.LevelId) ? mapLevelIdToInfo[(scre.LevelId)].Color : null,
                            },
                            ToLevelId = scre.ToLevelId,
                            ToLevelInfo = new BadgeInfoDto
                            {
                                Name = mapLevelIdToInfo.ContainsKey(scre.ToLevelId) ? mapLevelIdToInfo[scre.ToLevelId].Name : null,
                                Color = mapLevelIdToInfo.ContainsKey(scre.ToLevelId) ? mapLevelIdToInfo[(scre.ToLevelId)].Color : null,
                            },
                            FromSalary = scre.Salary,
                            ToSalary = scre.ToSalary,
                            ContractCode = s.Code,
                            ApplyDate = scre.ApplyDate,
                            Note = scre.Note,
                            UpdatedTime = scre.LastModificationTime,
                            UpdatedUser = scre.LastModifierUser != null ? scre.LastModifierUser.FullName : "Ts tool",
                            IsNotAllowToDelete = false,
                            Type = scre.Type,
                            Request = scre.SalaryChangeRequest != null ? new ChangeRequestInfoDto
                            {
                                Id = scre.SalaryChangeRequest.Id,
                                Name = scre.SalaryChangeRequest.Name,
                                Status = scre.SalaryChangeRequest.Status,
                            } : null,
                            CreationTime = scre.CreationTime,
                            HasContract = scre.HasContract
                        }).OrderByDescending(x => x.ApplyDate)
                          .ThenByDescending(x => x.CreationTime);

            return query;
        }

        public IQueryable<EmployeePayslipHistoryDto> QueryAllPayslipHistory(long employeeId)
        {
            var payslipSalaries = WorkScope.GetAll<PayslipSalary>()
              .GroupBy(x => x.PayslipId)
              .Select(x => new
              {
                  x.Key,
                  ListSalary = x.Select(s => new StandardSalaryDto
                  {
                      Salary = s.Salary,
                      Date = s.Date
                  })
              }).ToDictionary(x => x.Key, x => x.ListSalary);

            var payslipDetail = WorkScope.GetAll<PayslipDetail>()
                .GroupBy(x => new {Type = x.Type , Id =  x.PayslipId})
                .Select(x => new
                {
                    x.Key,
                    TotalMoney = x.Sum(s => s.Money)
                }).ToDictionary(x => x.Key, x =>  x.TotalMoney);

            var branches = WorkScope.GetAll<Branch>()
                .ToDictionary(x => x.Id, x => new BadgeInfoDto { Name = x.Name, Color = x.Color });

            var levels = WorkScope.GetAll<Level>()
                .ToDictionary(x => x.Id, x => new BadgeInfoDto { Name = x.Name, Color = x.Color });

            var jobPositions = WorkScope.GetAll<JobPosition>()
               .ToDictionary(x => x.Id, x => new BadgeInfoDto { Name = x.Name, Color = x.Color });

            var payslips = WorkScope.GetAll<Payslip>()
                .Where(x => x.EmployeeId == employeeId)
                .Select(x => new EmployeePayslipHistoryDto
                {
                    Id = x.Id,
                    EmployeeId = x.EmployeeId,
                    PayslipId = x.Id,
                    FullName = x.Employee.FullName,
                    Avatar = x.Employee.Avatar,
                    Sex = x.Employee.Sex,
                    Email = x.Employee.Email,
                    UserType = x.Employee.UserType,
                    PayslipUserType = x.UserType,
                    BranchInfo = new BadgeInfoDto
                    {
                        Name = x.Employee.Branch.Name,
                        Color = x.Employee.Branch.Color
                    },
                    JobPositionInfo = new BadgeInfoDto
                    {
                        Name = x.Employee.JobPosition.Name,
                        Color = x.Employee.JobPosition.Color
                    },
                    LevelInfo = new BadgeInfoDto
                    {
                        Name = x.Employee.Level.Name,
                        Color = x.Employee.Level.Color
                    },
                    PayslipBranchInfo = new BadgeInfoDto
                    {
                        Name = branches.ContainsKey(x.BranchId)? branches[x.BranchId].Name: null,
                        Color = branches.ContainsKey(x.BranchId) ? branches[x.BranchId].Color : null,
                    },
                    PayslipLevelInfo = new BadgeInfoDto
                    {
                        Name = levels.ContainsKey(x.LevelId)? levels[x.LevelId].Name: null,
                        Color = levels.ContainsKey(x.LevelId) ? levels[x.LevelId].Color : null,
                    },
                    PayslipJobPositionInfo = new BadgeInfoDto
                    {
                        Name = jobPositions.ContainsKey(x.JobPositionId) ? jobPositions[x.JobPositionId].Name: null,
                        Color = jobPositions.ContainsKey(x.JobPositionId)? jobPositions[x.JobPositionId].Color: null
                    },
                    RealSalary = x.Salary,
                    UpdatedTime = x.LastModificationTime,
                    UpdatedUser = x.LastModifierUser.FullName,
                    ApplyMonth = x.Payroll.ApplyMonth,
                    RemainLeaveDayBefore = x.RemainLeaveDayBefore,
                    RemainLeaveDayAfter = x.RemainLeaveDayAfter,
                    Teams = x.Employee.EmployeeTeams.Select(s => new EmployeeTeamDto
                    {
                        TeamId = s.Team.Id,
                        TeamName = s.Team.Name
                    }).ToList(),

                    StandardSalary = (List<StandardSalaryDto>)(payslipSalaries.ContainsKey(x.Id) ? payslipSalaries[x.Id] : null),
                    NormalSalary = payslipDetail.ContainsKey(new { Type = PayslipDetailType.SalaryNormal,Id = x.Id }) ? payslipDetail[new { Type = PayslipDetailType.SalaryNormal, Id = x.Id }] : 0,
                    OTSalary = payslipDetail.ContainsKey(new { Type = PayslipDetailType.SalaryOT, Id = x.Id }) ? payslipDetail[new { Type = PayslipDetailType.SalaryOT, Id = x.Id }] : 0,
                    Benefit = payslipDetail.ContainsKey(new { Type = PayslipDetailType.Benefit, Id = x.Id }) ? payslipDetail[new { Type = PayslipDetailType.Benefit, Id = x.Id }] : 0,
                    Punishment = payslipDetail.ContainsKey(new { Type = PayslipDetailType.Punishment, Id = x.Id }) ? payslipDetail[new { Type = PayslipDetailType.Punishment, Id = x.Id }] : 0,
                    Bonus = payslipDetail.ContainsKey(new { Type = PayslipDetailType.Bonus, Id = x.Id }) ? payslipDetail[new { Type = PayslipDetailType.Bonus, Id = x.Id }] : 0,
                    Debt = payslipDetail.ContainsKey(new { Type = PayslipDetailType.Debt, Id = x.Id }) ? payslipDetail[new { Type = PayslipDetailType.Debt, Id = x.Id }] : 0,
                    PayrollInfo = new PayrollInfoDto { PayrollId = x.Payroll.Id, PayrollStatus = x.Payroll.Status}
                });;
            return payslips;
        }

        public List<StandardSalaryDto> GetPayslipSalary( long payslipId)
        {
            var query = WorkScope.GetAll<PayslipSalary>()
              .Where(x => x.PayslipId == payslipId)
              .Select(x => new StandardSalaryDto
              {
                  Date = x.Date,
                  Salary = x.Salary
              }).ToList();
            return query;
        }


        public List<EmployeeSalaryHistoryDto> GetAllEmployeeSalaryHistory(long employeeId)
        {
            var query = QueryAllSalaryHistory(employeeId)
                .ToList();

            var initial = query.LastOrDefault();
            if (initial != null)
            {
                initial.IsNotAllowToDelete = true;
            }
            return query;
        }

        public List<EmployeePayslipHistoryDto> GetAllEmployeePayslipHistory(long employeeId)
        {
            return QueryAllPayslipHistory(employeeId)
                .OrderByDescending(x=> x.ApplyMonth)
                .ToList();
        }
        public List<EmployeeBranchHistoryDto> GetAllEmployeeBranchHistory(long employeeId)
        {
            var query = QueryAllBranchHistory()
                .Where(x => x.EmployeeId == employeeId)
                .ToList();

            var initial = query.LastOrDefault();

            if (initial != null)
            {
                initial.IsNotAllowToDelete = true;
            }
            return query;
        }

        public List<EmployeeWorkingHistoryDto> GetAllEmployeeWorkingHistory(long employeeId)
        {
            var query = QueryAllWorkingHistory()
                .Where(x => x.EmployeeId == employeeId)
                .ToList();

            var initial = query.LastOrDefault();

            if(initial != null)
            {
                initial.IsNotAllowToDelete = true;
            }
            return query;
        }

        public void CreateWorkingHistory(CreateWorkingHistoryDto input)
        {
            var entity = ObjectMapper.Map<EmployeeWorkingHistory>(input);
            WorkScope.InsertAsync(entity);
        }

        public void CreateBranchHistory(CreateBranchHistoryDto input)
        {
            var entity = ObjectMapper.Map<EmployeeBranchHistory>(input);
            WorkScope.InsertAsync(entity);
        }


        public async Task<long> DeleteSalaryHistory(long id)
        {
            var salaryChangeRequestEmployee = WorkScope.GetAll<SalaryChangeRequestEmployee>()
                             .Where(s => s.Id == id)
                             .Select(s => new {
                                 s.Type,
                                 s.EmployeeId,
                             }).FirstOrDefault();
            if (salaryChangeRequestEmployee == default) { throw new UserFriendlyException("Not found SalaryChangeRequestEmployee with Id {id}"); }

            if (salaryChangeRequestEmployee.Type == SalaryRequestType.Initial)
            {
                throw new UserFriendlyException("Can't delete change request employee with type Initial");
            }
            var latestRequestId = WorkScope.GetAll<SalaryChangeRequestEmployee>()
                    .Where(x => x.EmployeeId == salaryChangeRequestEmployee.EmployeeId)
                    .OrderByDescending(x => x.ApplyDate)
                    .ThenByDescending(x => x.CreationTime)
                    .Select(x=> x.Id)
                    .FirstOrDefault();

            await DeleteEmployeeContractByChangeRequestEmployeeId(id);
            await WorkScope.DeleteAsync<SalaryChangeRequestEmployee>(id);
            
            if(latestRequestId == id)
            {
                await UpdateEmployeeWhenDeleteSalaryHistory(id, salaryChangeRequestEmployee.EmployeeId);
            }
            return id;

        }

        public async Task DeleteEmployeeContractByChangeRequestEmployeeId(long requestEmployeeId)
        {
            var  employeeContract = WorkScope.GetAll<EmployeeContract>()
                .Where(x=> x.SalaryRequestEmployeeId == requestEmployeeId)
                .FirstOrDefault();
            if(employeeContract != default)
            {
                await WorkScope.DeleteAsync<EmployeeContract>(employeeContract.Id);
            }
        }

        public async Task UpdateEmployeeWhenDeleteSalaryHistory(long requestId, long employeeId)
        {
            var request = WorkScope.GetAll<SalaryChangeRequestEmployee>()
                .Where(x => x.EmployeeId == employeeId && x.Id != requestId)
                .OrderByDescending(x => x.ApplyDate)
                .ThenByDescending(x => x.CreationTime)
                .FirstOrDefault();

            if(request != default)
            {
                var employeeContract = WorkScope.GetAll<EmployeeContract>()
                .Where(x => x.EmployeeId == employeeId && x.SalaryRequestEmployeeId != requestId)
                .OrderByDescending(x=> x.StartDate)
                .ThenByDescending(x=> x.CreationTime)
                .FirstOrDefault();

                var employee = WorkScope.GetAsync<Employee>(employeeId).Result;
                employee.JobPositionId = request.ToJobPositionId;
                employee.LevelId = request.ToLevelId;
                employee.UserType = request.ToUserType;
                employee.RealSalary = request.ToSalary;
                if (request.Type == SalaryRequestType.StopWorking)
                {
                    employee.ProbationPercentage = 0;
                    employee.Salary = 0;
                }
                else
                {
                    employee.ProbationPercentage = employeeContract.ProbationPercentage;
                    employee.Salary = employeeContract.BasicSalary;

                }
                

                await WorkScope.UpdateAsync(employee);

            }
            else { throw new UserFriendlyException("Can't found salary change request employee to update employee"); }

        }


        public async Task<long> DeleteBranchHistory(long id)
        {
            await ValidDeleteEmployeeBranchHistory(id);
            await WorkScope.DeleteAsync<EmployeeBranchHistory>(id);
            return id;

        }

        public async Task<long> DeleteWorkingHistory(long id , long employeeId)
        {
            await ValidDeleteEmployeeWorkingHistory(id);

            var latestHistory = WorkScope.GetAll<EmployeeWorkingHistory>()
                .Where(x => x.EmployeeId == employeeId)
                .OrderByDescending(x => x.DateAt)
                .ThenByDescending(x => x.CreationTime)
                .FirstOrDefault();

            await WorkScope.DeleteAsync<EmployeeWorkingHistory>(id);

            if (latestHistory.Id == id)
            {
                await UpdateEmployeeStatus(employeeId, id);
            }
            
            return id;

        }



        public async Task<long> UpdateEmployeeStatus(long employeeId, long id)
        {
            var histories = GetAllEmployeeWorkingHistory(employeeId)
               .Where(x=> x.Id != id)
               .OrderByDescending(x => x.DateAt)
               .FirstOrDefault();
            var employee = WorkScope.GetAsync<Employee>(employeeId).Result;
            employee.Status = histories.Status;
            await WorkScope.UpdateAsync(employee);

            var inputToUpdate = new InputToUpdateUserStatusDto()
            {
                EmailAddress = GetEmployeeEmailById(employeeId),
                DateAt = histories.DateAt,

            };
            switch (histories.Status)
            {
                case EmployeeStatus.Quit:
                    {
                        _changeEmployeeWorkingStatusManager.ConfirmUserQuit(inputToUpdate);
                        break;
                    }
                case EmployeeStatus.Working:
                    {
                        _changeEmployeeWorkingStatusManager.ConfirmUserBackToWork(inputToUpdate);
                        break;
                    }
                case EmployeeStatus.Pausing:
                    {
                        _changeEmployeeWorkingStatusManager.ConfirmUserPause(inputToUpdate);
                        break;
                    }
                case EmployeeStatus.MaternityLeave:
                    {
                        _changeEmployeeWorkingStatusManager.ConfirmUserMaternityLeave(inputToUpdate);
                        break;
                    }
            }
            return employeeId;
        }

        private string GetEmployeeEmailById(long employeeId)
        {
            var employeeEmail = WorkScope.GetAll<Employee>()
                .Where(x => x.Id == employeeId)
                .Select(x => x.Email)
                .FirstOrDefault();
            return employeeEmail;
        }

        public async Task<UpdateNoteBranchHistoryDto> UpdateNoteInBranchHistory(UpdateNoteBranchHistoryDto input)
        {
            var entity = WorkScope.GetAsync<EmployeeBranchHistory>(input.Id).Result;
            if (entity == null)
            {
                throw new UserFriendlyException("There is no employee branch history with Id = {id}");
            };

            entity.Note = input.Note;
            await WorkScope.UpdateAsync(entity);
            return input;

        }

        public async Task<UpdateNoteWorkingHistoryDto> UpdateNoteInWorkingHistory(UpdateNoteWorkingHistoryDto input)
        {
            var entity = WorkScope.GetAsync<EmployeeWorkingHistory>(input.Id).Result;
            if (entity == null)
            {
                throw new UserFriendlyException("There is no employee working history with Id = {id}");
            };

            entity.Note = input.Note;
            await WorkScope.UpdateAsync(entity);
            return input;

        }

        public async Task<UpdateDateWorkingHistoryDto> UpdateDateInWorkingHistory(UpdateDateWorkingHistoryDto input)
        {
            var ewh = WorkScope.GetAsync<EmployeeWorkingHistory>(input.Id).Result;
            if (ewh == default)
            {
                throw new UserFriendlyException($"There is no employee working history with Id = {input.Id}");
            };

            await UpdateDateToSalaryChangeRequestEmployee(ewh.DateAt, ewh.EmployeeId, input);
            ewh.DateAt = input.DateAt;
            await WorkScope.UpdateAsync(ewh);
            return input;

        }
        private async Task UpdateDateToSalaryChangeRequestEmployee(DateTime dateAt, long employeeId, UpdateDateWorkingHistoryDto input)
        {
            var scre = WorkScope.GetAll<SalaryChangeRequestEmployee>()
                .Where(x => x.EmployeeId == employeeId)
                .Where(x => x.ApplyDate.Date == dateAt.Date)
                .FirstOrDefault();

            if (scre != default)
            {
                scre.ApplyDate = input.DateAt;
                await WorkScope.UpdateAsync(scre);
                await UpdateDateToEmployeeContract(input.DateAt, scre.Id);
            };

        }
        private async Task UpdateDateToEmployeeContract(DateTime dateAt,long SalaryRequestEmployeeId)
        {
            var empContract = WorkScope.GetAll<EmployeeContract>()
                .Where(x=> x.SalaryRequestEmployeeId == SalaryRequestEmployeeId)
                .FirstOrDefault();

            if (empContract != default)
            {
                empContract.StartDate = dateAt;
                await WorkScope.UpdateAsync(empContract);
            };
            
        }

        public async Task<UpdateNoteWorkingHistoryDto> UpdateNoteInSalaryHistory(UpdateNoteWorkingHistoryDto input)
        {
            var entity = WorkScope.GetAsync<SalaryChangeRequestEmployee>(input.Id).Result;
            if (entity == null)
            {
                throw new UserFriendlyException("There is no employee salary history with Id = {id}");
            };

            entity.Note = input.Note;
            await WorkScope.UpdateAsync(entity);
            return input;

        }

        private async Task ValidDeleteEmployeeBranchHistory(long id)
        {
            var entity = await WorkScope.GetAsync<EmployeeBranchHistory>(id);
            if(entity == null)
            {
                throw new UserFriendlyException("There is no employee branch history with Id = {id}");
            }

        }

        private async Task ValidDeleteEmployeeWorkingHistory(long id)
        {
            var entity = await WorkScope.GetAsync<EmployeeWorkingHistory>(id);
            if (entity == null)
            {
                throw new UserFriendlyException("There is no employee working history with Id = {id}");
            }


        }
    }
}
