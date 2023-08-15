using Abp.Dependency;
using Abp.Domain.Uow;
using Abp.Modules;
using Abp.TestBase;
using Castle.MicroKernel.Registration;
using HRMv2.Authorization;
using HRMv2.EntityFrameworkCore;
using HRMv2.EntityFrameworkCore.Seed.Host;
using HRMv2.EntityFrameworkCore.Seed.Tenants;
using HRMv2.Tests.Seeders;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace HRMv2.Tests
{
    public abstract class TestBase<TStartupModule> : AbpIntegratedTestBase<TStartupModule>
        where TStartupModule : AbpModule
    {
        protected TestBase()
        {
            //SeedUserData();
            SeedData();
            LoginAsHostAdmin();
        }

        public void UsingDbContext(Action<HRMv2DbContext> action)
        {
            using (var context = Resolve<HRMv2DbContext>())
            {
                action(context);
                context.SaveChanges();
            }
        }

        protected virtual async Task<TResult> WithUnitOfWorkAsync<TResult>(Func<Task<TResult>> func)
        {
            using (var scope = Resolve<IServiceProvider>().CreateScope())
            {
                var uowManager = scope.ServiceProvider.GetRequiredService<IUnitOfWorkManager>();
                using (var uow = uowManager.Begin())
                {
                    var result = await func();
                    await uow.CompleteAsync();
                    return result;
                }
            }
        }

        protected virtual async Task WithUnitOfWorkAsync(Func<Task> func)
        {
            using (var scope = Resolve<IServiceProvider>().CreateScope())
            {
                var uowManager = scope.ServiceProvider.GetRequiredService<IUnitOfWorkManager>();
                using (var uow = uowManager.Begin())
                {
                    await func();
                    await uow.CompleteAsync();
                }
            }
        }

        private void SeedData()
        {
            UsingDbContext((context) =>
            {
                new DataSeederConsumer().Seed(context);
            });
        }

        private void SeedUserData()
        {
            UsingDbContext((context) =>
            {
                new InitialHostDbBuilder(context).Create();
                new DefaultTenantBuilder(context).Create();
            });
        }

        private void LoginAsHostAdmin()
        {
            var logInManager = Resolve<LogInManager>();
            var loginResult = logInManager.LoginAsync("admin", "123qwe").Result;
            AbpSession.UserId = loginResult.User?.Id;
            AbpSession.TenantId = loginResult.User?.TenantId;
        }
    }
}
