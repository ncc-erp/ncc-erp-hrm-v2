using Abp.UI;
using HRMv2.Entities;
using HRMv2.Manager.Categories.Levels.Dto;
using HRMv2.NccCore;
using Microsoft.EntityFrameworkCore;
using NccCore.Extension;
using NccCore.Paging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HRMv2.Constants.Enum.HRMEnum;

namespace HRMv2.Manager.Categories.Levels
{
    public class LevelManager : BaseManager
    {
        public LevelManager(IWorkScope workScope) : base(workScope)
        {
        }

        public IQueryable<LevelDto> QueryAllLevel()
        {
            return WorkScope.GetAll<Level>().Select(x => new LevelDto
            {
                Id = x.Id,
                Name = x.Name,
                Code = x.Code,
                ShortName = x.ShortName,
                Color = x.Color,
            }).OrderBy(x => x.Name);
        }

        public List<LevelDto> GetAll()
        {
            return QueryAllLevel().ToList();
        }

        public async Task<GridResult<LevelDto>> GetAllPaging(GridParam input)
        {
            var query = QueryAllLevel();
            return await query.GetGridResult(query, input);
        }

        public async Task<LevelDto> Create(LevelDto input)
        {
            await ValidCreate(input);
            var entity = ObjectMapper.Map<Level>(input);
            input.Id = await WorkScope.InsertAndGetIdAsync(entity);

            return input;
        }

        public async Task<LevelDto> Update(LevelDto input)
        {
            await ValidUpdate(input);
            var entity = await WorkScope.GetAsync<Level>(input.Id);
            ObjectMapper.Map(input, entity);

            await WorkScope.UpdateAsync(entity);
            return input;
        }

        public async Task<long> Delete(long id)
        {
            await ValidDelete(id);
            await WorkScope.DeleteAsync<Level>(id);

            return id;
        }

        private async Task ValidCreate(LevelDto input)
        {
            var isExist = await WorkScope.GetAll<Level>()
                .AnyAsync(x => x.Code.ToLower().Trim() == input.Code.ToLower().Trim()
                || x.Name.Trim() == input.Name.Trim());
            if (isExist)
            {
                throw new UserFriendlyException($"Name or Code is Already Exist");
            }
        }

        private async Task ValidUpdate(LevelDto input)
        {
            var isExist = await WorkScope.GetAll<Level>()
                .AnyAsync(x => x.Id != input.Id && (x.Code.ToLower().Trim() == input.Code.ToLower().Trim()
                || x.Name == input.Name));
            if (isExist)
            {
                throw new UserFriendlyException($"Name or Code is Already Exist");
            }
        }
        private async Task ValidDelete(long id)
        {
            var entity = await WorkScope.GetAsync<Level>(id);
            if (entity == null)
            {
                throw new UserFriendlyException($"There is no Level with id {id}");
            }
            var hadUsers = await WorkScope.GetAll<Employee>().AnyAsync(s => s.LevelId == id);
            if (hadUsers)
                throw new UserFriendlyException(String.Format("Level Id {0} has user", id));
        }

        public void CreateDefaultLevel(int tenantId)
        {
            var existLevel = WorkScope.GetAll<Level>().Where(q => q.TenantId == tenantId).FirstOrDefault();

            if (existLevel != default)
            {
                return;
            }

            var seedDataLevel = new List<Level>{
                new Level
                {
                    Name = "Intern_0",
                    ShortName = "I0",
                    Code = "0",
                    Color = "#B2BEB5",
                    TenantId = tenantId,
                },
                new Level
                {
                    Name = "Intern_1",
                    ShortName = "I1",
                    Code = "1",
                    Color = "#8F9779",
                    TenantId = tenantId,
                },
                new Level
                {
                    Name = "Intern_2",
                    ShortName = "I2",
                    Code = "2",
                    Color = "#665D1E",
                    TenantId = tenantId,
                },
                new Level
                {
                    Name = "Intern_3",
                    ShortName = "I3",
                    Code = "3",
                    Color = "#777",
                    TenantId = tenantId,
                },
                new Level
                {
                    Name = "Fresher-",
                    ShortName = "F-",
                    Code = "4",
                    Color = "#60b8ff",
                    TenantId = tenantId,
                },
                new Level
                {
                    Name = "Fresher",
                    ShortName = "F",
                    Code = "5",
                    Color = "#318CE7",
                    TenantId = tenantId,
                },
                new Level
                {
                    Name = "Fresher+",
                    ShortName = "F+",
                    Code = "6",
                    Color = "#1f75cb",
                    TenantId = tenantId,
                },
                new Level
                {
                    Name = "Junior-",
                    ShortName = "J-",
                    Code = "7",
                    Color = "#ad9fa1",
                    TenantId = tenantId,
                },
                new Level
                {
                    Name = "Junior",
                    ShortName = "J",
                    Code = "8",
                    Color = "#A57164",
                    TenantId = tenantId,
                },
                new Level
                {
                    Name = "Junior+",
                    ShortName = "J+",
                    Code = "9",
                    Color = "#3B2F2F",
                    TenantId = tenantId,
                },
                new Level
                {
                    Name = "Middle-",
                    ShortName = "M-",
                    Code = "10",
                    Color = "#A4C639",
                    TenantId = tenantId,
                },
                new Level
                {
                    Name = "Middle",
                    ShortName = "M",
                    Code = "11",
                    Color = "#3bab17",
                    TenantId = tenantId,
                },
                new Level
                {
                    Name = "Middle+",
                    ShortName = "M+",
                    Code = "12",
                    Color = "#008000",
                    TenantId = tenantId,
                },
                new Level
                {
                    Name = "Senior-",
                    ShortName = "S-",
                    Code = "13",
                    Color = "#c36285",
                    TenantId = tenantId,
                },
                new Level
                {
                    Name = "Senior",
                    ShortName = "S",
                    Code = "14",
                    Color = "#AB274F",
                    TenantId = tenantId,
                },
                new Level
                {
                    Name = "Principal",
                    ShortName = "P",
                    Code = "15",
                    Color = "#902ee1",
                    TenantId = tenantId,
                },
            };

            WorkScope.InsertRange(seedDataLevel);
        }
    }
}
