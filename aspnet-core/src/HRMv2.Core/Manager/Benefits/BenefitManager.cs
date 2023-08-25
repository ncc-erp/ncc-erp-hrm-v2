using Abp.UI;
using HRMv2.Entities;
using HRMv2.Manager.Benefits.Dto;
using HRMv2.Manager.Categories.Benefits.Dto;
using HRMv2.Manager.Common.Dto;
using HRMv2.Manager.Employees;
using HRMv2.Manager.Employees.Dto;
using HRMv2.NccCore;
using Microsoft.EntityFrameworkCore;
using NccCore.Extension;
using NccCore.Paging;
using NccCore.Uitls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HRMv2.Constants.Enum.HRMEnum;

namespace HRMv2.Manager.Categories.Benefits
{
    public class BenefitManager : BaseManager
    {

        public BenefitManager(IWorkScope workScope) : base(workScope)
        {
            
        }

        public IQueryable<GetBenefitDto> QueryAllBenefit()
        {
            return WorkScope.GetAll<Benefit>()
                .OrderByDescending(x => x.CreationTime)
                .Select(x => new GetBenefitDto
            {
                Id = x.Id,
                Name = x.Name,
                IsActive = x.IsActive,
                IsBelongToAllEmployee = x.IsBelongToAllEmployee,
                Money = x.Money,
                Type = x.Type,
                ApplyDate = x.ApplyDate,
                UpdatedUser = x.LastModifierUser == null ? string.Empty : x.LastModifierUser.FullName,
                UpdatedTime = (DateTime)x.LastModificationTime,
                CreatorUser = x.CreatorUser == null ? string.Empty : x.CreatorUser.FullName,
                CreationTime = x.CreationTime
            });
        }

        public IQueryable<GetbenefitEmployeeDto> QueryAllBenefitEmployee()
        {
            var employeeStatus = WorkScope.GetAll<EmployeeWorkingHistory>()
                .GroupBy(x => x.EmployeeId)
                .Select(x => new 
                {
                    x.Key,
                    WorkingStatus = new BEWorkingStatus
                    {
                        Status = x.OrderBy(s => s.DateAt).Select(s => s.Status).LastOrDefault(),
                        DateAt = x.OrderBy(s => s.DateAt).Select(s => s.DateAt).LastOrDefault()
                    }
                }).ToDictionary(x => x.Key, x => x.WorkingStatus);

            return WorkScope.GetAll<BenefitEmployee>()
                .OrderByDescending(x => x.CreationTime)
                .Select(x => new GetbenefitEmployeeDto
                {

                    Id = x.Id,
                    EmployeeId = x.EmployeeId,
                    BenefitId = x.BenefitId,
                    Avatar = x.Employee.Avatar,
                    BranchInfo = new BadgeInfoDto
                    {
                        Name = x.Employee.Branch.Name,
                        Color = x.Employee.Branch.Color,
                    },
                    BranchId = x.Employee.BranchId,
                    JobPositionId = x.Employee.JobPositionId,
                    Sex = x.Employee.Sex,
                    LevelId = x.Employee.LevelId,
                    UserType = x.Employee.UserType,
                    Email = x.Employee.Email,
                    FullName = x.Employee.FullName,
                    Status = x.Employee.Status,
                    Skills = x.Employee.EmployeeSkills.Select(s => new EmployeeSkillDto
                    {
                        SkillId = s.Skill.Id,
                        SkillName = s.Skill.Name
                    }).ToList(),
                    Teams = x.Employee.EmployeeTeams.Select(s => new EmployeeTeamDto
                    {
                        TeamId = s.Team.Id,
                        TeamName = s.Team.Name
                    }).ToList(),
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
                    StartDate = x.StartDate,
                    EndDate = x.EndDate,
                    UpdatedUserName = x.LastModifierUser.FullName,
                    UpdatedTime = (DateTime)x.LastModificationTime,
                    WorkingStatus = employeeStatus.ContainsKey(x.EmployeeId) ? employeeStatus[x.EmployeeId] : null
                });
        }

