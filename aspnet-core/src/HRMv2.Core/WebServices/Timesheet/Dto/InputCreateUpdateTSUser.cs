using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HRMv2.Constants.Enum.HRMEnum;

namespace HRMv2.WebServices.Timesheet.Dto
{
    public class InputCreateUpdateTSUser
    {
        public string UserName { get; set; }
        public string Name { get; set; }
        public string EmailAddress { get; set; }
        public string Surname { get; set; }
        public string PhoneNumber { get; set; }
        public Sex Sex { get; set; }
        public string Branch { get; set; }
        public string Level { get; set; }
        public UserType Type { get; set; }
        public string UserCode { get; set; }
        public bool IsActive { get; set; }
        public string BranchCode { get; set; }
    }
}
