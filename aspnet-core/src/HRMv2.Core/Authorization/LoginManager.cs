using Microsoft.AspNetCore.Identity;
using Abp.Authorization;
using Abp.Authorization.Users;
using Abp.Configuration;
using Abp.Configuration.Startup;
using Abp.Dependency;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Zero.Configuration;
using HRMv2.Authorization.Roles;
using HRMv2.Authorization.Users;
using HRMv2.MultiTenancy;
using Abp.UI;
using System.Threading.Tasks;
using Castle.Core.Logging;
using Abp.Extensions;
using System;
using Newtonsoft.Json;
using HRMv2.Configuration;
using Google.Apis.Auth;
using System.Linq;
using HRMv2.Manager.Employees;
using static HRMv2.Constants.Enum.HRMEnum;
using System.Collections.Generic;

namespace HRMv2.Authorization
{
    public class LogInManager : AbpLogInManager<Tenant, Role, User>
    {
        private ILogger Logger { get; set; }
        private readonly EmployeeManager _employeeManager;
        public LogInManager(
            UserManager userManager,
            IMultiTenancyConfig multiTenancyConfig,
            IRepository<Tenant> tenantRepository,
            IUnitOfWorkManager unitOfWorkManager,
            ISettingManager settingManager,
            IRepository<UserLoginAttempt, long> userLoginAttemptRepository,
            IUserManagementConfig userManagementConfig,
            IIocResolver iocResolver,
            IPasswordHasher<User> passwordHasher,
            RoleManager roleManager,
            UserClaimsPrincipalFactory claimsPrincipalFactory,
            EmployeeManager employeeManager)
            : base(
                  userManager,
                  multiTenancyConfig,
                  tenantRepository,
                  unitOfWorkManager,
                  settingManager,
                  userLoginAttemptRepository,
                  userManagementConfig,
                  iocResolver,
                  passwordHasher,
                  roleManager,
                  claimsPrincipalFactory)

        {
            Logger = NullLogger.Instance;
            _employeeManager = employeeManager;
        }
        [UnitOfWork]
        public async Task<AbpLoginResult<Tenant, User>> LoginAsyncNoPass(string token, string secretCode = "", string tenancyName = null, bool shouldLockout = true)
        {
            Logger.Info("LoginAsyncNoPass");
            var result = await LoginAsyncInternalNoPass(token, secretCode, tenancyName, shouldLockout);
            var user = result.User;
            SaveLoginAttempt(result, tenancyName, user == null ? null : user.EmailAddress);
            return result;
        }

        public async Task<AbpLoginResult<Tenant, User>> LoginAsyncInternalNoPass(string token, string secretCode, string tenancyName, bool shouldLockout)
        {
            Logger.Info("LoginAsyncInternalNoPass");
            if (token.IsNullOrEmpty())
            {
                throw new ArgumentNullException(nameof(token));
            }
            try
            {
                GoogleJsonWebSignature.Payload payload = await GoogleJsonWebSignature.ValidateAsync(token);
                var emailAddress = payload.Email;
                Logger.Info("Payload: " + JsonConvert.SerializeObject(payload));
                // checking
                var clientAppId = await SettingManager.GetSettingValueAsync(AppSettingNames.GoogleClientId);//get clientAppId from setting
                Logger.Info("ClientAppId: " + clientAppId);
                var correctAudience = payload.AudienceAsList.Any(s => s == clientAppId);
                var correctIssuer = payload.Issuer == "accounts.google.com" || payload.Issuer == "https://accounts.google.com";
                var correctExpriryTime = payload.ExpirationTimeSeconds != null || payload.ExpirationTimeSeconds > 0;

                Tenant tenant = null;

                Logger.Info("correctAudience: " + correctAudience + ", correctIssuer: " + correctIssuer + ", correctExpriryTime: " + correctExpriryTime);
                if (correctAudience && correctIssuer && correctExpriryTime)
                {
                    //Get and check tenant
                    using (UnitOfWorkManager.Current.SetTenantId(null))
                    {
                        if (!MultiTenancyConfig.IsEnabled)
                        {
                            tenant = await GetDefaultTenantAsync();
                        }
                        else if (!string.IsNullOrWhiteSpace(tenancyName))
                        {
                            tenant = await TenantRepository.FirstOrDefaultAsync(t => t.TenancyName == tenancyName);
                            if (tenant == null)
                            {
                                return new AbpLoginResult<Tenant, User>(AbpLoginResultType.InvalidTenancyName);
                            }

                            if (!tenant.IsActive)
                            {
                                return new AbpLoginResult<Tenant, User>(AbpLoginResultType.TenantIsNotActive, tenant);
                            }
                        }
                    }
                    var tenantId = tenant == null ? (int?)null : tenant.Id;
                    using (UnitOfWorkManager.Current.SetTenantId(tenantId))
                    {
                        await UserManager.InitializeOptionsAsync(tenantId);

                        var user = await UserManager.FindByNameOrEmailAsync(tenantId, emailAddress);
                        if (user == null)
                        {
                            var employee = _employeeManager.GetEmployeeByEmail(emailAddress);
                            if (employee.Status == EmployeeStatus.Working || employee.Status == EmployeeStatus.MaternityLeave)
                            {

                                user = await CreateUserAsync(emailAddress, tenantId, Utils.CommonUtil.GetNameByFullName(employee.FullName), Utils.CommonUtil.GetSurNameByFullName(employee.FullName));
                            }
                            else
                            {

                                throw new UserFriendlyException(string.Format("Login Fail - Account does not exist"));
                            }
                        }

                        if (await UserManager.IsLockedOutAsync(user))
                        {
                            return new AbpLoginResult<Tenant, User>(AbpLoginResultType.LockedOut, tenant, user);
                        }
                        if (shouldLockout)
                        {
                            if (await TryLockOutAsync(tenantId, user.Id))
                            {
                                return new AbpLoginResult<Tenant, User>(AbpLoginResultType.LockedOut, tenant, user);
                            }
                        }

                        await UserManager.ResetAccessFailedCountAsync(user);
                        return await CreateLoginResultAsync(user, tenant);
                    }
                }
                else
                {
                    return new AbpLoginResult<Tenant, User>(AbpLoginResultType.InvalidUserNameOrEmailAddress, null);
                }
            }
            catch (InvalidJwtException e)
            {
                return new AbpLoginResult<Tenant, User>(AbpLoginResultType.InvalidUserNameOrEmailAddress, null);
            }
        }
        private async Task<User> CreateUserAsync(string emailAddress, int? tenantId, string name, string surname)
        {
            var user = new User
            {
                TenantId = tenantId,
                EmailAddress = emailAddress,
                UserName = emailAddress,
                Name = name,
                Surname = surname,
                Roles = new List<UserRole>(),
                Password ="",
            };
            user.SetNormalizedNames();
            var role = await RoleManager.GetRoleByNameAsync(StaticRoleNames.Tenants.Employee);
            if (role == null)
            {
                throw new UserFriendlyException("Role not found");
            }

            user.Roles.Add(new UserRole
            {
                TenantId = tenantId,
                RoleId = role.Id,
                UserId = user.Id
            });
            await UserManager.CreateAsync(user);
            return user;
        }
    }
}