        public async Task<AddEmployeeToBenefitDto> AddEmployeeToBenefit(AddEmployeeToBenefitDto input)
        {
            var currentEmployeeIds = QueryAllBenefitEmployee()
                .Where(x => x.BenefitId == input.BenefitId)
                .Select(x => x.Id);
            var listToInsert = new List<BenefitEmployee>();
            foreach (var employeeId in input.ListEmployeeId)
            {
                if (!currentEmployeeIds.Contains(employeeId))
                {
                    DateTime employeeWorkingDate = WorkScope.GetAll<Employee>()
                        .Where(x => x.Id == employeeId)
                        .Select(x => x.StartWorkingDate.Date)
                        .FirstOrDefault();

                    var entity = new BenefitEmployee
                    {
                        EmployeeId = employeeId,
                        StartDate = input.StartDate != null ? (DateTime)input.StartDate : employeeWorkingDate,
                        EndDate = input.EndDate.HasValue ? (DateTime)input.EndDate : null ,
                        BenefitId = input.BenefitId,
                        LastModificationTime = DateTimeUtils.GetNow(),
                        LastModifierUserId = AbpSession.UserId
                    };
                    listToInsert.Add(entity);
                }
            }
            await WorkScope.InsertRangeAsync(listToInsert);
            return input;
        }

        public async Task CloneBenefit(CloneBenefitDto input)
        {
            var EmployeesToClone = QueryAllBenefitEmployee()
                .Where(x => x.BenefitId == input.BenefitId)
                .Select(x => new {
                    EmployeeId = x.EmployeeId,
                    StartDate = x.StartDate,
                    EndDate = x.EndDate
                }).ToList();

            var entity = ObjectMapper.Map<Benefit>(input);
            entity.IsActive = true;
            var insertedBenefitId = await WorkScope.InsertAndGetIdAsync(entity);

            if (input.IsCloneEmployee)
            {
                List<BenefitEmployee> EmployeeeToAdd = new();
                foreach (var employee in EmployeesToClone)
                {
                    var benefitEmployee = new BenefitEmployee
                    {
                        EmployeeId = employee.EmployeeId,
                        BenefitId = insertedBenefitId,
                        StartDate = employee.StartDate,
                        EndDate = employee.EndDate
                    };
                    EmployeeeToAdd.Add(benefitEmployee);
                }
                await WorkScope.InsertRangeAsync(EmployeeeToAdd);
            }
        }

        public async Task<QuickAddEmployeeDto> QuickAddEmployee(QuickAddEmployeeDto input)
        {
            var currentEmployeeIds = QueryAllBenefitEmployee()
                .Where(x => x.BenefitId == input.BenefitId)
                .Select(x => x.EmployeeId);
            if (currentEmployeeIds.Contains(input.EmployeeId))
            {
                throw new UserFriendlyException("Employee is already exist in Benenefit");
            }

            DateTime employeeWorkingDate = WorkScope.GetAll<Employee>()
                       .Where(x => x.Id == input.EmployeeId)
                       .Select(x => x.StartWorkingDate.Date)
                       .FirstOrDefault();

            var entity = new BenefitEmployee
            {
                EmployeeId = input.EmployeeId,
                StartDate = input.StartDate != null ? (DateTime)input.StartDate : employeeWorkingDate,
                EndDate = input.EndDate.HasValue ? (DateTime)input.EndDate : null ,
                BenefitId = input.BenefitId,
            };
            await WorkScope.InsertAsync(entity);
            return input;
        }

        public List<GetBenefitDto> GetAll()
        {
            return QueryAllBenefit().ToList();
        }
        public async Task<GridResult<GetBenefitDto>> GetAllPaging(GridParam input)
        {
            var queryCountemployee = await WorkScope.GetAll<BenefitEmployee>()
                .GroupBy(x => x.BenefitId)
                .Select(x => new { x.Key, CountEmployee = x.Count() })
                .ToDictionaryAsync(x => x.Key, x => x.CountEmployee);
            var query = QueryAllBenefit()
                .Select(x => new GetBenefitDto
            {
                Id = x.Id,
                Name = x.Name,
                IsActive = x.IsActive,
                IsBelongToAllEmployee = x.IsBelongToAllEmployee,
                Money = x.Money,
                Type = x.Type,
                ApplyDate = x.ApplyDate,
                UpdatedTime = x.UpdatedTime,
                UpdatedUser = x.UpdatedUser,
                CreationTime = x.CreationTime,
                CreatorUser = x.CreatorUser,
                UserCount = queryCountemployee.ContainsKey(x.Id) ? queryCountemployee[x.Id] : 0
            });
            return await query.GetGridResult(query, input);
        }


