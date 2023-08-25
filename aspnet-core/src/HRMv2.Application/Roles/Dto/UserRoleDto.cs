using System;
using System.Collections.Generic;
using System.Text;
using static HRMv2.Constants.Enum.HRMEnum;

namespace HRMv2.Roles.Dto
{
    public class UserRoleDto
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        public string UserName { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string AvatarPath { get; set; }
    }
}
