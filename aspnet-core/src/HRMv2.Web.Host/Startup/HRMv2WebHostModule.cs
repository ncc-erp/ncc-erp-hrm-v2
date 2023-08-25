using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Abp.Modules;
using Abp.Reflection.Extensions;
using HRMv2.Configuration;
using Abp.Threading.BackgroundWorkers;
using HRMv2.Constants;
using HRMv2.Web.Host.Startup.BackgroundWorker;
namespace HRMv2.Web.Host.Startup
{
    [DependsOn(
       typeof(HRMv2WebCoreModule))]
    public class HRMv2WebHostModule: AbpModule
    {
        private readonly IWebHostEnvironment _env;
        private readonly IConfigurationRoot _appConfiguration;

        public HRMv2WebHostModule(IWebHostEnvironment env)
        {
            _env = env;
            _appConfiguration = env.GetAppConfiguration();  
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(HRMv2WebHostModule).GetAssembly());
        }
        public override void PostInitialize()
        {
            base.PostInitialize();
            if (AppConsts.EnableSyncDataService)
            { 
                var workManager = IocManager.Resolve<IBackgroundWorkerManager>();
                workManager.Add(IocManager.Resolve<UpdateEmployeeInfoToOtherToolBackgroundWorker>());
            }           
        }
    }
}
