using System.Threading.Tasks;
using Abp.Authorization;
using Abp.Net.Mail;
using Abp.Runtime.Session;
using AutoMapper.Configuration;
using HRMv2.Authorization;
using HRMv2.Configuration.Dto;
using HRMv2.WebServices;
using HRMv2.WebServices.Dto;
using HRMv2.WebServices.Finfast;
using HRMv2.WebServices.IMS;
using HRMv2.WebServices.Project;
using HRMv2.WebServices.Talent;
using HRMv2.WebServices.Timesheet;
using HRMv2.WebServices.Timesheet.Dto;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace HRMv2.Configuration
{
    [AbpAuthorize]
    public class ConfigurationAppService : HRMv2AppServiceBase, IConfigurationAppService
    {
        private readonly Microsoft.Extensions.Configuration.IConfiguration _appConfiguration;
        private readonly TimesheetWebService _timesheetWebService;
        private readonly TalentWebService _talentWebService;
        private readonly ProjectService _projectService;
        private readonly IMSWebService _imsWebService;
        private readonly FinfastWebService _finfastWebService;
        public ConfigurationAppService(Microsoft.Extensions.Configuration.IConfiguration appConfiguration,
            TimesheetWebService timesheetWebService,
            TalentWebService talentWebService,
            ProjectService projectService,
            IMSWebService imsWebService,
            FinfastWebService finfastWebService)
        {
            _appConfiguration = appConfiguration;
            _timesheetWebService = timesheetWebService;
            _talentWebService = talentWebService;
            _projectService = projectService;
            _imsWebService = imsWebService;
            _finfastWebService = finfastWebService;
            

        }
        public async Task ChangeUiTheme(ChangeUiThemeInput input)
        {
            await SettingManager.ChangeSettingForUserAsync(AbpSession.ToUserIdentifier(), AppSettingNames.UiTheme, input.Theme);
        }

        [AbpAuthorize(PermissionNames.Admin_Configuration_WokerAutoUpdateAllEmployeeInfo_View)]
        public async Task<WorkerAutoUpdateAllEmployeeInfoToOtherToolSettingDto> GetAutoUpdateAllEmployeeInfoToOther()
        {
            return new WorkerAutoUpdateAllEmployeeInfoToOtherToolSettingDto
            {
                RunAtHour = await SettingManager.GetSettingValueForApplicationAsync(AppSettingNames.AutoUpdateEmployeeInfoToOtherToolAtHour),
                EnableWorkerAutoUpdateAllEmployeeInfoToOtherToolSetting = bool.Parse(await SettingManager.GetSettingValueForApplicationAsync(AppSettingNames.EnableWorkerAutoUpdateAllEmployeeInfoToOtherToolSetting)),
            };
        }
       
        [AbpAllowAnonymous]
        [AbpAuthorize(PermissionNames.Admin_Configuration_LoginSetting_View)]
        public async Task<LoginSettingDto> GetLoginSetting()
        {
            return new LoginSettingDto
            {
                GoogleClientId = await SettingManager.GetSettingValueForApplicationAsync(AppSettingNames.GoogleClientId),
                EnableNormalLogin = bool.Parse(await SettingManager.GetSettingValueForApplicationAsync(AppSettingNames.EnableNormalLogin)),
            };
        }
        [AbpAuthorize(PermissionNames.Admin_Configuration_View)]
        public async Task<ConfigurationDto> GetAllSetting()
        {
            return new ConfigurationDto
            {
                FinfastService = new SettingDto
                {
                    BaseAddress = _appConfiguration.GetValue<string>("FinfastService:BaseAddress"),
                    SecurityCode = _appConfiguration.GetValue<string>("FinfastService:SecurityCode")

                },
                ProjectService = new SettingDto
                {
                    BaseAddress = _appConfiguration.GetValue<string>("ProjectService:BaseAddress"),
                    SecurityCode = _appConfiguration.GetValue<string>("ProjectService:SecurityCode")
                },
                TimesheetService = new SettingDto
                {
                    BaseAddress = _appConfiguration.GetValue<string>("TimesheetService:BaseAddress"),
                    SecurityCode = _appConfiguration.GetValue<string>("TimesheetService:SecurityCode")
                },
                IMSService = new SettingDto
                {
                    BaseAddress = _appConfiguration.GetValue<string>("IMSService:BaseAddress"),
                    SecurityCode = _appConfiguration.GetValue<string>("IMSService:SecurityCode")
                },
                TalentService = new SettingDto
                {
                    BaseAddress = _appConfiguration.GetValue<string>("TalentService:BaseAddress"),
                    SecurityCode = _appConfiguration.GetValue<string>("TalentService:SecurityCode")
                },
                KomuService = new KomuSettingDto
                {
                    BaseAddress = _appConfiguration.GetValue<string>("KomuService:BaseAddress"),
                    SecurityCode = _appConfiguration.GetValue<string>("KomuService:SecurityCode"),
                    EnableNoticeKomu = _appConfiguration.GetValue<string>("KomuService:EnableKomuNotification"),
                    ChannelIdDevMode = _appConfiguration.GetValue<string>("KomuService:ChannelIdDevMode"),
                },

            };
        }

        [HttpGet]
        public async Task<DiscordChannelDto> GetDiscordChannels()
        {
            return new DiscordChannelDto
            {
                ITChannel = await SettingManager.GetSettingValueForApplicationAsync(AppSettingNames.KomuITChannelId),
                PayrollChannelId = await SettingManager.GetSettingValueForApplicationAsync(AppSettingNames.PayrollChannelId),
            };
        }

        [HttpPost]
        public async Task<DiscordChannelDto> SetDiscordChannels(DiscordChannelDto input)
        {
            await SettingManager.ChangeSettingForApplicationAsync(AppSettingNames.KomuITChannelId, input.ITChannel);
            await SettingManager.ChangeSettingForApplicationAsync(AppSettingNames.PayrollChannelId, input.PayrollChannelId);

            return new DiscordChannelDto
            {
                ITChannel = await SettingManager.GetSettingValueForApplicationAsync(AppSettingNames.KomuITChannelId),
                PayrollChannelId = await SettingManager.GetSettingValueForApplicationAsync(AppSettingNames.PayrollChannelId),
            };
        }

        [AbpAuthorize(PermissionNames.Admin_Configuration_LoginSetting_Edit)]
        public async Task<LoginSettingDto> ChangeLoginSetting(LoginSettingDto input)
        {
            await SettingManager.ChangeSettingForApplicationAsync(AppSettingNames.GoogleClientId, input.GoogleClientId);
            await SettingManager.ChangeSettingForApplicationAsync(AppSettingNames.EnableNormalLogin, input.EnableNormalLogin.ToString());
            return input;


        }
        [AbpAuthorize(PermissionNames.Admin_Configuration_WokerAutoUpdateAllEmployeeInfo_Edit)]
        public async Task<WorkerAutoUpdateAllEmployeeInfoToOtherToolSettingDto> ChangeWorkerAutoUpdateAllEmployeeInfoToOther(WorkerAutoUpdateAllEmployeeInfoToOtherToolSettingDto input)
        {
            await SettingManager.ChangeSettingForApplicationAsync(AppSettingNames.EnableWorkerAutoUpdateAllEmployeeInfoToOtherToolSetting , input.EnableWorkerAutoUpdateAllEmployeeInfoToOtherToolSetting.ToString());
            await SettingManager.ChangeSettingForApplicationAsync(AppSettingNames.AutoUpdateEmployeeInfoToOtherToolAtHour, input.RunAtHour);
            return input;


        }
        Task<string> IConfigurationAppService.ChangeSetting(ConfigurationDto input)
        {
            throw new System.NotImplementedException();
        }

        [HttpGet]
        public async Task<EmailConfigDto> GetEmailSetting()
        {
            return new EmailConfigDto
            {
                DefaultAddress = await SettingManager.GetSettingValueForApplicationAsync(EmailSettingNames.DefaultFromAddress),
                DisplayName = await SettingManager.GetSettingValueForApplicationAsync(EmailSettingNames.DefaultFromDisplayName),
                Host = await SettingManager.GetSettingValueForApplicationAsync(EmailSettingNames.Smtp.Host),
                Port = await SettingManager.GetSettingValueForApplicationAsync(EmailSettingNames.Smtp.Port),
                UserName = await SettingManager.GetSettingValueForApplicationAsync(EmailSettingNames.Smtp.UserName),
                Password = await SettingManager.GetSettingValueForApplicationAsync(EmailSettingNames.Smtp.Password),
                EnableSsl = await SettingManager.GetSettingValueForApplicationAsync(EmailSettingNames.Smtp.EnableSsl),
                UseDefaultCredentials = await SettingManager.GetSettingValueForApplicationAsync(EmailSettingNames.Smtp.UseDefaultCredentials)
            };
        }

        [HttpPost]
        public async Task SetEmailSetting(EmailConfigDto input)
        {
            await SettingManager.ChangeSettingForApplicationAsync(EmailSettingNames.DefaultFromAddress, input.DefaultAddress);
            await SettingManager.ChangeSettingForApplicationAsync(EmailSettingNames.DefaultFromDisplayName, input.DisplayName);
            await SettingManager.ChangeSettingForApplicationAsync(EmailSettingNames.Smtp.Host, input.Host);
            await SettingManager.ChangeSettingForApplicationAsync(EmailSettingNames.Smtp.Port, input.Port);
            await SettingManager.ChangeSettingForApplicationAsync(EmailSettingNames.Smtp.UserName, input.UserName);
            await SettingManager.ChangeSettingForApplicationAsync(EmailSettingNames.Smtp.Password, input.Password);
            await SettingManager.ChangeSettingForApplicationAsync(EmailSettingNames.Smtp.EnableSsl, input.EnableSsl);
            await SettingManager.ChangeSettingForApplicationAsync(EmailSettingNames.Smtp.UseDefaultCredentials, input.UseDefaultCredentials);
        }

       [HttpGet]
       public GetResultConnectDto CheckConnectToTimesheet()
       {
          return _timesheetWebService.CheckConnectToTimesheet();
       }

       [HttpGet]
       public GetResultConnectDto CheckConnectToTalent()
       {
           return _talentWebService.CheckConnectToTalent();
       }

       [HttpGet]
       public GetResultConnectDto CheckConnectToFinfast()
       {
           return _finfastWebService.CheckConnectToFinfast();
       }

       [HttpGet]
       public GetResultConnectDto CheckConnectToProject()
       {
           return _projectService.CheckConnectToProject();
       }

       [HttpGet]
       public GetResultConnectDto CheckConnectToIMS()
       {
           return _imsWebService.CheckConnectToIMS();
       }
    }
}
