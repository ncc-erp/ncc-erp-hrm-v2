using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HRMv2.Constants.Enum.HRMEnum;

namespace HRMv2.Manager.ChangeEmployeeWorkingStatuses.Dto
{
    public class EmployeeInfoDto
    {
        public long EmployeeId { get; set; }    
        public EmployeeStatus Status { get; set; }
        public UserType UserType { get; set; }
        public long LevelId { get; set; }
        public long JobPositionId { get; set; }

        public double Salary { get; set; }
    }


}
