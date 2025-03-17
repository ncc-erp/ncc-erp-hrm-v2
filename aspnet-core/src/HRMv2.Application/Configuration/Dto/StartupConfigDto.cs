using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMv2.Configuration.Dto
{
    public class LoginSettingDto
    {
        public string GoogleClientId { get; set; }
        public bool EnableNormalLogin { get; set; }
        public string MezonClientId {  get; set; }
        public bool EnableLoginMezon {  get; set; }
        public bool EnableLoginGoogle {  get; set; }
    }
   
    public class ConfigLoginSetting
    {
        public bool EnableNormalLogin { get; set; }
        public bool EnableLoginGoogle { get; set; }
        public bool EnableLoginMezon { get; set; }
    }
}
