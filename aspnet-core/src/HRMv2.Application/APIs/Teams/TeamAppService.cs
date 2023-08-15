using Abp.Authorization;
using HRMv2.Authorization;
using HRMv2.Manager.Categories.Teams;
using HRMv2.Manager.Categories.Teams.Dto;
using Microsoft.AspNetCore.Mvc;
using NccCore.Paging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMv2.APIs.Teams
{
 
    public class TeamAppService : HRMv2AppServiceBase
    {
        private readonly TeamManager _teamManager;
        public TeamAppService(TeamManager teamManager)
        {
            _teamManager = teamManager;
        }

        [HttpGet]
        public List<TeamDto> GetAll()
        {
            return _teamManager.GetAll();
        }

        [HttpPost]
        [AbpAuthorize(PermissionNames.Category_Team_View)]
        public async Task<GridResult<TeamDto>> GetAllPaging(GridParam input)
        {
            return await _teamManager.GetAllPaging(input);
        }

        [HttpPost]
        [AbpAuthorize(PermissionNames.Category_Team_Create)]
        public async Task<TeamDto> Create(TeamDto input)
        {
            return await _teamManager.Create(input);
        }

        [HttpPut]
        [AbpAuthorize(PermissionNames.Category_Team_Edit)]
        public async Task<TeamDto> Update(TeamDto input)
        {
            return await _teamManager.Update(input);
        }

        [HttpDelete]
        [AbpAuthorize(PermissionNames.Category_Team_Delete)]
        public async Task<long> Delete(long id)
        {
            return await _teamManager.Delete(id);
        }

        [HttpPost]
        public async Task<AddEmployeesToTeamDto> AddEmployeeToTeam(AddEmployeesToTeamDto input)
        {
            return await _teamManager.AddEmployeeToTeam(input);
        }
    }
}
