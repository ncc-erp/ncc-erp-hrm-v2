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
using Abp.Extensions;
using System;
using Newtonsoft.Json;
using HRMv2.Configuration;
using Google.Apis.Auth;
using System.Linq;
using HRMv2.Manager.Employees;
using static HRMv2.Constants.Enum.HRMEnum;
using HRMv2.WebServices.Mezon.Dto;
using Microsoft.Extensions.Configuration;
using HRMv2.WebServices;
using Microsoft.Extensions.Logging;
using HRMv2.NccCore;

namespace HRMv2.Authorization
{
    public class LogInManager : AbpLogInManager<Tenant, Role, User>
    {
        private ILogger<BaseWebService> Logger;
        private readonly EmployeeManager _employeeManager;
        private readonly UserManager _userManager;
        private readonly IConfiguration _configuration;
        private readonly IWorkScope _workScope;
        private readonly IUnitOfWork _unitOfWork;
  
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
            IConfiguration configuration,
            IWorkScope workScope,
            IUnitOfWork unitOfWork,
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
            Logger = IocManager.Instance.Resolve<ILogger<BaseWebService>>();
            _employeeManager = employeeManager;
            _userManager = userManager;
            _configuration = configuration;
            _unitOfWork = unitOfWork;
            _workScope = workScope;
        }
        [UnitOfWork]
        public async Task<AbpLoginResult<Tenant, User>> LoginAsyncNoPass(string token, string tenancyName = null, bool shouldLockout = true)
        {
            Logger.LogInformation("LoginAsyncNoPass");
            var result = await LoginAsyncInternalNoPass(TypeLoginOuth2.Google,token, tenancyName, shouldLockout,null);
            var user = result.User;
            SaveLoginAttempt(result, tenancyName, user == null ? null : user.EmailAddress);
            return result;
        }
        [UnitOfWork]
        public async Task<AbpLoginResult<Tenant,User>> LoginAsyncNoPassWithMezon(AuthOauth2Mezon mezonOauthResult,string tenancyName = null , bool shouldLockout = true)
        {
            Logger.LogInformation("LoginAsyncNoPassWithMezon");
            var result = await LoginAsyncInternalNoPass(TypeLoginOuth2.Mezon,null,tenancyName, shouldLockout,mezonOauthResult);
            var user = result.User;
            SaveLoginAttempt(result,tenancyName,user == null ? null : user.EmailAddress);
            return result;
        }

