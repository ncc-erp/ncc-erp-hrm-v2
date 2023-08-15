using Abp.Authorization;
using HRMv2.Authorization.Roles;
using HRMv2.Authorization.Users;

namespace HRMv2.Authorization
{
    public class PermissionChecker : PermissionChecker<Role, User>
    {
        public PermissionChecker(UserManager userManager)
            : base(userManager)
        {
        }
    }
}
