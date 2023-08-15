using Abp.Authorization;
using HRMv2.Authorization;
using HRMv2.Manager.Categories.Skills;
using HRMv2.Manager.Categories.Skills.Dto;
using Microsoft.AspNetCore.Mvc;
using NccCore.Paging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMv2.APIs.Skills
{
    [AbpAuthorize(PermissionNames.Category_Skill)]
    public class SkillAppService : HRMv2AppServiceBase
    {
        private readonly SkillManager _skillManager;
        public SkillAppService(SkillManager skillManager)
        {
            _skillManager = skillManager;
        }

        [HttpGet]
        public List<SkillDto> GetAll()
        {
           return _skillManager.GetAll();
        }

        [HttpGet]
        public SkillDto Get(long id)
        {
            return _skillManager.Get(id);
        }

        [HttpPost]
        [AbpAuthorize(PermissionNames.Category_Skill)]
        public async Task<GridResult<SkillDto>> GetAllPaging(GridParam input)
        {
            return await _skillManager.GetAllPaging(input);
        }

        [HttpPost]
        [AbpAuthorize(PermissionNames.Category_Skill_Create)]
        public async Task<SkillDto> Create(SkillDto input)
        {
            return await _skillManager.Create(input);
        }

        [HttpPut]
        [AbpAuthorize(PermissionNames.Category_Skill_Edit)]
        public async Task<SkillDto> Update(SkillDto input)
        {
            return await _skillManager.Update(input);
        }

        [HttpDelete]
        [AbpAuthorize(PermissionNames.Category_Skill_Delete)]
        public async Task<long> Delete(long id)
        {
            return await _skillManager.Delete(id);
        }
    }
}
