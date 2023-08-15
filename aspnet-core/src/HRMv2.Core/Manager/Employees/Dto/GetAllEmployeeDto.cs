using HRMv2.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HRMv2.Constants.Enum.HRMEnum;

namespace HRMv2.Manager.Employees.Dto
{
    public class GetAllEmployeeDto
    {
        public string Email { get; set; }
        public string FullName { get; set; }
        public string BranchCode { get; set; }
        public string JobPositionCode { get; set; }
        public UserType UserType { get; set; }
        public string UserTypeName => CommonUtil.GetUserTypeNameVN(UserType);
        public EmployeeStatus Status { get; set; }
        public string StatusName => CommonUtil.GetWorkingStatusName(Status);
    }
}
