using HRMv2.Manager.Common.Dto;
using HRMv2.Utils;
using NccCore.Paging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HRMv2.Constants.Enum.HRMEnum;

namespace HRMv2.Manager.Salaries.Payslips.Dto
{
    public class GetPayslipEmployeeDto : BaseEmployeeDto
    {
        public double RealSalary { get; set; }
        public float RemainLeaveDays { get; set; }
        public string PayslipBanrch { get; set; }
        public UserType PayslipUserType { get; set; }
        public string PayslipUserTypeName => CommonUtil.GetUserTypeNameVN(PayslipUserType);
        public string PayslipLevel { get; set; }
        public string PayslipPosition { get; set; }
        public long EmployeeId { get; set; }
        public double NormalSalary { get; set; }
        public double OTSalary { get; set; }
        public double MaternityLeaveSalary { get; set; }
        public double TotalBonus { get; set; }
        public double TotalPunishment { get; set; }
        public double TotalDebt { get;set; }
        public float NormalDay { get; set; }
        public float OTHour { get; set; }
        public float OffDay { get; set; }
        public float LeaveDayBefore { get; set; }
        public float AddedLeaveDay { get; set; }
        public float RefundLeaveDay { get; set; }
        public float StandardNormalDay { get; set; }
        public float StandardOpentalk { get; set; }
        public DateTime UpdatedTime => LastModificationTime.HasValue ? LastModificationTime.Value : CreationTime;
        public string UpdatedUserName => LastModifierName != string.Empty ? LastModifierName : CreatorName;

        public DateTime CreationTime { get; set; }
        public DateTime? LastModificationTime { get; set; }
        public string CreatorName { get; set; }
        public string LastModifierName { get; set; }

        public PayslipConfirmStatus ConfirmStatus { get; set; }
        public string ComplainNote { get; set; }
        public int OpentalkCount { get; set; }
        public DateTime? ComplainDeadline { get; set; }
        public BankInfoDto BankInfo { get; set; }
        public List<BenefitPayslipDetailDto> ListBenefit { get; set; }
        public List<InputsalaryDto> ListInputSalary { get; set; }
    }

    public class InputGetPayslipEmployeeDto
    {
        public GridParam GridParam { get; set; }
        public List<long> TeamIds { get; set; }
        public bool IsAndCondition { get; set; }
        public List<EmployeeStatus> StatusIds { get; set; }
        public List<long> LevelIds { get; set; }
        public List<UserType> Usertypes { get; set; }
        public List<long> BranchIds { get; set; }
        public List<long> JobPositionIds { get; set; }
    }

    public class BenefitPayslipDetailDto
    {
        public double Money { get; set; }
        public string Note { get; set; }
    }
    public class InputsalaryDto
    {
        public double Salary { get; set; }
        public DateTime FromDate { get; set; }
        public string Note { get; set; }
    }

    public class BankInfoDto
    {
        public string BankName { get; set; }
        public string BankAccountNumber { get; set; }
    }
}