        public async Task<GridResult<GetbenefitEmployeeDto>> GetEmployeeInBenefitPaging(long benefitId, GetbenefitEmployeeInputDto input)
        {
            var query = QueryAllBenefitEmployee()
                .Where(x => x.BenefitId == benefitId);


            if (input.StatusIds != null && input.StatusIds.Count == 1) query = query.Where(x => input.StatusIds[0] == x.Status);
            else if (input.StatusIds != null && input.StatusIds.Count > 1) query = query.Where(x => input.StatusIds.Contains(x.Status));

            if (input.BranchIds != null && input.BranchIds.Count == 1) query = query.Where(x => input.BranchIds[0] == x.BranchId);
            else if (input.BranchIds != null && input.BranchIds.Count > 1) query = query.Where(x => input.BranchIds.Contains(x.BranchId));

            if (input.UserTypes != null && input.UserTypes.Count == 1) query = query.Where(x => input.UserTypes[0] == x.UserType);
            else if (input.UserTypes != null && input.UserTypes.Count > 1) query = query.Where(x => input.UserTypes.Contains(x.UserType));

            if (input.LevelIds != null && input.LevelIds.Count == 1) query = query.Where(x => input.LevelIds[0] == x.LevelId);
            else if (input.LevelIds != null && input.LevelIds.Count > 1) query = query.Where(x => input.LevelIds.Contains(x.LevelId));

            if (input.JobPositionIds != null && input.JobPositionIds.Count == 1) query = query.Where(x => input.JobPositionIds[0] == x.JobPositionId);
            else if (input.JobPositionIds != null && input.JobPositionIds.Count > 1) query = query.Where(x => input.JobPositionIds.Contains(x.JobPositionId));



            
            if (input.TeamIds == null || input.TeamIds.Count == 0)
            {
                return await query.GetGridResult(query, input.GridParam);
            }

            if (input.TeamIds.Count == 1 || !input.IsAndCondition)
            {
                var employeeHaveAnyTeams = QueryEmployeeHaveAnyTeams(input.TeamIds).Distinct();

                query = from employee in query
                        join employeeId in employeeHaveAnyTeams on employee.EmployeeId equals employeeId
                        select employee;
                return await query.GetGridResult(query, input.GridParam);

            }

            var employeeIds = QueryEmployeeHaveAllTeams(input.TeamIds).Result;

            query = query.Where(s => employeeIds.Contains(s.EmployeeId));

            return await query.GetGridResult(query, input.GridParam);
        }


        public List<long> GetListEmployeeIdInBenefit(long benefitId)
        {
            return WorkScope.GetAll<BenefitEmployee>()
                .Where(x => x.BenefitId == benefitId)
                .Select(x => x.EmployeeId).ToList();
        }

        public List<GetEmployeeDto> GetAllEmployeeNotInBenefit(long benefitId)
        {
            var employeeIdsInBenefit = GetListEmployeeIdInBenefit(benefitId);
            
            var query = WorkScope.GetAll<Employee>()
                .Select(x => new GetEmployeeDto
                {
                    Id = x.Id,
                    Status = x.Status,
                    FullName = x.FullName,
                    Avatar = x.Avatar,
                    Sex = x.Sex,
                    Email = x.Email,
                    BranchId = x.BranchId,
                    LevelId = x.LevelId,
                    UserType = x.UserType,
                    Skills = x.EmployeeSkills.Select(s => new EmployeeSkillDto
                    {
                        SkillId = s.Skill.Id,
                        SkillName = s.Skill.Name
                    }).ToList(),
                    Teams = x.EmployeeTeams.Select(s => new EmployeeTeamDto
                    {
                        TeamId = s.Team.Id,
                        TeamName = s.Team.Name
                    }).ToList(),
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
                })
                .Where(x=> !employeeIdsInBenefit.Contains(x.Id))
                .ToList();

            return query;
        }

        public async Task<GridResult<GetBenefitsOfEmployeeDto>> GetBenefitByEmployeeId(long id, GridParam input)
        {
            var query =  WorkScope.GetAll<BenefitEmployee>()
                .Where(x => x.EmployeeId == id)
                .Select(x => new GetBenefitsOfEmployeeDto
                {
                    Id = x.Id,
                    BenefitId = x.Benefit.Id,
                    BenefitName = x.Benefit.Name,
                    BenefitType = x.Benefit.Type,
                    Status = x.Benefit.IsActive,
                    StartDate = x.StartDate,
                    EndDate = x.EndDate,
                    Money = x.Benefit.Money,
                });
            return await query.GetGridResult(query, input);

        }

        public async Task<List<DateTime>> GetListMonthFilter()
        {
            var query = await WorkScope.GetAll<Benefit>()
                .Select(x => x.ApplyDate)
                .Distinct()
                .OrderByDescending(x => x)
                .ToListAsync();
            return query;
        }

        public GetBenefitDto Get(long id)
        {
            return QueryAllBenefit()
            .Where(x => x.Id == id)
            .FirstOrDefault();
        }

        public async Task<BenefitDto> Create(BenefitDto input)
        {
            await ValidCreate(input);
            var entity = ObjectMapper.Map<Benefit>(input);
            entity.IsActive = true;
            input.Id = await WorkScope.InsertAndGetIdAsync(entity);
            return input;
        }

