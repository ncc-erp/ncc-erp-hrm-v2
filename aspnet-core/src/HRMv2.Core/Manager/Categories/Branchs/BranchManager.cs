using Abp.Authorization;
using Abp.UI;
using HRMv2.Entities;
using HRMv2.NccCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NccCore.Extension;
using NccCore.Paging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMv2.Manager.Categories
{
    public class BranchManager : BaseManager
    {
        public BranchManager(IWorkScope workScope) : base(workScope)
        {
        }

        public IQueryable<BranchDto> QueryAllBranch()
        {
            var query = (from x in WorkScope.GetAll<Branch>()
                        join e in WorkScope.GetAll<Employee>()
                        on x.CEOId equals e.Id into eb
                        from ebi in eb.DefaultIfEmpty()
                        join hr in WorkScope.GetAll<Employee>()
                        on x.HRId equals hr.Id into hrb
                        from hr in hrb.DefaultIfEmpty()
                        select new BranchDto
                        {
                            Id = x.Id,
                            Name = x.Name,
                            Address = x.Address,
                            Code = x.Code,
                            ShortName = x.ShortName,
                            Color = x.Color,
                            CEOId = x.CEOId,
                            HRId = x.HRId,
                            CompanyPhone = x.CompanyPhone,
                            CompanyTaxCode = x.CompanyTaxCode,
                            NameInContract = x.NameInContract,
                            CEOFullName = ebi.FullName,
                            HRFullName = hr.FullName
                        }).OrderBy(x => x.Name);
            return query;
        }
        public List<BranchDto> GetAll()
        {
            return QueryAllBranch().ToList();
        }

        public async Task<GridResult<BranchDto>> GetAllPaging(GridParam input)
        {
            var query = QueryAllBranch();
            return await query.GetGridResult(query, input);
        }

        public async Task<BranchDto> Create(BranchDto input)
        {
            await ValidCreate(input);
            var entity = ObjectMapper.Map<Branch>(input);
            input.Id = await WorkScope.InsertAndGetIdAsync(entity);

            return input;
        }

        public async Task<BranchDto> Update(BranchDto input)
        {
            await ValidUpdate(input);
            var entity = await WorkScope.GetAsync<Branch>(input.Id);
            ObjectMapper.Map(input, entity);

            await WorkScope.UpdateAsync(entity);
            return input;
        }

        public async Task<long> Delete(long id)
        {
            await ValidDelete(id);            
            await WorkScope.DeleteAsync<Branch>(id);

            return id;
        }

        private async Task ValidCreate(BranchDto input)
        {
            var isExist = await WorkScope.GetAll<Branch>()
                .AnyAsync(x => x.Code.ToLower().Trim() == input.Code.ToLower().Trim()
                || x.Name.Trim() == input.Name.Trim());
            if (isExist)
            {
                throw new UserFriendlyException($"Name or Code is Already Exist");
            }
        }

        private async Task ValidUpdate(BranchDto input)
        {
            var isExist = await WorkScope.GetAll<Branch>()
                .AnyAsync(x => x.Id != input.Id && (x.Code.ToLower().Trim() == input.Code.ToLower().Trim()
                || x.Name == input.Name));
            if (isExist)
            {
                throw new UserFriendlyException($"Name or Code is Already Exist");
            }
        }
        private async Task ValidDelete(long id)
        {
            var entity = await WorkScope.GetAsync<Branch>(id);
            if (entity == null)
            {
                throw new UserFriendlyException($"There is no branch with id {id}");
            }
            var hadUsers = await WorkScope.GetAll<Employee>().AnyAsync(s => s.BranchId == id);
            if (hadUsers)
            {
                throw new UserFriendlyException(String.Format("Branch Id {0} has user", id));
            }        
        }
    }
}
