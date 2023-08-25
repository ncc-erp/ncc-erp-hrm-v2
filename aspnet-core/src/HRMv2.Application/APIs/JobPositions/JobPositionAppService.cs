using Abp.Authorization;
using HRMv2.Authorization;
using HRMv2.Manager.Categories.JobPositions;
using HRMv2.Manager.Categories.JobPositions.Dto;
using HRMv2.Manager.Categories.PunishmentTypes.Dto;
using Microsoft.AspNetCore.Mvc;
using NccCore.Paging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMv2.APIs.JobPositions
{
    [AbpAuthorize]
    public class JobPositionAppService : HRMv2AppServiceBase
    {

        private readonly JobPositionManager _jobPositionManager;
        public JobPositionAppService(JobPositionManager jobPositionManager)
        {
            _jobPositionManager = jobPositionManager;
        }

        [HttpGet]
        public List<JobPositionDto> GetAll()
        {
            return _jobPositionManager.GetAll();
        }

        [HttpPost]
        [AbpAuthorize(PermissionNames.Category_JobPosition_View)]
        public async Task<GridResult<JobPositionDto>> GetAllPaging(GridParam input)
        {
            return await _jobPositionManager.GetAllPaging(input);
        }

        [HttpPost]
        [AbpAuthorize(PermissionNames.Category_JobPosition_Create)]
        public async Task<JobPositionDto> Create(JobPositionDto input)
        {
            return await _jobPositionManager.Create(input);
        }

        [HttpPut]
        [AbpAuthorize(PermissionNames.Category_JobPosition_Edit)]
        public async Task<JobPositionDto> Update(JobPositionDto input)
        {
            return await _jobPositionManager.Update(input);
        }

        [HttpDelete]
        [AbpAuthorize(PermissionNames.Category_JobPosition_Delete)]
        public async Task<long> Delete(long id)
        {
            return await _jobPositionManager.Delete(id);
        }
    }
}
