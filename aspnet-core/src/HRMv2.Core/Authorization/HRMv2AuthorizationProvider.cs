using Abp.Authorization;
using Abp.Localization;
using Abp.MultiTenancy;
using static HRMv2.Authorization.PermissionNames;

namespace HRMv2.Authorization
{
    public class HRMv2AuthorizationProvider : AuthorizationProvider
    {
        public override void SetPermissions(IPermissionDefinitionContext context)
        {
            foreach (var permission in SystemPermission.ListPermissions)
            {
                context.CreatePermission(permission.Name, L(permission.DisplayName), multiTenancySides: permission.MultiTenancySides);
            }
        }

        private static ILocalizableString L(string name)
        {
            return new LocalizableString(name, HRMv2Consts.LocalizationSourceName);
        }
    }
}
