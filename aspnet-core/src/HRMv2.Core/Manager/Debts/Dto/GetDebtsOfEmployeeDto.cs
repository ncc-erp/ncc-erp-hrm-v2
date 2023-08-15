using Abp.Application.Services.Dto;
using NccCore.Paging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HRMv2.Constants.Enum.HRMEnum;

namespace HRMv2.Manager.Debts.Dto
{
    public class GetDebtsOfEmployeeDto:EntityDto<long>
    {
        public long? EmployeeId { get; set; }
        public double InterestRate { get; set; }
        public double Money { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Note { get; set; }
        public DebtPaymentType PaymentType { get; set; }
        public DebtStatus Status { get; set; }
        public double TotalPaid { get; set; }
        public string PaymentMethodName { get {
                return PaymentType switch
                {
                    DebtPaymentType.Salary => "Trừ lương",
                    DebtPaymentType.RealMoney => "Tiền mặt",
                    _ => "",
                };
            } }
    }

    public class GetDebtEmployeeInputDto
    {
        public GridParam GridParam { get; set; }
        public List<long> TeamIds { get; set; }
        public bool IsAndCondition { get; set; }
        public List<EmployeeStatus> StatusIds { get; set; }
        public List<long> LevelIds { get; set; }
        public List<UserType> UserTypes { get; set; }
        public List<long> BranchIds { get; set; }
        public List<long> JobPositionIds { get; set; }
        public List<long> DebtStatusIds { get; set; }
        public List<long> PaymentTypeIds { get; set; }
    }

    public class QueryEmployeeHaveAnyTeamsDto
    {
        public long? EmployeeId { get; set; }
    }

    public class GetDebtEmployeeDto
    {
        public string Email { get; set; }
        public string FullName { get; set; }
        public double Money { get; set; }
        public DateTime StartDate { get; set; }
        public double InterestRate { get; set; }
        public string Note { get; set; }
    }

    public class GetAllDebtEmployeeDto
    {
        public List<GetDebtEmployeeDto> ListDebtEmployees { get; set; }
        public double TotalLoan { get; set; }
        public int EmployeeCount { get; set; }
    }

}
