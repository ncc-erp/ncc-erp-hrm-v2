using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Abp.Authorization;
using Abp.Authorization.Users;
using Abp.Configuration;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Organizations;
using Abp.Runtime.Caching;
using HRMv2.Authorization.Roles;
using System.Threading.Tasks;
using Abp.UI;
using System.Linq;
using HRMv2.NccCore;
using System.Linq.Dynamic.Core;

namespace HRMv2.Authorization.Users
{
    public class UserManager : AbpUserManager<Role, User>
    {
        private readonly RoleManager _roleManager;
        private readonly IWorkScope _workScope;
        public UserManager(
          RoleManager roleManager,
          UserStore store,
          IOptions<IdentityOptions> optionsAccessor,
          IPasswordHasher<User> passwordHasher,
          IEnumerable<IUserValidator<User>> userValidators,
          IEnumerable<IPasswordValidator<User>> passwordValidators,
          ILookupNormalizer keyNormalizer,
          IdentityErrorDescriber errors,
          IServiceProvider services,
          ILogger<UserManager<User>> logger,
          IPermissionManager permissionManager,
          IUnitOfWorkManager unitOfWorkManager,
          ICacheManager cacheManager,
          IRepository<OrganizationUnit, long> organizationUnitRepository,
          IRepository<UserOrganizationUnit, long> userOrganizationUnitRepository,
          IOrganizationUnitSettings organizationUnitSettings,
          ISettingManager settingManager,
          IWorkScope workScope,
          IRepository<UserLogin, long> userLoginRepository)
          : base(
              roleManager,
              store,
              optionsAccessor,
              passwordHasher,
              userValidators,
              passwordValidators,
              keyNormalizer,
              errors,
              services,
              logger,
              permissionManager,
              unitOfWorkManager,
              cacheManager,
              organizationUnitRepository,
              userOrganizationUnitRepository,
              organizationUnitSettings,
              settingManager,
              userLoginRepository)
        {
            _roleManager = roleManager;
            _workScope = workScope;
        }

        public async Task UpdateUserActiveAsync(string email, bool isActive)
        {
            var user = await FindByNameOrEmailAsync(email);
            if (user == null)
            {
                Logger.LogInformation("not found user with email " + email);
                return;
            }
            user.IsActive = isActive;
            await UpdateAsync(user);
        }
        public async Task DeleteAsync(string email)
        {

            var user = await FindByNameOrEmailAsync(email);
            if (user == null)
            {
                Logger.LogInformation("not found user with email " + email);
                return;
            }
            await DeleteAsync(user);
        }

        public async Task<User> GetUserByEmailAsync(string email)
        {
            return await FindByEmailAsync(email);
        }
        public async Task<User> CreateUserAsync(string email, int? tenantId, string name, string surName, string userMezonId)
        {
            var userName = email.Split('@')[0];
            var user = new User
            {
                TenantId = tenantId,
                UserName = userName.ToLower(),
                Name = name,
                UserMezonId = userMezonId,
                Surname = surName,
                EmailAddress = email.ToLower(),
                IsActive = true,
                Roles = new List<UserRole>(),
            };
            user.Password = PasswordHasher.HashPassword(user, User.CreateRandomPassword());
            user.SetNormalizedNames();
            var role = await _roleManager.GetRoleByNameAsync(StaticRoleNames.Tenants.Employee);
            if (role == null)
            {
                throw new UserFriendlyException("Not found role: " + StaticRoleNames.Tenants.Employee);
            }
            user.Roles.Add(new UserRole
            {
                TenantId = tenantId,
                RoleId = role.Id,
                UserId = user.Id
            });
            await CreateAsync(user);
            return user;
        }

        public async Task<string> GetUserMezonIdByEmail(string email)
        {
            var userMezonId = _workScope.GetAll<User>().Where(x => x.EmailAddress == email)
                .Select(x => x.UserMezonId).FirstOrDefault(); ;
            return userMezonId;
        }

        public async Task UpdateUserMezonId(string email,string userMezonId)
        {
            var user = _workScope.GetAll<User>().Where(x => x.EmailAddress == email).FirstOrDefault();

            user.UserMezonId = userMezonId;
            await UpdateAsync(user);
        }


    }
}
