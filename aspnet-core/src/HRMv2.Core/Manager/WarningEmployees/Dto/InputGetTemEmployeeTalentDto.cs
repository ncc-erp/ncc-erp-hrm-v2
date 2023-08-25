using NccCore.Paging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HRMv2.Constants.Enum.HRMEnum;

namespace HRMv2.Manager.WarningEmployees.Dto
{
    public class InputGetTemEmployeeTalentDto
    {
        public GridParam GridParam { get; set; }
        public bool IsAndCondition { get; set; }
        public List<TalentOnboardStatus> StatusIds { get; set; }
        public List<long> LevelIds { get; set; }
        public List<UserType> Usertypes { get; set; }
        public List<long> BranchIds { get; set; }
        public List<long> JobPositionIds { get; set; }

    }
}
