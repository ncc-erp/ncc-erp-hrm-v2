using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HRMv2.Constants.Enum.HRMEnum;

namespace HRMv2.Manager.Employees.Dto
{
    public class GetEmployeeByUserMezonId
    {
        public string Email { get; set; }
        public string FullName { get; set; }
        public string BranchCode { get; set; }
        public string BranchName { get; set; }
        public string UserMezonId { get; set; }
        public EmployeeStatus Status { get; set; }
    }
}
