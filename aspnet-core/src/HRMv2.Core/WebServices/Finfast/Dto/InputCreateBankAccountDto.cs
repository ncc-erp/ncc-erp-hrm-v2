using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMv2.WebServices.Finfast.Dto
{
    public class InputCreateBankAccountDto
    {
        public string Name { get; set; }
        public string Code { get; set; }
        public string HolderName { get; set; }
        public string BankNumber { get; set; }
    }
}