        public async Task<AbpLoginResult<Tenant, User>> LoginAsyncInternalNoPass(TypeLoginOuth2 type, string token, string tenancyName, bool shouldLockout, AuthOauth2Mezon mezonOauthResult)
        {
            Logger.LogInformation("LoginAsyncInternalNoPass");
            try
            {
                var emailAddress = "";
                var clientAppId = "";
                var correctAudience = false;
                var correctIssuer = false;
                var correctExpriryTime = false;
                var userMezonId = "";

                if (type == TypeLoginOuth2.Google)
                {
                    if (token.IsNullOrEmpty())
                    {
                        throw new ArgumentNullException(nameof(token));
                    }
                    GoogleJsonWebSignature.Payload payload = await GoogleJsonWebSignature.ValidateAsync(token);
                    emailAddress = payload.Email;
                    Logger.LogInformation("LoginAsyncInternalNoPass Payload: " + JsonConvert.SerializeObject(payload));
                    // checking
                    clientAppId = await SettingManager.GetSettingValueAsync(AppSettingNames.GoogleClientId);//get clientAppId from setting
                    Logger.LogInformation("ClientAppId: " + clientAppId);
                    correctAudience = payload.AudienceAsList.Any(s => s == clientAppId);
                    correctIssuer = payload.Issuer == "accounts.google.com" || payload.Issuer == "https://accounts.google.com";
                    correctExpriryTime = payload.ExpirationTimeSeconds.HasValue && payload.ExpirationTimeSeconds > 0;
                }
                else if (type == TypeLoginOuth2.Mezon)
                {
                    Logger.LogInformation("LoginAsyncInternalNoPass mezonOauthResult: " + JsonConvert.SerializeObject(mezonOauthResult));
                    emailAddress = mezonOauthResult.sub;
                    userMezonId = mezonOauthResult.user_id;
                    clientAppId = _configuration.GetValue<string>("Oauth2Mezon:Client_Id");
                    correctAudience = mezonOauthResult.aud.Any(s => s == clientAppId);
                    correctIssuer = mezonOauthResult.iss == "https://oauth2.mezon.ai";
                    correctExpriryTime = mezonOauthResult.auth_time > 0;
                }

                Tenant tenant = null;
               
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

                    var user = type == TypeLoginOuth2.Mezon ? GetUserByMezonUserId(userMezonId): await UserManager.FindByEmailAsync(emailAddress);                        

                    if (user == null)
                    {
                        return new AbpLoginResult<Tenant, User>(AbpLoginResultType.InvalidUserNameOrEmailAddress, tenant, user);
                    }

                    if (type == TypeLoginOuth2.Mezon 
                        && emailAddress.ToLower().Contains("@ncc.asia") 
                        && emailAddress.ToLower() != user.EmailAddress.ToLower())
                    {
                        throw new UserFriendlyException($"Login lỗi do nhầm thông tin, Mezon email {emailAddress} != AbpUser email {user.EmailAddress} => Liên hệ HR để update đúng thông tin");
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
            catch (InvalidJwtException e)
            {
                return new AbpLoginResult<Tenant, User>(AbpLoginResultType.InvalidUserNameOrEmailAddress, null);
            }
        }

        private User GetUserByMezonUserId(string mezonUserId)
        {            
            if (string.IsNullOrEmpty(mezonUserId))
            {
                throw new UserFriendlyException("MezonUserId null or empty");
            }
            return _workScope.GetAll<User>()
                .Where(x => x.UserMezonId == mezonUserId)
                .FirstOrDefault();
        }

        [Obsolete("This method is deprecated. Dont use it")]        
        private async Task<User> GetOrCreateUserAsync(string email,string userMezonId, int? tenantId)
        {
            var user = _workScope.GetAll<User>().FirstOrDefault(x => x.UserMezonId == userMezonId);
            
            if (user != null) return user;

            var employeeInfo = _employeeManager.GetWorkingEmployeeByUserMezonId(userMezonId);
            if (employeeInfo != null)
            {
                return await _userManager.CreateUserAsync(
                    email,
                    tenantId,
                    Utils.CommonUtil.GetNameByFullName(employeeInfo.FullName),
                    Utils.CommonUtil.GetSurNameByFullName(employeeInfo.FullName),
                    userMezonId
                );
            }

            user = await UserManager.FindByNameOrEmailAsync(tenantId, email);
            if (user != null)
            {
                user.UserMezonId = userMezonId;
                var employee = _employeeManager.GetEmployeeByEmail(email);
                if (employee != null)
                {
                    employee.UserMezonId = userMezonId;
                }
                _unitOfWork.SaveChanges();
                return user;
            }
            var employeeByEmail = _employeeManager.GetEmployeeByEmail(email);
            
            if (employeeByEmail == null)
            {
                  throw new UserFriendlyException($"Login Fail - Not found employee with email {email}");
            }            

            if (employeeByEmail.Status != EmployeeStatus.Working && employeeByEmail.Status != EmployeeStatus.MaternityLeave)
            {
                  throw new UserFriendlyException($"Login Fail - {email} is not working or maternity leave");
            } 
            
            employeeByEmail.UserMezonId = userMezonId ;
            _unitOfWork.SaveChanges();

            return await _userManager.CreateUserAsync(
                email,
                tenantId,
                Utils.CommonUtil.GetNameByFullName(employeeByEmail.FullName),
                Utils.CommonUtil.GetSurNameByFullName(employeeByEmail.FullName),
                userMezonId
            );
        }
    }
}
