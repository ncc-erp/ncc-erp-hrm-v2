using Abp.AspNetCore.Mvc.Controllers;
using Abp.IdentityFramework;
using Microsoft.AspNetCore.Identity;

namespace HRMv2.Controllers
{
    public abstract class HRMv2ControllerBase: AbpController
    {
        protected HRMv2ControllerBase()
        {
            LocalizationSourceName = HRMv2Consts.LocalizationSourceName;
        }

        protected void CheckErrors(IdentityResult identityResult)
        {
            identityResult.CheckErrors(LocalizationManager);
        }
    }
}
