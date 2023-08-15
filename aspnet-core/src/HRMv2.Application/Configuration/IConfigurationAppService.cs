using System.Threading.Tasks;
using HRMv2.Configuration.Dto;

namespace HRMv2.Configuration
{
    public interface IConfigurationAppService
    {
        Task ChangeUiTheme(ChangeUiThemeInput input);
        Task<ConfigurationDto> GetAllSetting();
        Task<string> ChangeSetting(ConfigurationDto input);
    }
}
