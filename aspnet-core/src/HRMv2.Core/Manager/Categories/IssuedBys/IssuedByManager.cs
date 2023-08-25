using Abp.UI;
using HRMv2.Entities;
using HRMv2.Manager.Categories.IssuedBys.Dto;
using HRMv2.NccCore;
using Microsoft.EntityFrameworkCore;
using NccCore.Extension;
using NccCore.Paging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HRMv2.Manager.Categories.IssuedBys
{
    public class IssuedByManager : BaseManager
    {
        public IssuedByManager(IWorkScope workScope) : base(workScope)
        {
        }
        public IQueryable<IssuedByDto> QueryAllIssuedBy()
        {
            return WorkScope.GetAll<IssuedBy>().Select(x => new IssuedByDto
            {
                Id = x.Id,
                Name = x.Name,
            });
        }
        public async Task<GridResult<IssuedByDto>> GetAllPaging(GridParam input)
        {
            var query = QueryAllIssuedBy();
            return await query.GetGridResult(query, input);
        }
        public List<IssuedByDto> GetAll()
        {
            return QueryAllIssuedBy().ToList();
        }
        public async Task<IssuedByDto> Create(IssuedByDto input)
        {
            await ValidCreate(input);
            var entity = ObjectMapper.Map<IssuedBy>(input);
            input.Id = await WorkScope.InsertAndGetIdAsync(entity);

            return input;
        }

        public async Task<IssuedByDto> Update(IssuedByDto input)
        {
            await ValidUpdate(input);
            var entity = await WorkScope.GetAsync<IssuedBy>(input.Id);
            ObjectMapper.Map(input, entity);

            await WorkScope.UpdateAsync(entity);
            return input;
        }
        public async Task<long> Delete(long id)
        {
            await ValidDelete(id);
            await WorkScope.DeleteAsync<IssuedBy>(id);

            return id;
        }
        private async Task ValidCreate(IssuedByDto input)
        {
            var isExist = await WorkScope.GetAll<IssuedBy>()
               .AnyAsync(x => x.Name.Trim() == input.Name.Trim());
            if (isExist)
            {
                throw new UserFriendlyException($"Name is Already Exist");
            }
        }

        private async Task ValidUpdate(IssuedByDto input)
        {
            var isExist = await WorkScope.GetAll<IssuedBy>()
                .AnyAsync(x => x.Id != input.Id && (x.Name == input.Name));
            if (isExist)
            {
                throw new UserFriendlyException($"Name is Already Exist");
            }
        }
        private async Task ValidDelete(long id)
        {
            var entity = await WorkScope.GetAsync<IssuedBy>(id);
            if (entity == null)
            {
                throw new UserFriendlyException($"There is no bank with id {id}");
            }
        }
    }
}