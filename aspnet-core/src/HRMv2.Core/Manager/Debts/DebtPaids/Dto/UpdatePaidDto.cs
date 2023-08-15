using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using HRMv2.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HRMv2.Constants.Enum.HRMEnum;

namespace HRMv2.Manager.Debts.PaidsManagger.Dto
{
    [AutoMapTo(typeof(DebtPaid))]
    public class UpdatePaidDto:EntityDto<long>
    {
        public long DebtId { get; set; }
        public DateTime Date { get; set; }
        public long? UserSalaryId { get; set; }
        public DebtPaymentType PaymentType { get; set; }
        public double Money { get; set; }
        public string Note { get; set; }
    }
}
