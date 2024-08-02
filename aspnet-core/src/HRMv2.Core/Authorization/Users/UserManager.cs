﻿using System;
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
        public async Task DeactiveUser(long userId)
        {
            await UpdateUserActive(userId, false);
        }

        public async Task UpdateUserActive(long userId, bool isActive)
        {
            var user = await GetUserByIdAsync(userId);
            user.IsActive = isActive;
            await UpdateAsync(user);
        }
        public async Task<User> CreateUserForEmployee(CreateUpdateEmployeeDto input, string name, string surName)
        {
            var email = input.Email;
            var userName = email.Split('@')[0];

            var userCreateDto = new User
            {
                UserName = userName,
                Name = name,
                Surname = surName,
                EmailAddress = email,
                IsActive = true,
                Roles = new List<UserRole>(),
                Password = "",
            };
            userCreateDto.SetNormalizedNames();
            var role = await _roleManager.GetRoleByNameAsync(StaticRoleNames.Tenants.Employee);
            if (role == null)
            {
                throw new UserFriendlyException("Role not found");
            }

            userCreateDto.Roles.Add(new UserRole
            {
                TenantId = null,
                RoleId = role.Id,
                UserId = userCreateDto.Id
            });
            await CreateAsync(userCreateDto);
            return userCreateDto;
        }
        public async Task DeleteAsync(long input)
        {
            var user = await GetUserByIdAsync(input);
            await DeleteAsync(user);
        }
        public async Task<User> GetUserByEmail(string email)
        {
            var user = FindByNameOrEmailAsync(email);
                
            return await user;
        }
    }
}
