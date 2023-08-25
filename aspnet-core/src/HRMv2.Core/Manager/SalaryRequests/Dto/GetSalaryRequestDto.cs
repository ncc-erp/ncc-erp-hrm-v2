using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using HRMv2.Entities;
using NccCore.Anotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HRMv2.Constants.Enum.HRMEnum;

namespace HRMv2.Manager.SalaryRequests.Dto
{
    [AutoMapTo(typeof(SalaryChangeRequest))]
    public class GetSalaryRequestDto : EntityDto<long>
    {
        [ApplySearch]
        public string Name { get; set; }
        public DateTime ApplyMonth { get; set; }
        public SalaryRequestStatus Status { get; set; }
        public string CreatorUser { get; set; }
        public DateTime CreationTime { get; set; }
        public string LastModifyUser { get; set; }
        public DateTime LastModifyTime { get; set; }
    }
}
