using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMv2.Manager.Bonuses.Dto
{
    public class EmployeeInBonusDto
    {
        public string EmailAddress { get; set; }
        public double Money { get; set; }
        public string Note { get; set; }
    }

    public class ResultSendBonus
    {
        public string EmailAddress { get; set; }
        public string SyncNote { get; set; }
    }
}
