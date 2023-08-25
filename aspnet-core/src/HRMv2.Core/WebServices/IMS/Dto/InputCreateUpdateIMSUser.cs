using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMv2.WebServices.IMS.Dto
{
    public class InputCreateUpdateIMSUser
    {
        public string EmailAddress { get; set; }
        public string Name { get; set; }
        public string UserCode { get; set; }
        public bool IsActive { get; set; }
        public string Surname { get; set; }
        public string UserName { get; set; }
    }
}
