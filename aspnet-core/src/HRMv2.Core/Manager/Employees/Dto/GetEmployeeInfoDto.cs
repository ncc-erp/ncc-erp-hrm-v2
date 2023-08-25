using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMv2.Manager.Employees.Dto
{
    public class GetEmployeeInfoDto
    {
        public bool IsAllowEdit { get; set; }
        public bool IsAllowEditWorkingStatus { get; set; }
        public bool IsAllowEditBranch { get; set; }
        public GetEmployeeDetailDto EmployeeInfo { get; set; }
    }
}
