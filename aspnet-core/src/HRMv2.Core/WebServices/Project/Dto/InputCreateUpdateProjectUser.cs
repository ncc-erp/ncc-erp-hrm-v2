using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HRMv2.Constants.Enum.HRMEnum;

namespace HRMv2.WebServices.Project.Dto
{
    public class InputCreateUpdateProjectUser
    {
        public string UserName { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string EmailAddress { get; set; }
        public string UserCode { get; set; }
        public UserType UserType { get; set; }
        public string UserLevel { get; set; }
        public string Branch { get; set; }
        public string BranchCode { get; set; }
    }
}
