using HRMv2.Constants;
using HRMv2.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HRMv2.Constants.Enum.HRMEnum;

namespace HRMv2.Manager.Notifications.Email.Dto
{
    public class DebtMailTemplateDto
    {
        public string EmployeeFullName { get; set; }
        public string EmployeeIdCard { get; set; }
        public string EmployeeIssuedBy { get; set; }
        public string EmployeeIssuedOn { get; set; }
        public string AmountLoan { get; set; }
        public string AmountLoanInWords => CommonUtil.NumberToText(Convert.ToDouble(AmountLoan));
        public string LoanStartDate { get; set; }
        public string InterestRate { get; set; }
        public List<PaidPlanDto> ListDebtPaymentPlans { get; set; }
        public string DebtPaymentPlans => CommonUtil.GetPaymentPlansToText(ListDebtPaymentPlans);
        public string CompanyName => AppConsts.CompanyName;
        public string Subject => $"[NCC][{EmployeeFullName}] XÁC NHẬN KHOẢN VAY";
        public string SendToEmail { get; set; }


    }

    public class PaidPlanDto
    {
        public long DebtId { get; set; }
        public int Index { get;set; }
        public DateTime Date { get; set; }
        public double Money { get; set; }
        public DebtPaymentType PaymentType { get; set; }
        public string MethodPaidNote => DebtPaymentType.Salary == PaymentType ? @"&nbsp;&nbsp;&nbsp;Trừ vào lương tháng&nbsp;" + Date.ToString("MM/yyyy") : @"&nbsp;&nbsp;&nbsp;Chuyển tiền vào tài khoản công ty ngày&nbsp;" + Date.ToString("dd/MM/yyyy");
    }

    public class GetDebtDto
    {
        public string EmployeeFullName { get; set; }
        public string EmployeeIdCard { get; set; }
        public string EmployeeIssuedBy { get; set; }
        public DateTime? EmployeeIssuedOn { get; set; }
        public double AmountLoan { get; set; }
        public DateTime LoanStartDate { get; set; }
        public double InterestRate { get; set; }
        public string EmployeeEmail { get; set; }
    }
}
