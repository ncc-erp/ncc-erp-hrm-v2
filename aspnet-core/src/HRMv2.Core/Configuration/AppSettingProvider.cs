using System.Collections.Generic;
using Abp.Configuration;

namespace HRMv2.Configuration
{
    public class AppSettingProvider : SettingProvider
    {
        public override IEnumerable<SettingDefinition> GetSettingDefinitions(SettingDefinitionProviderContext context)
        {
            return new[]
            {
                new SettingDefinition(AppSettingNames.UiTheme, "red", scopes: SettingScopes.Application | SettingScopes.Tenant | SettingScopes.User, clientVisibilityProvider: new VisibleSettingClientVisibilityProvider()),
                new SettingDefinition(AppSettingNames.EmailHR, "EmailHR", scopes:SettingScopes.Application| SettingScopes.Tenant),
                new SettingDefinition(AppSettingNames.GoogleClientId,"GoogleClientId",scopes:SettingScopes.Application| SettingScopes.Tenant),
                new SettingDefinition(AppSettingNames.SecretRegisterCode,"SecretRegisterCode",scopes:SettingScopes.Application| SettingScopes.Tenant),
                new SettingDefinition(AppSettingNames.SecurityCode, "SecurityCode", scopes:SettingScopes.Application| SettingScopes.Tenant),
                new SettingDefinition(AppSettingNames.AutoCreateUserToTimesheet, "True", scopes:SettingScopes.Application| SettingScopes.Tenant),
                new SettingDefinition(AppSettingNames.AutoCreateUserToIMS, "True", scopes:SettingScopes.Application| SettingScopes.Tenant),
                new SettingDefinition(AppSettingNames.AutoCreateUserToProject, "True", scopes:SettingScopes.Application| SettingScopes.Tenant),
                new SettingDefinition(AppSettingNames.AutoCreateUserToTalent, "True", scopes:SettingScopes.Application| SettingScopes.Tenant),
                new SettingDefinition(AppSettingNames.FinaceUri,"FinaceUri",scopes:SettingScopes.Application|SettingScopes.Tenant),
                new SettingDefinition(AppSettingNames.FinanceSecretKey,"FinanceSecretKey",scopes:SettingScopes.Application|SettingScopes.Tenant),
                new SettingDefinition(AppSettingNames.IMSUri,"IMSUri",scopes:SettingScopes.Application|SettingScopes.Tenant),
                new SettingDefinition(AppSettingNames.IMSSecretKey,"IMSSecretKey",scopes:SettingScopes.Application|SettingScopes.Tenant),
                new SettingDefinition(AppSettingNames.ProjectUri,"ProjectUri",scopes:SettingScopes.Application|SettingScopes.Tenant),
                new SettingDefinition(AppSettingNames.ProjectSecretKey,"ProjectSecretKey",scopes:SettingScopes.Application|SettingScopes.Tenant),
                new SettingDefinition(AppSettingNames.TimesheetUri,"TimesheetUri",scopes:SettingScopes.Application|SettingScopes.Tenant),
                new SettingDefinition(AppSettingNames.TimeSheetSecretKey,"TimeSheetSecretKey",scopes:SettingScopes.Application|SettingScopes.Tenant),
                new SettingDefinition(AppSettingNames.TalentUri,"TalentUri",scopes:SettingScopes.Application|SettingScopes.Tenant),
                new SettingDefinition(AppSettingNames.TalentSecretKey,"TalentSecretKey",scopes:SettingScopes.Application|SettingScopes.Tenant),
                new SettingDefinition(AppSettingNames.EnableNormalLogin,"True",scopes:SettingScopes.Application|SettingScopes.Tenant),
                new SettingDefinition(AppSettingNames.KomuITChannelId,"KomuITChannelId",scopes:SettingScopes.Application|SettingScopes.Tenant),
                new SettingDefinition(AppSettingNames.PayrollChannelId,"PayrollChannelId",scopes:SettingScopes.Application|SettingScopes.Tenant),
                new SettingDefinition(AppSettingNames.EnableWorkerAutoUpdateAllEmployeeInfoToOtherToolSetting,"True",scopes:SettingScopes.Application|SettingScopes.Tenant),
                new SettingDefinition(AppSettingNames.AutoUpdateEmployeeInfoToOtherToolAtHour,"23",scopes:SettingScopes.Application|SettingScopes.Tenant),
            };
        }
    }
}