using Abp.Authorization;
using HRMv2.Authorization;
using HRMv2.Manager.BackgoundJobInfosManager;
using HRMv2.Manager.BackgroundJobInfos.Dto;
using Microsoft.AspNetCore.Mvc;
using NccCore.Paging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMv2.APIs.BackgroundJobs
{
    [AbpAuthorize]
    public class BackgroundJobAppService: HRMv2AppServiceBase
    {
        private readonly BackgoundJobInfosManager _backgroundJobInfosManager;
        public BackgroundJobAppService(BackgoundJobInfosManager backgroundJobManager)
        {
            _backgroundJobInfosManager = backgroundJobManager;
        }
        [HttpPost]
        public async Task<GridResult<GetBackgroundJobDto>> GetAllPaging(InputToGetAll input)
        {
            return await _backgroundJobInfosManager.GetAllPaging(input);
        }

        [HttpDelete]
        [AbpAuthorize(PermissionNames.Admin_BackgroundJob_Delete)]
        public void Delete(long Id)
        {
            _backgroundJobInfosManager.Delete(Id);
        }
        [HttpPut]
        public void RetryBackgroundJob(RetryBackgroundJobDto input)
        {
            _backgroundJobInfosManager.RetryBackgroundJob(input);
        }


    }
}
