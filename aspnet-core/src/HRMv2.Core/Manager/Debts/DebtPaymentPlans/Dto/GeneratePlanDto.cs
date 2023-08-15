using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMv2.Manager.Debts.PaymentPlansManager.Dto
{
    public class GeneratePlanDto
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public double Money { get; set; }
        public long DebtId { get; set; }
    }
}
