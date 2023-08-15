using HRMv2.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HRMv2.Constants.Enum.HRMEnum;

namespace HRMv2.Manager.Employees.Dto
{
    public class GetEmployeeStatisticDto
    {
        public List<EmployeeStatisticDto> OnboardedEmployees { get; set; }
        public List<EmployeeStatisticDto> QuitEmployees { get; set; }
    }


    public class EmployeeStatisticDto{
        public long Id { get; set; }
        public string Email { get; set; }
        public UserType UserType { get; set; }
        public string UserTypeName => CommonUtil.GetUserTypeNameVN(UserType);
        public string BranchName { get; set; }
    }
}
