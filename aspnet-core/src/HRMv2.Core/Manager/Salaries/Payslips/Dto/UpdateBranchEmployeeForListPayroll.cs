using Amazon.S3.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HRMv2.Constants.Enum.HRMEnum;

namespace HRMv2.Manager.Salaries.Payslips.Dto
{
    public class UpdateBranchEmployeeForListPayroll
    {
        public List<long> PayrollIds { get; set; }
        public long EmployeeId { get; set; }
        public long BranchId { get; set; }
    }
    public class UpdateLevelEmployeeForListPayroll
    {
        public List<long> PayrollIds { get; set; }
        public long EmployeeId { get; set; }
        public long LevelId { get; set; }
    }
    public class UpdateJobPositionEmployeeForListPayroll
    {
        public List<long> PayrollIds { get; set; }
        public long EmployeeId { get; set; }
        public long JobPositionId { get; set; }
    }
    public class UpdateUserTypeEmployeeForListPayroll
    {
        public List<long> PayrollIds { get; set; }
        public long EmployeeId { get; set; }
        public UserType UserType { get; set; }
    }
}
