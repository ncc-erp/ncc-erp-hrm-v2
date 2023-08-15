using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HRMv2.Constants.Enum.HRMEnum;

namespace HRMv2.Manager.SalaryRequests.Dto
{
    public class DicEmployeeInfoDto
    {
        public long Id { get; set; }
        public long LevelId { get; set; }
        public long JobPositionId { get; set; }
        public double ProbationPercentage { get; set; }
        public UserType UserType { get; set; }
        public double RealSalary { get; set; }
    }
}
