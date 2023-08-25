using System;
using System.Transactions;
using Microsoft.EntityFrameworkCore;
using Abp.Dependency;
using Abp.Domain.Uow;
using Abp.EntityFrameworkCore.Uow;
using Abp.MultiTenancy;
using HRMv2.EntityFrameworkCore.Seed.Host;
using HRMv2.EntityFrameworkCore.Seed.Tenants;
using Abp;
using Abp.Runtime.Session;
using Abp.Application.Services;
using System.Linq;

namespace HRMv2.EntityFrameworkCore.Seed
{
    public static class SeedHelper
    {
        public static void SeedHostDb(IIocResolver iocResolver)
        {
            WithDbContext<HRMv2DbContext>(iocResolver, SeedHostDb);
        }

        public static void SeedHostDb(HRMv2DbContext context)
        {
            context.SuppressAutoSetTenantId = true;

            // Host seed
            new InitialHostDbBuilder(context).Create();

            // Default tenant seed (in host database).
            
            new DefaultTenantBuilder(context).Create();
            var listTenantIds = context.Tenants.IgnoreQueryFilters()
                .Where(t=> t.IsActive && !t.IsDeleted)
                .Select(t=> t.Id)
                .ToList();
            foreach(var id in listTenantIds)
            {
                new TenantRoleAndUserBuilder(context, id).Create();
                new TenantLevelBuilder(context, id).Create();
                new TenantEmailTemplateBuilder(context, id).Create();
            }
        }

        private static void WithDbContext<TDbContext>(IIocResolver iocResolver, Action<TDbContext> contextAction)
            where TDbContext : DbContext
        {
            using (var uowManager = iocResolver.ResolveAsDisposable<IUnitOfWorkManager>())
            {
                using (var uow = uowManager.Object.Begin(TransactionScopeOption.Suppress))
                {
                    var context = uowManager.Object.Current.GetDbContext<TDbContext>(MultiTenancySides.Host);

                    contextAction(context);

                    uow.Complete();
                }
            }
        }
    }
}
