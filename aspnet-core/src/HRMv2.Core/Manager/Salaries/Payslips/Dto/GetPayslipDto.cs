using Abp.AutoMapper;
using HRMv2.Entities;
using HRMv2.Manager.Common.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HRMv2.Constants.Enum.HRMEnum;

namespace HRMv2.Manager.Salaries.Payslips.Dto
{
    [AutoMapTo(typeof(Payslip))]
    public class GetPayslipDto : BaseEmployeeDto
    {
        public long PayrollId { get; set; }
        public long EmployeeId { get; set; }
        public double Salary { get; set; }
        public double RemainLeaveDayBefore { get; set; }
        public double RemainLeaveDayAfter { get; set; }
        public double NormalHour { get; set; }
        public double OTHour { get; set; }
        public double RealOffsetDay { get; set; }
        public double OpentalkCount { get; set; }
        public double RemoteDay { get; set; }
        public double OffDay { get; set; }
        public double WorkAtOfficeOrOnsiteDay { get; set; }
        public double OffsetHour { get; set; }
        public DateTime CreationTime { get; set; }
        public long BranchEmployeePayslipId { get; set; }
        public UserType UserTypeEmployeePayslip { get; set; }
        public long LevelEmployeePayslipId { get; set; }
        public long JobPositionEmployeePayslipId { get; set; }
        public List<long> PayslipTeams { get; set; }
    }
}
