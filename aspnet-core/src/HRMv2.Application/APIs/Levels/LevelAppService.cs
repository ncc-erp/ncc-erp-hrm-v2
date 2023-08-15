using Abp.Authorization;
using HRMv2.Authorization;
using HRMv2.Manager.Categories.Levels;
using HRMv2.Manager.Categories.Levels.Dto;
using Microsoft.AspNetCore.Mvc;
using NccCore.Paging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMv2.APIs.Levels
{
    [AbpAuthorize]
    public class LevelAppService : HRMv2AppServiceBase
    {
        private readonly LevelManager _levelManager;
        public LevelAppService(LevelManager levelManager)
        {
            _levelManager = levelManager;
        }

        [HttpGet]
        public List<LevelDto> GetAll()
        {
            return _levelManager.GetAll();
        }

        [HttpPost]
        [AbpAuthorize(PermissionNames.Category_Level_View)]
        public async Task<GridResult<LevelDto>> GetAllPaging(GridParam input)
        {
            return await _levelManager.GetAllPaging(input);
        }

        [HttpPost]
        [AbpAuthorize(PermissionNames.Category_Level_Create)]
        public async Task<LevelDto> Create(LevelDto input)
        {
            return await _levelManager.Create(input);
        }

        [HttpPut]
        [AbpAuthorize(PermissionNames.Category_Level_Edit)]
        public async Task<LevelDto> Update(LevelDto input)
        {
            return await _levelManager.Update(input);
        }

        [HttpDelete]
        [AbpAuthorize(PermissionNames.Category_Level_Delete)]
        public async Task<long> Delete(long id)
        {
            return await _levelManager.Delete(id);
        }
    }
}
