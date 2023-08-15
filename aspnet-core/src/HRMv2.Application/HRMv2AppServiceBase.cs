using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Abp.Application.Services;
using Abp.IdentityFramework;
using Abp.Runtime.Session;
using HRMv2.Authorization.Users;
using HRMv2.MultiTenancy;
using HRMv2.NccCore;

namespace HRMv2
{
    /// <summary>
    /// Derive your application services from this class.
    /// </summary>
    public abstract class HRMv2AppServiceBase : ApplicationService
    {
        public TenantManager TenantManager { get; set; }
        public UserManager UserManager { get; set; }

        protected HRMv2AppServiceBase()
        {
            LocalizationSourceName = HRMv2Consts.LocalizationSourceName;
        }

        protected virtual async Task<User> GetCurrentUserAsync()
        {
            var user = await UserManager.FindByIdAsync(AbpSession.GetUserId().ToString());
            if (user == null)
            {
                throw new Exception("There is no current user!");
            }

            return user;
        }

        protected virtual Task<Tenant> GetCurrentTenantAsync()
        {
            return TenantManager.GetByIdAsync(AbpSession.GetTenantId());
        }

        protected virtual void CheckErrors(IdentityResult identityResult)
        {
            identityResult.CheckErrors(LocalizationManager);
        }
    }
}
