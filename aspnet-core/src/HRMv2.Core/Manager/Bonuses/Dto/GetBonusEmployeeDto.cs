using HRMv2.Manager.Common.Dto;
using NccCore.Paging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HRMv2.Constants.Enum.HRMEnum;

namespace HRMv2.Manager.Bonuses.Dto
{
    public class GetBonusEmployeeDto : BaseEmployeeDto
    {
        public long BonusId { get; set; }
        public long EmployeeId { get; set; }
        public double Money { get; set; }
        public string Note { get; set; }

        public string BonusName { get; set; }
        public DateTime ApplyDate { get; set; }

        public DateTime? LastModificationTime { get; set; }
        public string FullNameModification { get; set; }
    }

    public class GetBonusEmployeeInputDto
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
