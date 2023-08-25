using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HRMv2.Constants.Enum.HRMEnum;

namespace HRMv2.Manager.Salaries.Payslips.Dto
{
    public class GetPayslipDetailDto
    {
        public List<PayslipContractSalaryDto> InputSalary { get; set; }
        public CalculateResultDto CalculateResult { get; set; }
        public DateTime ParollMonth { get; set; }
        public string EmployeeFullName { get; set; }
        public double TotalRealSalary { get; set; }
        public double LeaveDayAfter { get; set; }
        public float StandardWorkingDay { get; set; }
        public int StandardOpenTalk { get; set; }
        public float LeaveDayBefore { get; set; }
        public float MonthlyAddedLeaveDay { get; set; }
        public float NormalworkingDay { get; set; }
        public int OpenTalkCount { get; set; }
        public float EmployeeTotalDay => NormalworkingDay + Math.Min(OpenTalkCount * 0.5f, 1);
        public float TotalStandardDay => StandardWorkingDay + (StandardOpenTalk * 0.5f);
        public float OffDay { get; set; }
        public float OTHour { get; set; }
        public float RefundLeaveDay { get; set; }
        public float WorkAtOfficeOrOnsiteDay { get; set; }
        public PayslipConfirmStatus ConfirmStatus { get; set; }
        public string ComplainNote { get; set; }
        public DateTime? ComplainDeadline { get; set; }
    }

    public class PayslipContractSalaryDto
    {
        public DateTime FromDate { get; set; }
        public double Salary { get; set; }
        public string Note { get; set; }
    }



    public class PayslipCollectedDataDto
    {
        public float LeaveDayBefore { get; set; }
        public float MonthlyAddedLeaveDay { get; set; }
        public float LeaveDayAfter { get; set; }
        public float NormalworkingDay { get; set; }
        public int OpenTalkCount { get; set; }
        public float TotalDay => NormalworkingDay + (OpenTalkCount >= 2 ? OpenTalkCount * 0.5f : 1);
        public float OffDay { get; set; }
        public float OTHour { get; set; }
        public float NonSalaryOffDay { get; set; }
        public float WorkAtOfficeOrOnsiteDay { get; set; }
    }

    public class CalculateResultDto
    {
        public double NormalSalary { get; set; }
        public double OTSalary { get; set; }
        public double MaternityLeaveSalary { get; set; }
        public double TotalBenefit { get; set; }
        public double TotalBonus { get; set; }
        public double TotalPunishment { get; set; }
        public double TotalDebt { get; set; }
        public double TotalRefund { get; set; }
    }
}
