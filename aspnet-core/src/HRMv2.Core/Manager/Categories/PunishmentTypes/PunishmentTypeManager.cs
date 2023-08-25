using Abp.Authorization;
using Abp.UI;
using HRMv2.Entities;
using HRMv2.Manager.Categories.PunishmentTypes.Dto;
using HRMv2.NccCore;
using Microsoft.EntityFrameworkCore;
using NccCore.Extension;
using NccCore.Paging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMv2.Manager.Categories.PunishmentTypes
{
    public class PunishmentTypeManager : BaseManager
    {
        public PunishmentTypeManager(IWorkScope workScope) : base(workScope)
        {
        }

        public IQueryable<PunishmentTypeDto> QueryPunishmentType()
        {
            return WorkScope.GetAll<PunishmentType>().Select(x => new PunishmentTypeDto
            {
                Id = x.Id,
                Name = x.Name,
                IsActive = x.IsActive,
                Api = x.Api
            });
        }

        public async Task<GridResult<PunishmentTypeDto>> GetAllPaging(GridParam input)
        {
            var query = QueryPunishmentType();
            return await query.GetGridResult(query, input);
        }

        public List<PunishmentTypeDto> GetAll()
        {
            return QueryPunishmentType()
                .Where(x => x.IsActive)
                .ToList();
        }

        public async Task<PunishmentTypeDto> Create(PunishmentTypeDto input)
        {
            await ValidCreate(input);
            var entity = ObjectMapper.Map<PunishmentType>(input);
            input.Id = await WorkScope.InsertAndGetIdAsync(entity);

            return input;
        }

        public async Task<PunishmentTypeDto> Update(PunishmentTypeDto input)
        {
            await ValidUpdate(input);
            var entity = await WorkScope.GetAsync<PunishmentType>(input.Id);
            ObjectMapper.Map(input, entity);

            await WorkScope.UpdateAsync(entity);
            return input;
        }

        public async Task<long> Delete(long id)
        {
            await ValidDelete(id);
            await WorkScope.DeleteAsync<PunishmentType>(id);
            return id;
        }

        private async Task ValidCreate(PunishmentTypeDto input)
        {
            var isExist = await WorkScope.GetAll<PunishmentType>()
                .AnyAsync(x =>  x.Name.Trim() == input.Name.Trim());
            if (isExist)
            {
                throw new UserFriendlyException($"Name is Already Exist");
            }
        }

        private async Task ValidUpdate(PunishmentTypeDto input)
        {
            var isExist = await WorkScope.GetAll<PunishmentType>()
                .AnyAsync(x => x.Id != input.Id && x.Name == input.Name);
            if (isExist)
            {
                throw new UserFriendlyException($"Name is Already Exist");
            }
        }
        private async Task ValidDelete(long id)
        {
            var entity = await WorkScope.GetAsync<PunishmentType>(id);
            if (entity == null)
            {
                throw new UserFriendlyException($"There is no PunishmentType with id {id}");
            }
        }
    }
}
