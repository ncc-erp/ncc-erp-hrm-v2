using HRMv2.WebServices.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMv2.WebServices.Talent.Dto
{
    public class UpdateTalentUserStatusDto
    {
        public string EmailAddress { get; set; }
        public bool IsActive { get; set; }
    }
}
