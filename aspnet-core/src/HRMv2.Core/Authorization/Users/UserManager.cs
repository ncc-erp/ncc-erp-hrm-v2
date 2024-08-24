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
using Abp.Authorization.Roles;
using System.Threading.Tasks;
using Abp.UI;
using HRMv2.Manager.Employees.Dto;
using HRMv2.Authorization.Users.Dto;

namespace HRMv2.Authorization.Users
{
    public class UserManager : AbpUserManager<Role, User>
    {
        private readonly RoleManager _roleManager;
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
        }
        /* public async Task DeactiveUser(long userId)
         {
             await UpdateUserActive(userId, false);
         }*/

        public async Task UpdateUserActive(string email, bool isActive)
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
        
        public async Task<GetUserName> GetUserByEmail(string email)
        {
            var userName = await FindByNameOrEmailAsync(email);
            if(userName == null)
            {
                return null;
            }

            return new GetUserName
            {
                UserName = userName.UserName,
            };
        }

        public async Task<User> CreateUserAsync(string email, int? tenantId, string name, string surName)
        {
            var userName = email.Split('@')[0];
            var user = new User
            {
                TenantId = tenantId,
                UserName = userName.ToLower(),
                Name = name,
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
    }
}
