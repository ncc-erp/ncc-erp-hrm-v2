using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using HRMv2.Authorization.Roles;
using System;
using System.Collections.Generic;
using System.Text;

namespace HRMv2.Roles.Dto
{
    [AutoMap(typeof(Role))]
    public class RolePermissionDto : EntityDto<int>
    {
        public List<string> Permissions { get; set; }
    }
}
