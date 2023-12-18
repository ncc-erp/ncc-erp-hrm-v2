using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HRMv2.Constants.Enum.HRMEnum;

namespace HRMv2.Manager.Home.Dtos
{
    public class HomepageChartFilterDto
    {
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public List<long> JobPositionIds { get; set; }
        public List<long> LevelIds { get; set; }
        public List<long> BranchIds { get; set; }
        public List<long> TeamIds{ get; set; }
        public List<UserType> UserTypes { get; set; }
        public List<PayslipDetailType> PayslipDetailTypes { get; set; }
        public List<EmployeeStatus> WorkingStatuses { get; set; }
    }
}
