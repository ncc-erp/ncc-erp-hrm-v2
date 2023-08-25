using Abp.Authorization;
using Abp.UI;
using HRMv2.Entities;
using HRMv2.Manager.Categories.Banks.Dto;
using HRMv2.NccCore;
using Microsoft.EntityFrameworkCore;
using NccCore.Extension;
using NccCore.Paging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMv2.Manager.Categories.Banks
{
    public class BankManager : BaseManager
    {
        public BankManager(IWorkScope workScope) : base(workScope)
        {
        }

        public IQueryable<BankDto> QueryAllBank()
        {
            return WorkScope.GetAll<Bank>().Select(x => new BankDto
            {
                Id = x.Id,
                Name = x.Name,
                Code = x.Code,
            });
        }

        public async Task<GridResult<BankDto>> GetAllPaging(GridParam input)
        {
            var query = QueryAllBank();
            return await query.GetGridResult(query, input);
        }

        public List<BankDto> GetAll()
        {
            return QueryAllBank().ToList();
        }

        public async Task<BankDto> Create(BankDto input)
        {
            await ValidCreate(input);
            var entity = ObjectMapper.Map<Bank>(input);
            input.Id = await WorkScope.InsertAndGetIdAsync(entity);

            return input;
        }

        public async Task<BankDto> Update(BankDto input)
        {
            await ValidUpdate(input);
            var entity = await WorkScope.GetAsync<Bank>(input.Id);
            ObjectMapper.Map(input, entity);

            await WorkScope.UpdateAsync(entity);
            return input;
        }

        public async Task<long> Delete(long id)
        {
            await ValidDelete(id);
            await WorkScope.DeleteAsync<Bank>(id);

            return id;
        }

        private async Task ValidCreate(BankDto input)
        {
            var isExist = await WorkScope.GetAll<Bank>()
                .AnyAsync(x => x.Code.ToLower().Trim() == input.Code.ToLower().Trim()
                || x.Name.Trim() == input.Name.Trim());
            if (isExist)
            {
                throw new UserFriendlyException($"Name or Code is Already Exist");
            }
        }

        private async Task ValidUpdate(BankDto input)
        {
            var isExist = await WorkScope.GetAll<Bank>()
                .AnyAsync(x => x.Id != input.Id && (x.Code.ToLower().Trim() == input.Code.ToLower().Trim()
                || x.Name == input.Name));
            if (isExist)
            {
                throw new UserFriendlyException($"Name or Code is Already Exist");
            }
        }
        private async Task ValidDelete(long id)
        {
            var entity = await WorkScope.GetAsync<Bank>(id);
            if (entity == null)
            {
                throw new UserFriendlyException($"There is no bank with id {id}");
            }
            var hadUsers = await WorkScope.GetAll<Employee>().AnyAsync(s => s.BankId.Value == id);
            if (hadUsers)
                throw new UserFriendlyException(String.Format("Bank Id {0} has user", id));
        }
    }
}
