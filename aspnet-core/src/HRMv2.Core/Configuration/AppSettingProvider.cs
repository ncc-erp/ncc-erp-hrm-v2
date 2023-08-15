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
                new SettingDefinition(AppSettingNames.UiTheme, "", scopes: SettingScopes.Application | SettingScopes.Tenant | SettingScopes.User, clientVisibilityProvider: new VisibleSettingClientVisibilityProvider()),
                new SettingDefinition(AppSettingNames.EmailHR, "", scopes:SettingScopes.Application| SettingScopes.Tenant),
                new SettingDefinition(AppSettingNames.GoogleClientId,"",scopes:SettingScopes.Application| SettingScopes.Tenant),
                new SettingDefinition(AppSettingNames.SecretRegisterCode,"",scopes:SettingScopes.Application| SettingScopes.Tenant),
                new SettingDefinition(AppSettingNames.SecurityCode, "", scopes:SettingScopes.Application| SettingScopes.Tenant),
                new SettingDefinition(AppSettingNames.AutoCreateUserToTimesheet, "", scopes:SettingScopes.Application| SettingScopes.Tenant),
                new SettingDefinition(AppSettingNames.AutoCreateUserToIMS, "", scopes:SettingScopes.Application| SettingScopes.Tenant),
                new SettingDefinition(AppSettingNames.AutoCreateUserToProject, "", scopes:SettingScopes.Application| SettingScopes.Tenant),
                new SettingDefinition(AppSettingNames.AutoCreateUserToTalent, "", scopes:SettingScopes.Application| SettingScopes.Tenant),
                new SettingDefinition(AppSettingNames.FinaceUri,"",scopes:SettingScopes.Application|SettingScopes.Tenant),
                new SettingDefinition(AppSettingNames.FinanceSecretKey,"",scopes:SettingScopes.Application|SettingScopes.Tenant),
                new SettingDefinition(AppSettingNames.IMSUri,"",scopes:SettingScopes.Application|SettingScopes.Tenant),
                new SettingDefinition(AppSettingNames.IMSSecretKey,"",scopes:SettingScopes.Application|SettingScopes.Tenant),
                new SettingDefinition(AppSettingNames.ProjectUri,"",scopes:SettingScopes.Application|SettingScopes.Tenant),
                new SettingDefinition(AppSettingNames.ProjectSecretKey,"",scopes:SettingScopes.Application|SettingScopes.Tenant),
                new SettingDefinition(AppSettingNames.TimesheetUri,"",scopes:SettingScopes.Application|SettingScopes.Tenant),
                new SettingDefinition(AppSettingNames.TimeSheetSecretKey,"",scopes:SettingScopes.Application|SettingScopes.Tenant),
                new SettingDefinition(AppSettingNames.TalentUri,"",scopes:SettingScopes.Application|SettingScopes.Tenant),
                new SettingDefinition(AppSettingNames.TalentSecretKey,"",scopes:SettingScopes.Application|SettingScopes.Tenant),
                new SettingDefinition(AppSettingNames.EnableNormalLogin,"",scopes:SettingScopes.Application|SettingScopes.Tenant),
                new SettingDefinition(AppSettingNames.KomuITChannelId,"",scopes:SettingScopes.Application|SettingScopes.Tenant),
                new SettingDefinition(AppSettingNames.PayrollChannelId,"",scopes:SettingScopes.Application|SettingScopes.Tenant),
                new SettingDefinition(AppSettingNames.EnableWorkerAutoUpdateAllEmployeeInfoToOtherToolSetting,"",scopes:SettingScopes.Application|SettingScopes.Tenant),
                new SettingDefinition(AppSettingNames.AutoUpdateEmployeeInfoToOtherToolAtHour,"",scopes:SettingScopes.Application|SettingScopes.Tenant),
            };
        }
    }
}