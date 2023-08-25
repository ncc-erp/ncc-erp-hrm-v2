using Abp.UI;
using HRMv2.Entities;
using HRMv2.Manager.Common.Dto;
using HRMv2.Manager.Employees;
using HRMv2.Manager.Employees.Dto;
using HRMv2.Manager.Punishments.Dto;
using HRMv2.Manager.Refunds.Dto;
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

namespace HRMv2.Manager.Refunds
{
    public class RefundManager : BaseManager
    {
        private readonly EmployeeManager _employeeManager;

        public RefundManager(IWorkScope workScope, EmployeeManager employeeManager) : base(workScope)
        {
            _employeeManager = employeeManager;
        }

        public IQueryable<GetRefundDto> QueryAllRefund()
        {
            return WorkScope.GetAll<Refund>()
                .OrderByDescending(x => x.Date)
                .Select(x => new GetRefundDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    Date = x.Date,
                    IsActive = x.IsActive
                });
        }

        public IQueryable<GetRefundEmployeeDto> QueryAllRefundEmployee()
        {
            return WorkScope.GetAll<RefundEmployee>()
                .Select(x => new GetRefundEmployeeDto
                {
                    Id = x.Id,
                    Money = x.Money,
                    Note = x.Note,
                    EmployeeId = x.EmployeeId,
                    RefundId = x.RefundId,
                    UpdatedTime = x.LastModificationTime,
                    UpdatedUser = x.LastModifierUser != null ? x.LastModifierUser.FullName : "",
                    FullName = x.Employee.FullName,
                    BranchInfo = new BadgeInfoDto
                    {
                        Name = x.Employee.Branch.Name,
                        Color = x.Employee.Branch.Color
                    },
                    LevelInfo = new BadgeInfoDto
                    {
                        Name = x.Employee.Level.Name,
                        Color = x.Employee.Level.Color
                    },

                    JobPositionInfo = new BadgeInfoDto
                    {
                        Name = x.Employee.JobPosition.Name,
                        Color = x.Employee.JobPosition.Color
                    },
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
                    JobPositionId = x.Employee.JobPositionId,
                    Avatar = x.Employee.Avatar,
                    Sex = x.Employee.Sex,
                    Email = x.Employee.Email,
                    BranchId = x.Employee.BranchId,
                    LevelId = x.Employee.LevelId,
                    UserType = x.Employee.UserType,
                    Status = x.Employee.Status,
                });
        }

        public List<GetRefundDto> GetAll()
        {
            return QueryAllRefund().ToList();
        }
        public GetRefundDto Get(long id)
        {
            return QueryAllRefund()
                .Where(x => x.Id == id)
                .FirstOrDefault();
        }

        public List<GetEmployeeBasicInfoDto> GetAllEmployeeNotInRefund(long id)
        {
            var employeeIdInRefund = WorkScope.GetAll<RefundEmployee>()
                             .Where(p => p.RefundId == id)
                             .Select(p => p.EmployeeId).ToList();

            var query = _employeeManager.GetAllEmployeeBasicInfo()
                .Where(p => !employeeIdInRefund.Contains(p.Id))
                .ToList();

            return query;
        }

        public async Task<long> ActiveRefund(long id)
        {
            var refund = WorkScope.GetAll<Refund>()
                .Where(x => x.Id == id)
                .FirstOrDefault();

            if(refund == default)
            {
                throw new UserFriendlyException($"can't find refund with id {id}");
            }

            refund.IsActive = true;

            await WorkScope.UpdateAsync(refund);

            return id;
        }

        public async Task<long> DeActiveRefund(long id)
        {
            var refund = WorkScope.GetAll<Refund>()
                .Where(x => x.Id == id)
                .FirstOrDefault();

            if (refund == default)
            {
                throw new UserFriendlyException($"can't find refund with id {id}");
            }

            refund.IsActive = false;

            await WorkScope.UpdateAsync(refund);

            return id;
        }

        public async Task<GridResult<GetRefundDto>> GetAllPaging(GridParam input)
        {
            var query = QueryAllRefund();

            var dicRefundEmployee = WorkScope.GetAll<RefundEmployee>()
               .GroupBy(x => x.RefundId)
               .Select(x => new { x.Key, EmployeeCount = x.Count(), TotalMoney = x.Sum(x => x.Money) })
               .ToDictionary(x => x.Key, x => new
               {
                   x.EmployeeCount,
                   x.TotalMoney
               });

            var result = await query.GetGridResult(query, input);

            foreach (var item in result.Items)
            {
                if (dicRefundEmployee.ContainsKey(item.Id))
                {
                    item.EmployeeCount = dicRefundEmployee[item.Id].EmployeeCount;
                    item.TotalMoney = dicRefundEmployee[item.Id].TotalMoney;
                }
            }

            return result;
        }



        public async Task<CreateRefundDto> Create(CreateRefundDto input)
        {
            await ValidCreate(input);

            var entity = ObjectMapper.Map<Refund>(input);

            entity.IsActive = true;

            await WorkScope.InsertAsync(entity);

            return input;
        }

        private async Task ValidCreate(CreateRefundDto input)
        {
            var isExist = await WorkScope.GetAll<Refund>()
               .AnyAsync(x => x.Name.Trim() == input.Name.Trim());

            if (isExist)
            {
                throw new UserFriendlyException("Name is already exist!");
            }
        }

        private async Task ValidUpdate(UpdateRefundDto input)
        {
            var isExist = await WorkScope.GetAll<Refund>()
                .AnyAsync(x => x.Id != input.Id && x.Name.Trim() == input.Name.Trim());

            if (isExist)
            {
                throw new UserFriendlyException($"This refund name is already exist");
            }
        }

