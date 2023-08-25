using HRMv2.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMv2.Manager.CheckIn.Dto
{
    public class GetUserInfo
    {
        public string FullName { get; set; }
        public string FirstName => CommonUtil.GetNameByFullName(FullName);
        public string LastName => CommonUtil.GetSurNameByFullName(FullName);
        public string Email { get; set; }
    }
    public class GetUserForCheckInDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
    }
}
