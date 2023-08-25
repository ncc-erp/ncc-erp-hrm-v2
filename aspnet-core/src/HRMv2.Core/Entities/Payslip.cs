using Abp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HRMv2.Constants.Enum.HRMEnum;

namespace HRMv2.Entities
{
    public class Payslip : NccAuditEntity, IMayHaveTenant
    {
        public int? TenantId { get; set; }
        public long PayrollId { get; set; }
        [ForeignKey(nameof(PayrollId))]
        public Payroll Payroll { get; set; }
        public long EmployeeId { get; set; }
        [ForeignKey(nameof(EmployeeId))]
        public Employee Employee { get; set; }
        public long BranchId { get; set; }
        public UserType UserType { get; set; }
        public long LevelId { get; set; }
        public long JobPositionId { get; set; }
        public float RemainLeaveDayBefore { get; set; }
        public float RemainLeaveDayAfter { get; set; }
        public float AddedLeaveDay{ get; set; }
        public float NormalDay { get; set; }
        public float OTHour { get; set; }//đã nhân hệ số
        public float RefundLeaveDay { get; set; }
        public int OpentalkCount { get; set; }
        public float WorkAtOfficeOrOnsiteDay { get; set; }
        public float OffDay { get; set; }
        public double Salary { get; set; }
        public PayslipConfirmStatus ConfirmStatus { get; set; }
        public string ComplainNote { get; set; }
        public DateTime? ComplainDeadline { get; set; }
        public long? BankId { get; set; }
        [ForeignKey(nameof(BankId))]
        public Bank Bank { get; set; }
        public string BankAccountNumber { get; set; }
        public string BankName { get; set; }
    }
}