using Abp.Domain.Entities;
using HRMv2.Utils;
using NccCore.Anotations;
using NccCore.Paging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMv2.Manager.BackgroundJobInfos.Dto
{
    public class GetBackgroundJobDto
    {
        public long Id { get; set; }
        [ApplySearch]
        public string JobType { get; set; }
        [ApplySearch]
        public string JobAgrs { get; set; }
        public int TryCount { get; set; }
        public DateTime? LastTryTime { get; set; }
        public DateTime NextTryTime { get; set; }
        public bool IsAbandoned { get; set; }
        public int Priority { get; set; }         
        public DateTime CreationTime { get; set; }
        public long CreatorUserId { get; set; }
        public string Description => CommonUtil.BackgroundJobDescription(JobAgrs).Where(s=> s.SubJobType.Contains(SubJobType)).FirstOrDefault()?.Description;
        public string SubJobType => (((JobType.Split(','))[0]).Split('.')).LastOrDefault();
    }

    public class GetBGJobsDescription
    {
        public string SubJobType { get; set; }
        public string Description { get; set; }

    }

    public class InputToGetAll
    {
        public GridParam param { get; set; }
        public string SearchById { get; set; }
    }

}
