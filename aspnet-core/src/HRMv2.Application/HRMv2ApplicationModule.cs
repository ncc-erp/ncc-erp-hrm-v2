using Abp.AutoMapper;
using Abp.Modules;
using Abp.Reflection.Extensions;
using HRMv2.Authorization;
using HRMv2.Configuration.Dto;

namespace HRMv2
{
    [DependsOn(
        typeof(HRMv2CoreModule),
        typeof(AbpAutoMapperModule))]
    public class HRMv2ApplicationModule : AbpModule
    {
        public override void PreInitialize()
        {
            Configuration.Authorization.Providers.Add<HRMv2AuthorizationProvider>();
            Configuration.MultiTenancy.IsEnabled = HRMv2Consts.MultiTenancyEnabled;
            Configuration.MultiTenancy.TenantIdResolveKey = "Abp-TenantId";



        }

        public override void Initialize()
        {
            var thisAssembly = typeof(HRMv2ApplicationModule).GetAssembly();

            IocManager.RegisterAssemblyByConvention(thisAssembly);


            Configuration.Modules.AbpAutoMapper().Configurators.Add(
                // Scan the assembly for classes which inherit from AutoMapper.Profile
                cfg => cfg.AddMaps(thisAssembly)
            );
        }
    }
}
