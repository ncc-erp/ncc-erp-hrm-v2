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
    public class DebtPaidDto:EntityDto<long>
    {
        public long DebtId { get; set; }
        public DateTime Date { get; set; }
        public long? UserSalaryId { get; set; }
        public DebtPaymentType PaymentType { get; set; }
        public string Note { get; set; }
        public bool IsAllowEdit { get; set; }
        public double Money { get; set; }
        public string CreatorUser { get; set; }
        public DateTime CreationTime { get; set; }
        public string UpdatedUser { get; set; }
        public DateTime? UpdatedTime { get; set;}
        public string DisplayDate
        {
            get
            {
                switch (PaymentType)
                {
                    case DebtPaymentType.Salary:
                        {
                            return Date.ToString("MM/yyyy");
                        }
                    case DebtPaymentType.RealMoney:
                        {
                            return Date.ToString("dd/MM/yyyy");
                        }
                }
                return "";
            }
        }
    }
}
