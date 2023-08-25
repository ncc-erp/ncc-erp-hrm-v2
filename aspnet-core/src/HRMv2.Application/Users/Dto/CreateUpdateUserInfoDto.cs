using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using HRMv2.Authorization.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HRMv2.Constants.Enum.HRMEnum;

namespace HRMv2.Users.Dto
{
    public class CreateUpdateUserInfoDto
    {
        [AutoMap(typeof(User))]
        public class CreateUpdateUserDto : EntityDto<long>
        {
            public string Name { get; set; }
            public string UserName { get; set; }

            public string Surname { get; set; }
           
            public string EmailAddress { get; set; }
            public string[] RoleNames { get; set; }

        }

        public class UpdateUserRoleDto
        {
            public long UserId { get; set; }
            public string[] RoleNames { get; set; }

        }
    }
}
