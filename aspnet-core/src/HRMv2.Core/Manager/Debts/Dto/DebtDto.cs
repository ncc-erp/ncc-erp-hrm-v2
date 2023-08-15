using Abp.AutoMapper;
using HRMv2.Entities;
using HRMv2.Manager.Common.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HRMv2.Constants.Enum.HRMEnum;

namespace HRMv2.Manager.Debts.Dto
{
    [AutoMapTo(typeof(Debt))]
    public class DebtDto : BaseEmployeeDto
    {
        public long? EmployeeId { get; set; }
        public double InterestRate { get; set; }
        public double Money { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Note { get; set; }
        public DebtPaymentType PaymentType { get; set; }
        public DebtStatus DebtStatus { get; set; }
        public double TotalPaid { get; set; }
        public DateTime? UpdatedTime { get; set; }
        public string UpdatedUser { get; set; }
        public string CreatorUser { get; set; }
        public DateTime CreationTime { get; set; }
        public string IdCard { get; set; }
        public string IssuedBy { get; set; }
        public DateTime? IssuedOn { get; set; }

    }
}
