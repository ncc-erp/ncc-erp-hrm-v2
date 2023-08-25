using Abp.Authorization;
using Abp.UI;
using HRMv2.Entities;
using HRMv2.Manager.Categories.JobPositions.Dto;
using HRMv2.NccCore;
using Microsoft.EntityFrameworkCore;
using NccCore.Extension;
using NccCore.Paging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMv2.Manager.Categories.JobPositions
{
    public class JobPositionManager : BaseManager
    {
        public JobPositionManager(IWorkScope workScope) : base(workScope)
        {
        }

        public IQueryable<JobPositionDto> QueryAllJobPosition()
        {
            return WorkScope.GetAll<JobPosition>().Select(x => new JobPositionDto
            {
                Id = x.Id,
                Name = x.Name,
                Code = x.Code,
                ShortName = x.ShortName,
                Color = x.Color,
                NameInContract = x.NameInContract
            });
        }

        public List<JobPositionDto> GetAll()
        {
            return QueryAllJobPosition().ToList();
        }

        public async Task<GridResult<JobPositionDto>> GetAllPaging(GridParam input)
        {
            var query = QueryAllJobPosition();
            return await query.GetGridResult(query, input);
        }

        public async Task<JobPositionDto> Create(JobPositionDto input)
        {
            await ValidCreate(input);
            var entity = ObjectMapper.Map<JobPosition>(input);
            input.Id = await WorkScope.InsertAndGetIdAsync(entity);

            return input;
        }

        public async Task<JobPositionDto> Update(JobPositionDto input)
        {
            await ValidUpdate(input);
            var entity = await WorkScope.GetAsync<JobPosition>(input.Id);
            ObjectMapper.Map(input, entity);

            await WorkScope.UpdateAsync(entity);
            return input;
        }

        public async Task<long> Delete(long id)
        {
            await ValidDelete(id);
            await WorkScope.DeleteAsync<JobPosition>(id);

            return id;
        }

        private async Task ValidCreate(JobPositionDto input)
        {
            var isExist = await WorkScope.GetAll<JobPosition>()
                .AnyAsync(x => x.Code.ToLower().Trim() == input.Code.ToLower().Trim()
                || x.Name.Trim() == input.Name.Trim());
            if (isExist)
            {
                throw new UserFriendlyException($"Name or Code is Already Exist");
            }
        }

        private async Task ValidUpdate(JobPositionDto input)
        {
            var isExist = await WorkScope.GetAll<JobPosition>()
                .AnyAsync(x => x.Id != input.Id && (x.Code.ToLower().Trim() == input.Code.ToLower().Trim()
                || x.Name == input.Name));
            if (isExist)
            {
                throw new UserFriendlyException($"Name or Code is Already Exist");
            }
        }
        private async Task ValidDelete(long id)
        {
            var entity = await WorkScope.GetAsync<JobPosition>(id);
            if (entity == null)
            {
                throw new UserFriendlyException($"There is no JobPosition with id {id}");
            }
            var hadUsers = await WorkScope.GetAll<Employee>().AnyAsync(s => s.JobPositionId== id);
            if (hadUsers)
                throw new UserFriendlyException(String.Format("Job Position Id {0} has user", id));
        }
    }
}