        public async Task<BenefitDto> Update(BenefitDto input)
        {
            await ValidUpdate(input);
            var entity = await WorkScope.GetAsync<Benefit>(input.Id);
            ObjectMapper.Map(input, entity);

            await WorkScope.UpdateAsync(entity);
            return input;
        }
        public async Task<UpdateBEDto> UpdateBenefitEmployee(UpdateBEDto input)
        {
            var entity = await WorkScope.GetAsync<BenefitEmployee>(input.Id);
            ObjectMapper.Map(input, entity);
            await WorkScope.UpdateAsync(entity);
            return input;
        }

        public async Task<long> Delete(long id)
        {
            await ValidDelete(id);
            await WorkScope.DeleteAsync<Benefit>(id);

            return id;
        }

        public async Task<Benefit> ChangeStatus(UpdateBenefitStatusDto input)
        {
            var benefit = WorkScope.GetAll<Benefit>()
                .Where(x => x.Id == input.Id).FirstOrDefault();
            benefit.IsActive = input.IsActive;
            await WorkScope.UpdateAsync(benefit);
            return benefit;
        }

        public async Task<long> RemoveEmployeeFromBenefit(long id)
        {
            await WorkScope.DeleteAsync<BenefitEmployee>(id);
            return id;
        }

        public async Task<long> DeleteAllBenefitOfEmployee(long employeeId)
        {
            var employeeBenefitIds = WorkScope.GetAll<BenefitEmployee>()
               .Where(x => x.EmployeeId == employeeId)
               .Select(x => x.Id)
               .ToList();
            foreach (var id in employeeBenefitIds)
            {
                await RemoveEmployeeFromBenefit(id);
            };
            return employeeId;
        }

        public UpdateEmployeeDateDto UpdateAllStartDate(UpdateEmployeeDateDto input)
        {
            var currentEmoloyees = WorkScope.GetAll<BenefitEmployee>()
                .Where(x => x.BenefitId == input.BenefitId);
            foreach (var employee in currentEmoloyees)
            {
                employee.StartDate = input.Date;
            }
            CurrentUnitOfWork.SaveChanges();
            return input;
        }

        public UpdateEmployeeEndDateDto UpdateAllEndDate(UpdateEmployeeEndDateDto input)
        {
            var currentEmoloyees = WorkScope.GetAll<BenefitEmployee>()
                .Where(x => x.BenefitId == input.BenefitId);
            foreach (var employee in currentEmoloyees)
            {
                employee.EndDate = input.Date.HasValue ? input.Date : null;
            }
            CurrentUnitOfWork.SaveChanges();
            return input;
        }

        private async Task ValidCreate(BenefitDto input)
        {
            var isExist = await WorkScope.GetAll<Benefit>()
                .AnyAsync(x => x.Name.Trim() == input.Name.Trim());
            if (isExist)
            {
                throw new UserFriendlyException($"Name is Already Exist");
            }
        }

        private async Task ValidUpdate(BenefitDto input)
        {
            var isExist = await WorkScope.GetAll<Benefit>()
                .AnyAsync(x => x.Id != input.Id && x.Name == input.Name);
            if (isExist)
            {
                throw new UserFriendlyException($"Name is Already Exist");
            }
        }

        private async Task ValidDelete(long id)
        {
            var entity = await WorkScope.GetAsync<Benefit>(id);
            if (entity == null)
            {
                throw new UserFriendlyException($"There is no Benefit with id {id}");
            }
            var hasNameBenefitEmployee = await WorkScope.GetAll<BenefitEmployee>()
                .Where(x => x.BenefitId == id).Select(s => s.Benefit.Name)
                .FirstOrDefaultAsync();
            if(hasNameBenefitEmployee != default)
            {
                throw new UserFriendlyException($"Benefit {hasNameBenefitEmployee} has benefit employee");
            }
        }

        public List<GetBenefitsOfEmployeeDto> GetAllBenefitsByEmployeeId(long id)
        {
            var query = WorkScope.GetAll<BenefitEmployee>()
                .Where(x => x.EmployeeId == id)
                .Where(x=> x.Benefit.IsActive)
                .Where(x=> x.Benefit.Type != BenefitType.CheDoChung)
                .Select(x => new GetBenefitsOfEmployeeDto
                {
                    Id = x.Id,
                    BenefitId = x.Benefit.Id,
                    BenefitName = x.Benefit.Name,
                    BenefitType = x.Benefit.Type,
                    Status = x.Benefit.IsActive,
                    StartDate = x.StartDate,
                    EndDate = x.EndDate,
                    Money = x.Benefit.Money,
                }).ToList();
            return query;
        }
    }
}