        public async Task<UpdateRefundDto> Update(UpdateRefundDto input)
        {
            await ValidUpdate(input);

            var entity = WorkScope.GetAll<Refund>()
                .Where(x => x.Id == input.Id)
                .FirstOrDefault();

            if(entity == default)
            {
                throw new UserFriendlyException($"Can't find refund with id {input.Id}");
            }

            entity = ObjectMapper.Map(input, entity);

            await WorkScope.UpdateAsync(entity);

            if (input.UpdateNote)
            {
                UpdateRefundEmployeesNote(entity.Id, entity.Name);

            }

            return input;
        }

        public async Task<long> Delete(long id)
        {
            var refund = await WorkScope.GetAll<Refund>()
                .Where(x => x.Id == id)
                .FirstOrDefaultAsync();

            if (refund == default)
            {
                throw new UserFriendlyException($"Can't find refund with id {id}");
            }

            refund.IsDeleted = true;

            var refundEmployees = await WorkScope.GetAll<RefundEmployee>()
                .Where(x => x.RefundId == refund.Id)
                .ToListAsync();

            foreach (var re in refundEmployees)
            {
                re.IsDeleted = true;
            }

            CurrentUnitOfWork.SaveChanges();

            return id;
        }

        public List<string> GetListRefundDate()
        {
            return QueryAllRefund()
                .OrderBy(x => x.Date)
                .Select(x => x.Date.ToString("MM/yyyy"))
                .Distinct()
                .ToList();
        }

        public List<DateTime> GetListMonthsForCreate()
        {
            return DateTimeUtils.GetListMonthsForCreate();
        }


        //Refund employees
        public async Task<GridResult<GetRefundEmployeeDto>> GetRefundEmployeesPaging(long id, GetEmployeePunishment input)
        {
            var query = QueryAllRefundEmployee()
                .Where(x => x.RefundId == id);

            if (input.StatusIds != null && input.StatusIds.Count == 1) query = query.Where(x => input.StatusIds[0] == x.Status);
            else if (input.StatusIds != null && input.StatusIds.Count > 1) query = query.Where(x => input.StatusIds.Contains(x.Status));

            if (input.BranchIds != null && input.BranchIds.Count == 1) query = query.Where(x => input.BranchIds[0] == x.BranchId);
            else if (input.BranchIds != null && input.BranchIds.Count > 1) query = query.Where(x => input.BranchIds.Contains(x.BranchId));

            if (input.Usertypes != null && input.Usertypes.Count == 1) query = query.Where(x => input.Usertypes[0] == x.UserType);
            else if (input.Usertypes != null && input.Usertypes.Count > 1) query = query.Where(x => input.Usertypes.Contains(x.UserType));

            if (input.LevelIds != null && input.LevelIds.Count == 1) query = query.Where(x => input.LevelIds[0] == x.LevelId);
            else if (input.LevelIds != null && input.LevelIds.Count > 0) query = query.Where(x => input.LevelIds.Contains(x.LevelId));

            if (input.JobPositionIds != null && input.JobPositionIds.Count == 1) query = query.Where(x => input.JobPositionIds[0] == x.JobPositionId);
            else if (input.JobPositionIds != null && input.JobPositionIds.Count > 0) query = query.Where(x => input.JobPositionIds.Contains(x.JobPositionId));

            if (input.TeamIds == null || input.TeamIds.Count == 0)
            {
                return await query.GetGridResult(query, input.GridParam);
            }

            if (input.TeamIds.Count == 1 || !input.IsAndCondition)
            {
                var employeeHaveAnyTeams = _employeeManager.QueryEmployeeHaveAnyTeams(input.TeamIds).Distinct();

                query = from employee in query
                               join employeeId in employeeHaveAnyTeams on employee.EmployeeId equals employeeId
                               select employee;
                return await query.GetGridResult(query, input.GridParam);

            }

            var employeeIds = _employeeManager.QueryEmployeeHaveAllTeams(input.TeamIds).Result;

            query = query.Where(s => employeeIds.Contains(s.EmployeeId));

            return await query.GetGridResult(query, input.GridParam);
        }

        public async Task<AddEmployeeToRefundDto> AddEmployeeToRefund(AddEmployeeToRefundDto input)
        {
            var entity = ObjectMapper.Map<RefundEmployee>(input);

            await WorkScope.InsertAsync(entity);

            return input;
        }

        private void UpdateRefundEmployeesNote(long refundId, string note)
        {
            var refundEmployees = WorkScope.GetAll<RefundEmployee>()
                .Where(x => x.RefundId == refundId)
                .ToList();

            foreach (var re in refundEmployees)
            {
                re.Note = note;
            }

            CurrentUnitOfWork.SaveChanges();
        }

        public async Task<UpdateRefundemployeeDto> UpdateRefundEmployee(UpdateRefundemployeeDto input)
        {
            var entity = WorkScope.GetAll<RefundEmployee>()
                .Where(x => x.Id == input.Id)
                .FirstOrDefault();

            entity = ObjectMapper.Map(input, entity);

            await WorkScope.UpdateAsync(entity);

            return input;
        }

        public async Task<long> DeleteRefundEmployee(long id)
        {
            var entity = WorkScope.GetAll<RefundEmployee>()
                .Where(x => x.Id == id)
                .FirstOrDefault();

            if(entity == default)
            {
                throw new UserFriendlyException($"Can't find refundEmployee with id {id}");
            }

            await WorkScope.DeleteAsync(entity);

            return id;
        }

    }
}
