using Abp.Authorization;
using Abp.UI;
using HRMv2.Entities;
using HRMv2.Manager.Categories.Skills.Dto;
using HRMv2.NccCore;
using Microsoft.EntityFrameworkCore;
using NccCore.Extension;
using NccCore.Paging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMv2.Manager.Categories.Skills
{
    public class SkillManager : BaseManager
    {
        public SkillManager(IWorkScope workScope) : base(workScope)
        {
        }

        public IQueryable<SkillDto> QueryAllSkill()
        {
            return WorkScope.GetAll<Skill>().Select(x => new SkillDto
            {
                Id = x.Id,
                Name = x.Name,
                Code = x.Code
            }).OrderBy(x => x.Name);
        }

        public async Task<GridResult<SkillDto>> GetAllPaging(GridParam input)
        {
            var query = QueryAllSkill();
            return await query.GetGridResult(query, input);
        }

        public List<SkillDto> GetAll()
        {
            return QueryAllSkill().ToList();
        }

        public SkillDto Get(long id)
        {
            return QueryAllSkill()
                .Where(x => x.Id == id)
                .FirstOrDefault();
        }

        public async Task<SkillDto> Create(SkillDto input)
        {
            await ValidCreate(input);
            var entity = ObjectMapper.Map<Skill>(input);
            input.Id = await WorkScope.InsertAndGetIdAsync(entity);

            return input;
        }

        public async Task<SkillDto> Update(SkillDto input)
        {
            await ValidUpdate(input);
            var entity = await WorkScope.GetAsync<Skill>(input.Id);
            ObjectMapper.Map(input, entity);

            await WorkScope.UpdateAsync(entity);
            return input;
        }

        public async Task<long> Delete(long id)
        {
            await ValidDelete(id);
            await WorkScope.DeleteAsync<Skill>(id);

            return id;
        }

        private async Task ValidCreate(SkillDto input)
        {
            var isExist = await WorkScope.GetAll<Skill>()
                .AnyAsync(x => x.Code.ToLower().Trim() == input.Code.ToLower().Trim()
                || x.Name.Trim() == input.Name.Trim());
            if (isExist)
            {
                throw new UserFriendlyException($"Name or Code is Already Exist");
            }
        }

        private async Task ValidUpdate(SkillDto input)
        {
            var isExist = await WorkScope.GetAll<Skill>()
                .AnyAsync(x => x.Id != input.Id && (x.Code.ToLower().Trim() == input.Code.ToLower().Trim()
                || x.Name == input.Name));
            if (isExist)
            {
                throw new UserFriendlyException($"Name or Code is Already Exist");
            }
        }
        private async Task ValidDelete(long id)
        {
            var entity = await WorkScope.GetAsync<Skill>(id);
            if (entity == null)
            {
                throw new UserFriendlyException($"There is no Skill with id {id}");
            }
        }
    }
}
