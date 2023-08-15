using Abp.Configuration;
using Abp.Dependency;
using Abp.Domain.Uow;
using Abp.Threading.BackgroundWorkers;
using Abp.Threading.Timers;
using HRMv2.Configuration;
using HRMv2.Configuration.Dto;
using HRMv2.Manager.Employees;
using Microsoft.Extensions.Logging;
using NccCore.Uitls;

namespace HRMv2.Web.Host.Startup.BackgroundWorker
{
    public class UpdateEmployeeInfoToOtherToolBackgroundWorker : PeriodicBackgroundWorkerBase, ISingletonDependency
    {
        private readonly ILogger<UpdateEmployeeInfoToOtherToolBackgroundWorker> _log;
        private EmployeeManager _employeeManager;
        private ISettingManager _settingManager;

        public UpdateEmployeeInfoToOtherToolBackgroundWorker(
             AbpTimer timer,
             ILogger<UpdateEmployeeInfoToOtherToolBackgroundWorker> log,
             EmployeeManager employeeManager,
             ISettingManager settingManager

        ) : base(timer)
        {
            _log = log;
            _employeeManager = employeeManager;
            _settingManager = settingManager;
            Timer.Period = 60 * 60 * 1000;

        }

        [UnitOfWork]
        protected override void DoWork()
        {
            _log.LogCritical("Start Sync Data Background Service");
            SyncData();
        }

        private async void SyncData()
        {
            var strEnable = _settingManager.GetSettingValueForApplication(AppSettingNames.EnableWorkerAutoUpdateAllEmployeeInfoToOtherToolSetting);
            _log.LogInformation("strEnable = " + strEnable);

            bool enable;

            bool.TryParse(strEnable, out enable);

            if (!enable)
            {
                _log.LogInformation("enable = false => return");
                return;
            }

            int hourToRun;
            if (!int.TryParse(_settingManager.GetSettingValueForApplication(AppSettingNames.AutoUpdateEmployeeInfoToOtherToolAtHour), out hourToRun))
            {
                _log.LogInformation("Synchronization time is not configured; will default run at 23h");
                hourToRun = 23;
            }

            var now = DateTimeUtils.GetNow();
            _log.LogInformation("SyncData() started, synchronization time = " + hourToRun);
            if (now.Hour == hourToRun)
            {
                _employeeManager.UpdateAllWorkingEmployeeInfoToOtherTools();
            }
        }
    }
}