using Abp.Localization;
using Abp.Modules;
using Abp.Reflection.Extensions;
using Abp.Runtime.Security;
using Abp.Timing;
using Abp.Zero;
using Abp.Zero.Configuration;
using HRMv2.Authorization.Roles;
using HRMv2.Authorization.Users;
using HRMv2.Configuration;
using HRMv2.Localization;
using HRMv2.MultiTenancy;
using HRMv2.NccCore;
using HRMv2.Timing;

namespace HRMv2
{
    [DependsOn(typeof(AbpZeroCoreModule))]
    public class HRMv2CoreModule : AbpModule
    {
        public override void PreInitialize()
        {
            Configuration.Auditing.IsEnabledForAnonymousUsers = true;

            // Declare entity types
            Configuration.Modules.Zero().EntityTypes.Tenant = typeof(Tenant);
            Configuration.Modules.Zero().EntityTypes.Role = typeof(Role);
            Configuration.Modules.Zero().EntityTypes.User = typeof(User);

            HRMv2LocalizationConfigurer.Configure(Configuration.Localization);

            // Enable this line to create a multi-tenant application.
            Configuration.MultiTenancy.IsEnabled = HRMv2Consts.MultiTenancyEnabled;

            // Configure roles
            AppRoleConfig.Configure(Configuration.Modules.Zero().RoleManagement);

            Configuration.Settings.Providers.Add<AppSettingProvider>();
            
            Configuration.Localization.Languages.Add(new LanguageInfo("fa", "فارسی", "famfamfam-flags ir"));
            
            Configuration.Settings.SettingEncryptionConfiguration.DefaultPassPhrase = HRMv2Consts.DefaultPassPhrase;
            SimpleStringCipher.DefaultPassPhrase = HRMv2Consts.DefaultPassPhrase;
            Configuration.BackgroundJobs.IsJobExecutionEnabled = HRMv2Consts.EnableBackgroundJobExecution;
        }

        public override void Initialize()
        {
            IocManager.Register<IWorkScope, WorkScope>(Abp.Dependency.DependencyLifeStyle.Transient);
            IocManager.RegisterAssemblyByConvention(typeof(HRMv2CoreModule).GetAssembly());
        }

        public override void PostInitialize()
        {
            IocManager.Resolve<AppTimes>().StartupTime = Clock.Now;
        }
    }
}
