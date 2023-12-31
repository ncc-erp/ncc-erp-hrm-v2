﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Extensions;
using Abp.IdentityFramework;
using Abp.Linq.Extensions;
using Abp.MultiTenancy;
using Abp.Runtime.Security;
using HRMv2.Authorization;
using HRMv2.Authorization.Roles;
using HRMv2.Authorization.Users;
using HRMv2.Editions;
using HRMv2.Manager.Categories.Levels;
using HRMv2.Manager.Notifications.Email;
using HRMv2.MultiTenancy.Dto;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using static HRMv2.Authorization.PermissionNames;

namespace HRMv2.MultiTenancy
{
    [AbpAuthorize(PermissionNames.Admin_Tenant)]
    public class TenantAppService : AsyncCrudAppService<Tenant, TenantDto, int, PagedTenantResultRequestDto, CreateTenantDto, TenantDto>, ITenantAppService
    {
        private readonly TenantManager _tenantManager;
        private readonly EditionManager _editionManager;
        private readonly UserManager _userManager;
        private readonly RoleManager _roleManager;
        private readonly EmailManager _emailManager;
        private readonly LevelManager _levelManager;
        private readonly IAbpZeroDbMigrator _abpZeroDbMigrator;

        public TenantAppService(
            IRepository<Tenant, int> repository,
            TenantManager tenantManager,
            EditionManager editionManager,
            UserManager userManager,
            RoleManager roleManager,
            EmailManager emailManager,
            LevelManager levelManager,
            IAbpZeroDbMigrator abpZeroDbMigrator)
            : base(repository)
        {
            _tenantManager = tenantManager;
            _editionManager = editionManager;
            _userManager = userManager;
            _roleManager = roleManager;
            _abpZeroDbMigrator = abpZeroDbMigrator;
            _emailManager = emailManager;
            _levelManager = levelManager;
        }

        public override async Task<TenantDto> CreateAsync(CreateTenantDto input)
        {
            CheckCreatePermission();

            // Create tenant
            var tenant = ObjectMapper.Map<Tenant>(input);
            tenant.ConnectionString = input.ConnectionString.IsNullOrEmpty()
                ? null
                : SimpleStringCipher.Instance.Encrypt(input.ConnectionString);

            var defaultEdition = await _editionManager.FindByNameAsync(EditionManager.DefaultEditionName);
            if (defaultEdition != null)
            {
                tenant.EditionId = defaultEdition.Id;
            }

            await _tenantManager.CreateAsync(tenant);
            await CurrentUnitOfWork.SaveChangesAsync(); // To get new tenant's id.

            // Create tenant database
            _abpZeroDbMigrator.CreateOrMigrateForTenant(tenant);

            // We are working entities of new tenant, so changing tenant filter
            using (CurrentUnitOfWork.SetTenantId(tenant.Id))
            {
                // Create static roles for new tenant
                CheckErrors(await _roleManager.CreateStaticRoles(tenant.Id));

                await CurrentUnitOfWork.SaveChangesAsync(); // To get static role ids

                // Grant all permissions to admin role
                var adminRole = _roleManager.Roles.Single(r => r.Name == StaticRoleNames.Tenants.Admin);
                await _roleManager.GrantAllPermissionsAsync(adminRole);

                // Create admin user for the tenant 
                var adminUser = User.CreateTenantAdminUser(tenant.Id, GetAdminHostEmail());
                await _userManager.InitializeOptionsAsync(tenant.Id);
                CheckErrors(await _userManager.CreateAsync(adminUser, User.DefaultPassword));
                await CurrentUnitOfWork.SaveChangesAsync(); // To get admin user's id

                // Assign admin user to role!
                CheckErrors(await _userManager.AddToRoleAsync(adminUser, adminRole.Name));
                await CurrentUnitOfWork.SaveChangesAsync();

                await CreateRoleAndAddPermission(tenant.Id);
                 _emailManager.CreateDefaultMailTemplate(tenant.Id);
                 _levelManager.CreateDefaultLevel(tenant.Id);
                await CurrentUnitOfWork.SaveChangesAsync();
            }

            return MapToEntityDto(tenant);
        }

        protected override IQueryable<Tenant> CreateFilteredQuery(PagedTenantResultRequestDto input)
        {
            return Repository.GetAll()
                .WhereIf(!input.Keyword.IsNullOrWhiteSpace(), x => x.TenancyName.Contains(input.Keyword) || x.Name.Contains(input.Keyword))
                .WhereIf(input.IsActive.HasValue, x => x.IsActive == input.IsActive);
        }

        protected override void MapToEntity(TenantDto updateInput, Tenant entity)
        {
            // Manually mapped since TenantDto contains non-editable properties too.
            entity.Name = updateInput.Name;
            entity.TenancyName = updateInput.TenancyName;
            entity.IsActive = updateInput.IsActive;
        }

        public override async Task DeleteAsync(EntityDto<int> input)
        {
            CheckDeletePermission();

            var tenant = await _tenantManager.GetByIdAsync(input.Id);
            await _tenantManager.DeleteAsync(tenant);
        }

        private void CheckErrors(IdentityResult identityResult)
        {
            identityResult.CheckErrors(LocalizationManager);
        }

        private async Task CreateRoleAndAddPermission(int? tenantId)
        {
            var roleSeeds = new List<string>() { StaticRoleNames.Tenants.CEO,
                                                StaticRoleNames.Tenants.KT,
                                                StaticRoleNames.Tenants.SubKT};

            foreach (var roleSeed in roleSeeds)
            {
                var input = new Role
                {
                    TenantId = tenantId,
                    Name = roleSeed,
                    DisplayName = roleSeed,
                    IsStatic = false
                };

                var role = ObjectMapper.Map<Role>(input);
                role.SetNormalizedName();

                CheckErrors(await _roleManager.CreateAsync(role));

                var grantedPermissionsByRole = GrantPermissionRoles.PermissionRoles
                                                .Where(x => x.Key == roleSeed)
                                                .FirstOrDefault()
                                                .Value;
                if (grantedPermissionsByRole != null)
                {
                    var grantedPermissions = PermissionManager
                        .GetAllPermissions()
                        .Where(p => grantedPermissionsByRole.Contains(p.Name))
                        .ToList();
                    await _roleManager.SetGrantedPermissionsAsync(role, grantedPermissions);
                }
            }

            
        }

        private string GetAdminHostEmail()
        {
            try
            {
                var adminHostEmail = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build()
                .GetValue<string>($"DefaultAdminEmail:Tenant");
                if (string.IsNullOrEmpty(adminHostEmail))
                {
                    adminHostEmail = "tien.nguyenhuu@ncc.asia";
                }
                return adminHostEmail;
            }
            catch (Exception e)
            {
                return "tien.nguyenhuu@ncc.asia";
            }

        }
    }
}

