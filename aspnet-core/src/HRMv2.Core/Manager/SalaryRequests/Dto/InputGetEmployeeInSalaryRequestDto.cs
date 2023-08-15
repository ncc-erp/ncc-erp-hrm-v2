using NccCore.Paging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HRMv2.Constants.Enum.HRMEnum;

namespace HRMv2.Manager.SalaryRequests.Dto
{
    public class InputGetEmployeeInSalaryRequestDto
    {
        public GridParam GridParam { get; set; }
        public List<long> ToLevelIds { get; set; }
        public List<UserType> ToUsertypes { get; set; }
        public List<long> ToJobPositionIds { get; set; }
        public List<long> BranchIds { get; set; }


    }
}
