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
using HRMv2.WebServices.Mezon.Dto;
using Microsoft.Extensions.Configuration;
using HRMv2.WebServices;
using Microsoft.Extensions.Logging;
using HRMv2.Helper;
using System.Text;
using System.Net.Mail;
using System.Web;

namespace HRMv2.Authorization
{
    public class LogInManager : AbpLogInManager<Tenant, Role, User>
    {
        private ILogger<BaseWebService> Logger;
        private readonly EmployeeManager _employeeManager;
        private readonly UserManager _userManager;
        private readonly IConfiguration _configuration;
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
        }
        [UnitOfWork]
        public async Task<AbpLoginResult<Tenant, User>> LoginAsyncNoPass(string token, string tenancyName = null, bool shouldLockout = true)
        {
            Logger.LogInformation("LoginAsyncNoPass");
            var result = await LoginAsyncInternalNoPass(TypeLoginOuth2.Google, token, tenancyName, shouldLockout, null);
            var user = result.User;
            SaveLoginAttempt(result, tenancyName, user == null ? null : user.EmailAddress);
            return result;
        }

        [UnitOfWork]
        public async Task<AbpLoginResult<Tenant, User>> LoginHashMezonAsync(MezonHashAuthDto hashAuthDto, string tenancyName)
        {
            var result = await AuthMezonHashAsync(hashAuthDto);
            var user = result.User;
            SaveLoginAttempt(result, tenancyName, user == null ? null : user.EmailAddress);
            return result;
        }

        [UnitOfWork]
        public async Task<AbpLoginResult<Tenant, User>> LoginAsyncNoPassWithMezon(AuthOauth2Mezon input, string tenancyName = null, bool shouldLockout = true)
        {
            Logger.LogInformation("LoginAsyncNoPassWithMezon");
            var result = await LoginAsyncInternalNoPass(TypeLoginOuth2.Mezon, null, tenancyName, shouldLockout, input);
            var user = result.User;
            SaveLoginAttempt(result, tenancyName, user == null ? null : user.EmailAddress);
            return result;
        }

