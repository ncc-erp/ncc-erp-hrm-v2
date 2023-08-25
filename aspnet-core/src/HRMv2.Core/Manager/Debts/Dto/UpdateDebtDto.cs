using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using HRMv2.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HRMv2.Constants.Enum.HRMEnum;

namespace HRMv2.Manager.Debts.Dto
{
    [AutoMapTo(typeof(Debt))]
    public class UpdateDebtDto: EntityDto<long>
    {
        public long? EmployeeId { get; set; }
        public double InterestRate { get; set; }
        public double Money { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Note { get; set; }
        public DebtPaymentType PaymentType { get; set; }
        public DebtStatus DebtStatus { get; set; }
    }
}
