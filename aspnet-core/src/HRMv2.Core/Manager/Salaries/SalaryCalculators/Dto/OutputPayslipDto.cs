using Abp.AutoMapper;
using HRMv2.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HRMv2.Constants.Enum.HRMEnum;

namespace HRMv2.Manager.Salaries.SalaryCalculators.Dto
{
    [AutoMapTo(typeof(Payslip))]
    public class OutputPayslipDto
    {
        public long PayrollId { get; set; }
        public long EmployeeId { get; set; }
        public long BranchId { get; set; }
        public UserType UserType { get; set; }
        public long LevelId { get; set; }
        public long JobPositionId { get; set; }
        public float RemainLeaveDayBefore { get; set; }
        public float RemainLeaveDayAfter { get; set; }
        public float AddedLeaveDay { get; set; }

        /// <summary>
        /// Tổng số ngày làm việc thực tế( Không tính opentalk, ko tính bù phép)
        /// </summary>
        public float NormalDay { get; set; }
        public float OTHour { get; set; }//đã nhân hệ số
        public float RefundLeaveDay { get; set; }
        public int OpentalkCount { get; set; }
        public float WorkAtOfficeOrOnsiteDay { get; set; }
        public float OffDay { get; set; }
        public double Salary { get; set; }
        public DateTime ComplainDeadline { get; set; }
        public long? BankId { get; set; }
        public string BankAccountNumber { get; set; }
        public string BankName { get; set; }
    }
}
