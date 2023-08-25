using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMv2.WebServices.IMS.Dto
{
    public class UpdateIMSUserStatusDto
    {
        public string EmailAddress { get; set; }
        public bool IsActive { get; set; }  
    }
}
