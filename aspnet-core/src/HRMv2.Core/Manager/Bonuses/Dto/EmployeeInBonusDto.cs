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
        public float BonusXMonth { get; set; }
        public string EmailAddressToLowerTrim => EmailAddress.ToLower().Trim();
    }
}