        private async Task<AbpLoginResult<Tenant, User>> AuthMezonHashAsync(MezonHashAuthDto hashAuthDto)
        {
            if (hashAuthDto.HashData.IsNullOrEmpty() )
            {
                throw new ArgumentNullException(nameof(hashAuthDto.HashData));
            }

            try
            {
                var appToken = _configuration.GetValue<string>("Oauth2Mezon:MezonAppToken") ?? throw new ArgumentNullException("Invalid AppToken ");

                var rawHashData = Hasher.DecodeBase64(hashAuthDto.HashData);
                var hashData = HashParamsParser(rawHashData);

                var hashParams = new BaseHashData
                {
                    query_id = hashData.query_id,
                    user = hashData.user,
                    auth_date = hashData.auth_date,
                    signature = hashData.signature,

                };

                var mezonUser = JsonConvert.DeserializeObject<MezonUser>(hashParams.user);

                byte[] secretKey = Hasher.HMAC_SHA256(Encoding.UTF8.GetBytes(appToken), Encoding.UTF8.GetBytes("WebAppData"));

                var hasherData = Hasher.HEX(Hasher.HMAC_SHA256(secretKey,Encoding.UTF8.GetBytes(HashParamStringify(hashParams))));

                if (hashData.hash.Equals(hasherData) == false)
                {
                    throw new UserFriendlyException("Authentication failed - Invalid hash");
                }

                Tenant tenant = null;

                //Get and check tenant
                using (UnitOfWorkManager.Current.SetTenantId(null))
                {
                    if (!MultiTenancyConfig.IsEnabled)
                    {
                        tenant = await GetDefaultTenantAsync();
                    }
                    else if (!string.IsNullOrWhiteSpace(hashAuthDto.TenancyName))
                    {
                        tenant = await TenantRepository.FirstOrDefaultAsync(t => t.TenancyName == hashAuthDto.TenancyName);
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

                    var user = await UserManager.FindByNameOrEmailAsync(tenantId, mezonUser.mezon_id);
                    if (user == null)
                    {
                        var employee = _employeeManager.GetEmployeeByEmail(mezonUser.mezon_id);
                        if (employee == null)
                        {
                            throw new UserFriendlyException("Login Fail - Not found employee with email " + mezonUser.mezon_id);
                        }
                        if (employee.Status != EmployeeStatus.Working && employee.Status != EmployeeStatus.MaternityLeave)
                        {
                            throw new UserFriendlyException(string.Format("Login Fail - " + mezonUser.mezon_id + "is not working or maternity leave "));
                        }

                        user = await _userManager.CreateUserAsync(mezonUser.mezon_id, tenantId, Utils.CommonUtil.GetNameByFullName(employee.FullName), Utils.CommonUtil.GetSurNameByFullName(employee.FullName));
                    }


                    if (await UserManager.IsLockedOutAsync(user))
                    {
                        return new AbpLoginResult<Tenant, User>(AbpLoginResultType.LockedOut, tenant, user);
                    }

                    await UserManager.ResetAccessFailedCountAsync(user);
                    return await CreateLoginResultAsync(user, tenant);
                }

            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Authentication failed - Can't authenticate with Mezon server");
                return new AbpLoginResult<Tenant, User>(AbpLoginResultType.UnknownExternalLogin, null);
            }
        }

        private HashData HashParamsParser(string queryString)
        {
            var queryParams = HttpUtility.ParseQueryString(queryString);
            var hashData = new HashData()
            {
                query_id = queryParams["query_id"],
                user = queryParams["user"],
                auth_date = long.Parse(queryParams["auth_date"]),
                hash = queryParams["hash"],
                signature = queryParams["signature"]
            };
            return hashData;
        }

        private string HashParamStringify(object hashData)
        {
            var queryString = new StringBuilder();
            var properties = hashData.GetType().GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
            foreach( var property in properties)
            {
                var value = property.GetValue(hashData);
                if(value != null)
                {
                    if(queryString.Length > 0)
                        queryString.Append("&");
                    queryString.AppendFormat($"{Uri.EscapeDataString(property.Name)}={Uri.EscapeDataString(value.ToString())}");
                }
            }
            return queryString.ToString();  
        }

        


        public async Task<AbpLoginResult<Tenant, User>> LoginAsyncInternalNoPass(TypeLoginOuth2 type,string token, string tenancyName, bool shouldLockout,AuthOauth2Mezon input)
        {
            Logger.LogInformation("LoginAsyncInternalNoPass");
            try
            {
                var emailAddress = "";
                var clientAppId = "";
                var correctAudience = false;
                var correctIssuer = false;
                var correctExpriryTime = false;

                if (type == TypeLoginOuth2.Google)
                {
                    if (token.IsNullOrEmpty())
                    {
                        throw new ArgumentNullException(nameof(token));
                    }
                    GoogleJsonWebSignature.Payload payload = await GoogleJsonWebSignature.ValidateAsync(token);
                     emailAddress = payload.Email;
                    Logger.LogInformation("Payload: " + JsonConvert.SerializeObject(payload));
                    // checking
                     clientAppId = await SettingManager.GetSettingValueAsync(AppSettingNames.GoogleClientId);//get clientAppId from setting
                     Logger.LogInformation("ClientAppId: " + clientAppId);
                     correctAudience = payload.AudienceAsList.Any(s => s == clientAppId);
                     correctIssuer = payload.Issuer == "accounts.google.com" || payload.Issuer == "https://accounts.google.com";
                     correctExpriryTime = payload.ExpirationTimeSeconds != null || payload.ExpirationTimeSeconds > 0;
                }else if(type == TypeLoginOuth2.Mezon)
                {
                    emailAddress = input.sub;
                    clientAppId = _configuration.GetValue<string>("Oauth2Mezon:Client_Id");
                    correctAudience = input.aud.Any(s => s == clientAppId);
                    correctIssuer =  input.iss == "https://oauth2.mezon.ai";
                    correctExpriryTime = input.auth_time != null || input.auth_time > 0;
                }
                
                Tenant tenant = null;

                Logger.LogInformation("correctAudience: " + correctAudience + ", correctIssuer: " + correctIssuer + ", correctExpriryTime: " + correctExpriryTime);
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
                            if (employee == null)
                            {
                                throw new UserFriendlyException("Login Fail - Not found employee with email " + emailAddress);
                            }
                            if (employee.Status != EmployeeStatus.Working && employee.Status != EmployeeStatus.MaternityLeave)
                            {
                                throw new UserFriendlyException(string.Format("Login Fail - " + emailAddress + "is not working or maternity leave "));
                            }

                            user = await _userManager.CreateUserAsync(emailAddress, tenantId, Utils.CommonUtil.GetNameByFullName(employee.FullName), Utils.CommonUtil.GetSurNameByFullName(employee.FullName));
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
    }
}
