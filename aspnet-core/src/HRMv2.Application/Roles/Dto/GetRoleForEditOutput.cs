using System.Collections.Generic;
using static HRMv2.Authorization.PermissionNames;

namespace HRMv2.Roles.Dto
{
    public class GetRoleForEditOutput
    {
        public RoleEditDto Role { get; set; }

        public List<SystemPermission> Permissions { get; set; }

        public List<string> GrantedPermissionNames { get; set; }
    }
}