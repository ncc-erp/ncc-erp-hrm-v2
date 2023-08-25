using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using HRMv2.Entities;
using HRMv2.Manager.Common.Dto;
using NccCore.Paging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HRMv2.Constants.Enum.HRMEnum;

namespace HRMv2.Manager.Benefits.Dto
{
    [AutoMapTo(typeof(BenefitEmployee))]
    public class GetbenefitEmployeeDto: BaseEmployeeDto
    {
        public long EmployeeId { get; set; }
        public long BenefitId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string UpdatedUserName { get; set; }
        public DateTime? UpdatedTime { get; set; }
        public BEWorkingStatus WorkingStatus { get; set; }
    }

    public class BEWorkingStatus
    {
        public EmployeeStatus Status { get; set; }
        public DateTime DateAt { get; set; }
    }

    public class GetbenefitEmployeeInputDto
    {
        public GridParam GridParam { get; set; }
        public List<long> TeamIds { get; set; }
        public bool IsAndCondition { get; set; }
        public List<EmployeeStatus> StatusIds { get; set; }
        public List<long> LevelIds { get; set; }
        public List<UserType> UserTypes { get; set; }
        public List<long> BranchIds { get; set; }
        public List<long> JobPositionIds { get; set; }
    }
}
