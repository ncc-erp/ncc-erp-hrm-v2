using Abp.Authorization;
using Abp.UI;
using HRMv2.Entities;
using HRMv2.Manager.Categories.Teams.Dto;
using HRMv2.NccCore;
using Microsoft.EntityFrameworkCore;
using NccCore.Extension;
using NccCore.Paging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HRMv2.Constants.Enum.HRMEnum;

namespace HRMv2.Manager.Categories.Teams
{
    public class TeamManager : BaseManager
    {
        public TeamManager(IWorkScope workScope) : base(workScope)
        {
        }

        public IQueryable<TeamDto> QueryAllTeam()
        {
            return WorkScope.GetAll<Team>().Select(x => new TeamDto
            {
                Id = x.Id,
                Name = x.Name,
            });
        }

        public List<TeamDto> GetAll()
        {
            return QueryAllTeam().ToList();
        }

        public async Task<GridResult<TeamDto>> GetAllPaging(GridParam input)
        {
            var query = QueryAllTeam();
            var result = await query.GetGridResult(query, input);
            var dicTeamIdToEmployeeCount = GetDicTeamIdToEmployeeCount();

            foreach (var item in result.Items)
            {
                item.NumberOfEmployee = dicTeamIdToEmployeeCount.ContainsKey(item.Id) ? dicTeamIdToEmployeeCount[item.Id] : 0;
            }

            return result;
        }

        public Dictionary<long, int> GetDicTeamIdToEmployeeCount()
        {
            return   WorkScope.GetAll<EmployeeTeam>()
               .Where(x => x.Employee.Status == EmployeeStatus.Working || x.Employee.Status == EmployeeStatus.MaternityLeave)
               .GroupBy(x => x.TeamId)
               .Select(x => new
               {
                   x.Key,
                   countEmployee = x.Count()
               }).ToDictionary(x => x.Key, x => x.countEmployee);
        }

        public async Task<TeamDto> Create(TeamDto input)
        {
            await ValidCreate(input);
            var entity = ObjectMapper.Map<Team>(input);
            input.Id = await WorkScope.InsertAndGetIdAsync(entity);

            return input; 
        }

        public async Task<TeamDto> Update(TeamDto input)
        {
            await ValidUpdate(input);
            var entity = await WorkScope.GetAsync<Team>(input.Id);
            ObjectMapper.Map(input, entity);

            await WorkScope.UpdateAsync(entity);
            return input;
        }

        public async Task<long> Delete(long id)
        {
            await ValidDelete(id);
            await WorkScope.DeleteAsync<Team>(id);

            return id;
        }

        private async Task ValidCreate(TeamDto input)
        {
            var isExist = await WorkScope.GetAll<Team>()
                .AnyAsync(x => x.Name.Trim() == input.Name.Trim());
            if (isExist)
            {
                throw new UserFriendlyException($"Name is Already Exist");
            }
        }

        private async Task ValidUpdate(TeamDto input)
        {
            var isExist = await WorkScope.GetAll<Team>()
                .AnyAsync(x => x.Id != input.Id && x.Name == input.Name);
            if (isExist)
            {
                throw new UserFriendlyException($"Name is Already Exist");
            }
        }

        private async Task ValidDelete(long id)
        {
            var entity = await WorkScope.GetAsync<Team>(id);
            if (entity == null)
            {
                throw new UserFriendlyException($"There is no Team with id {id}");
            }
            var employeeTeams = WorkScope.GetAll<EmployeeTeam>()
            .Where(x => x.TeamId == id)
            .ToList();
            if (employeeTeams.Any())
            {
                throw new UserFriendlyException($"This team already has employee");
            };

            var payslipTeams = WorkScope.GetAll<PayslipTeam>()
                .Where(x => x.TeamId == id);

            if (payslipTeams.Any())
            {
                throw new UserFriendlyException($"This team already has payslip");
            }
              
        }

        public async Task<AddEmployeesToTeamDto> AddEmployeeToTeam(AddEmployeesToTeamDto input)
        {
            var existEmployeeIdsInTeam = WorkScope.GetAll<EmployeeTeam>()
                .Where(x => x.TeamId == input.TeamId)
                .Where(x => input.EmployeeIds.Contains(x.EmployeeId))
                .Select(x => x.EmployeeId)
                .ToList();

            var listToAdd = new List<EmployeeTeam>();

            foreach (var employeeId in input.EmployeeIds)
            {
                if (existEmployeeIdsInTeam.Contains(employeeId))
                {
                    continue;
                }

                var newEmployeeTeam = new EmployeeTeam
                {
                    EmployeeId = employeeId,
                    TeamId = input.TeamId
                };
                listToAdd.Add(newEmployeeTeam);
            }

            await WorkScope.InsertRangeAsync(listToAdd);

            return input;
        }
    }
}
