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
        public KomuSettingDto KomuService { get; set; }
    }

    public class KomuSettingDto
    {
        public string BaseAddress { get; set; }
        public string SecurityCode { get; set; }
        public string ChannelIdDevMode { get; set; }
        public string EnableNoticeKomu { get; set; }
    }

    public class SettingDto
    {
        public string BaseAddress { get; set; }
        public string SecurityCode { get; set; }
    }
}
