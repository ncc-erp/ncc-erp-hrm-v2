using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HRMv2.Constants.Enum.HRMEnum;

namespace HRMv2.Manager.Timesheet.Dto
{
    public class InpuReviewInternFromTSDto
    {
        public class UserToUpdateFromTSDto
        {
            public string NewLevel { get; set; }
            public UserType UserType { get; set; }
            public bool IsFullSalary { get; set; }
            public double Salary { get; set; }
            public DateTime ApplyDate { get; set; }
            public string NormalizedEmailAddress { get; set; }
        }

        public class InputCreateRequestHrmv2Dto
        {
            public string RequestName { get; set; }
            public List<UserToUpdateFromTSDto> ListUserToUpdate { get; set; }
            public DateTime Applydate { get; set; }
            public string CreatedBy { get; set; }
        }

        public class MapTSEmployeeDto
        {
            public string Email { get; set; }
            public long EmployeeId { get; set; }
            public UserType OldUserType { get; set; }
            public UserType NewUserType { get; set; }
            public long OldLevel { get; set; }
            public long NewLevel { get; set; }
            public long JobPosition { get; set; }
            public double OldSalary { get; set; }
            public double NewSalary { get; set; }
            public bool IsFullSalary { get; set; }
        }
    }
}
