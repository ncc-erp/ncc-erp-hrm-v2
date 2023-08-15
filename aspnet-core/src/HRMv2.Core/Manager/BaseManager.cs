using Abp.Application.Services;
using Abp.Dependency;
using HRMv2.Authorization.Users;
using HRMv2.Entities;
using HRMv2.MultiTenancy;
using HRMv2.NccCore;
using Microsoft.EntityFrameworkCore;
using NccCore.Extension;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HRMv2.Manager
{
    public class BaseManager : ApplicationService
    {       
        protected IWorkScope WorkScope { get; set; }

        public BaseManager(IWorkScope workScope)
        {
            WorkScope = workScope;
        }

        public Dictionary<string, long> GetDicEmployeeEmailToId()
        {
            return WorkScope.GetAll<Employee>()
                .Select(s => new {s.Id, s.Email})
                .ToDictionary(s => s.Email,s => s.Id);
        }

        public IQueryable<long> QueryEmployeeHaveAnyTeams(List<long> TeamsId)
        {
            return WorkScope.GetAll<EmployeeTeam>()
                .Where(s => TeamsId.Contains(s.TeamId))
                .Select(s => s.EmployeeId);
        }


        public async Task<List<long>> QueryEmployeeHaveAllTeams(List<long> teamsId)
        {
            var result = await WorkScope.GetAll<EmployeeTeam>()
                    .Where(s => teamsId[0] == s.TeamId)
                    .Select(s => s.EmployeeId)
                    .Distinct()
                    .ToListAsync();

            if (result == null || result.IsEmpty())
            {
                return new List<long>();
            }

            for (var i = 1; i < teamsId.Count(); i++)
            {
                var userIds = await WorkScope.GetAll<EmployeeTeam>()
                    .Where(s => teamsId[i] == s.TeamId)
                    .Select(s => s.EmployeeId)
                    .Distinct()
                    .ToListAsync();

                result = result.Intersect(userIds).ToList();

                if (result == null || result.IsEmpty())
                {
                    return new List<long>();
                }
            }

            return result;

        }

        public string GetBranchCodeById(long id)
        {
            return  WorkScope.GetAll<Branch>()
                .Where(x => x.Id == id)
                .Select(x => x.Code)
                .FirstOrDefault();
        }

        public string GetPositionCodeById(long id)
        {
            return WorkScope.GetAll<JobPosition>()
                .Where(x => x.Id == id)
                .Select(x => x.Code)
                .FirstOrDefault();
        }
        public List<string> GetLitsSkillNameById(long id)
        {
            return WorkScope.GetAll<EmployeeSkill>()
                .Where(x => x.Id == id)
                .Select(x => x.Skill.Name)
                .ToList();
        }

        public string GetLevelCodeById(long id)
        {
            return  WorkScope.GetAll<Level>()
                .Where(x => x.Id == id)
                .Select(x => x.Code)
                .FirstOrDefault();
        }
        public string GetJobPositionCodeById(long id)
        {
            return  WorkScope.GetAll<JobPosition>()
                .Where(x => x.Id == id)
                .Select(x => x.Code)
                .FirstOrDefault();
        }

        public string GetNameByFullName(string fullName)
        {
            return fullName.Substring(fullName.LastIndexOf(" ") + 1);
        }

        public string GetSurNameByFullName(string fullName)
        {
            return fullName.Substring(0, fullName.LastIndexOf(" "));
        }
    }
}
