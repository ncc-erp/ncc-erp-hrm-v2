using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMv2.Configuration.Dto
{
    public class ConfigurationDto
    {
        public string EmailHR { get; set; }
        public string GoogleClientId { get; set; }
        public string SecurityCode { get; set; }
        public string EnableNormalLogin { get; set; }
        public SettingDto FinfastService { get; set; }
        public SettingDto ProjectService { get; set; }
        public SettingDto TimesheetService { get; set; }
        public SettingDto IMSService { get; set; }
        public SettingDto TalentService { get; set; }
        public SettingDto VoucherService { get; set; }
        public KomuSettingDto KomuService { get; set; }
        public SettingDto HRMService { get; set; }
        public MezonSettingDto MezonService { get; set; }
        public Oauth2Mezon Oauth2Mezon { get; set; }
        public BotHRMSetting BotHRM { get; set; }
    }

    public class BotHRMSetting
    {
        public string NameBot { get; set; }
        public string ApplicationId { get; set; }
        public string ApplicationToken { get; set; }
        public string UrlSentToken { get; set; }
        public string UrlAuthenticate { get; set; }
    }
    public class Oauth2Mezon
    {
        public string Client_Id { set; get; }
        public string Client_Secret { set; get; }
        public string Grant_Type { get; set; }
        public string Redirect_URI { get; set; }
        public string Url_Oauth2Mezon { get; set; }
        public string Url_UserInfo { get; set; }
    }
    public class KomuSettingDto
    {
        public string BaseAddress { get; set; }
        public string SecurityCode { get; set; }
        public string ChannelIdDevMode { get; set; }
        public string EnableNoticeKomu { get; set; }
    }

    public class MezonSettingDto
    {
        public string DevModeUrl { get; set; }
        public string EnableMezonNotification { get; set; }
    }

    public class SettingDto
    {
        public string BaseAddress { get; set; }
        public string SecurityCode { get; set; }
    }
}
