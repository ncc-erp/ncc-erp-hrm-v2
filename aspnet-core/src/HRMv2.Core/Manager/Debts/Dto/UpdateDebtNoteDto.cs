using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMv2.Manager.Debts.Dto
{
    public class UpdateDebtNoteDto
    {
        public long DebtId { get; set; }
        public string Note { get; set; }
    }
}
