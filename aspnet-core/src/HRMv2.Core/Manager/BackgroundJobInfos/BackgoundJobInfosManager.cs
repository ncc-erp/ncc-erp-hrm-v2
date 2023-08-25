using Abp.BackgroundJobs;
using Abp.Domain.Repositories;
using Abp.UI;
using HRMv2.Manager.BackgroundJobInfos.Dto;
using HRMv2.NccCore;
using NccCore.Extension;
using NccCore.Paging;
using NccCore.Uitls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMv2.Manager.BackgoundJobInfosManager
{
    public class BackgoundJobInfosManager : BaseManager
    {
        private readonly IRepository<BackgroundJobInfo, long> _storeJob;
        private readonly IBackgroundJobManager _backgroundJobManager;
        public BackgoundJobInfosManager(IWorkScope workScope, 
            IRepository<BackgroundJobInfo, long> storeJob,
            IBackgroundJobManager backgroundJobManager) : base(workScope)
        {
            _storeJob = storeJob;
            _backgroundJobManager = backgroundJobManager;
        }
        public async Task<GridResult<GetBackgroundJobDto>> GetAllPaging(InputToGetAll input)
        {

            var query = _storeJob.GetAll()
                .Select(x => new GetBackgroundJobDto
                {
                    Id = x.Id,
                    JobType = x.JobType,
                    JobAgrs = x.JobArgs,
                    NextTryTime = x.NextTryTime,
                    LastTryTime = x.LastTryTime,
                    Priority = (int)x.Priority,
                    TryCount = x.TryCount,
                    IsAbandoned = x.IsAbandoned,
                    CreationTime = x.CreationTime,

                }).OrderByDescending(x => x.CreationTime);

            if (!string.IsNullOrEmpty(input.SearchById))
            {
                query = (IOrderedQueryable<GetBackgroundJobDto>)query.Where(x => x.Id.ToString().Contains(input.SearchById));
            }
            return  await query.GetGridResult(query, input.param);
        }

        public void Delete(long Id)
        {
            _backgroundJobManager.Delete(Id.ToString());
        }

        public void RetryBackgroundJob(RetryBackgroundJobDto input)
        {
            var bg = _storeJob.GetAll()
                .Where(x=> x.Id == input.JobId)
                .FirstOrDefault();
            if(bg == default)
            {
                throw new UserFriendlyException($"Can not found background job with Id = {input.JobId}");
            };
            bg.IsAbandoned = false;
            bg.NextTryTime = DateTimeUtils.GetNow().AddSeconds(input.TimeToExecute);
            _storeJob.Update(bg);
        }
 

    }
}
