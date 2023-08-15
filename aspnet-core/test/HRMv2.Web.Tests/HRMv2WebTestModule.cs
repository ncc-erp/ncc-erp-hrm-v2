using Abp.AspNetCore;
using Abp.AspNetCore.TestBase;
using Abp.Modules;
using Abp.Reflection.Extensions;
using HRMv2.EntityFrameworkCore;
using HRMv2.Web.Startup;
using Microsoft.AspNetCore.Mvc.ApplicationParts;

namespace HRMv2.Web.Tests
{
    [DependsOn(
        typeof(HRMv2WebMvcModule),
        typeof(AbpAspNetCoreTestBaseModule)
    )]
    public class HRMv2WebTestModule : AbpModule
    {
        public HRMv2WebTestModule(HRMv2EntityFrameworkModule abpProjectNameEntityFrameworkModule)
        {
            abpProjectNameEntityFrameworkModule.SkipDbContextRegistration = true;
        } 
        
        public override void PreInitialize()
        {
            Configuration.UnitOfWork.IsTransactional = false; //EF Core InMemory DB does not support transactions.
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(HRMv2WebTestModule).GetAssembly());
        }
        
        public override void PostInitialize()
        {
            IocManager.Resolve<ApplicationPartManager>()
                .AddApplicationPartsIfNotAddedBefore(typeof(HRMv2WebMvcModule).Assembly);
        }
    }
}