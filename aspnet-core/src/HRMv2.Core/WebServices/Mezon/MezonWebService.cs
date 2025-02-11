using Abp.Dependency;
using Abp.Runtime.Session;
using HRMv2.Manager.Notifications.NotifyToChannel.Dto;
using HRMv2.Manager.Notifications.SendMezonDM.Dto;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace HRMv2.WebServices.Mezon
{
    public class MezonWebService : BaseWebService
    {
        private const string serviceName = "MezonService";
        private readonly string _isNotifyToMezon;

        public MezonWebService(HttpClient httpClient, IConfiguration configuration, IAbpSession abpSession, IIocResolver iocResovler) : base(httpClient, abpSession, iocResovler)
        {
            _isNotifyToMezon = configuration.GetValue<string>($"{serviceName}:EnableKomuNotification", "true");
        }

        public void NotifyToChannel(MezonMessage mezonMessage, string mezonUrl)
        {
            if (_isNotifyToMezon != "true")
            {
                Logger.LogInformation("_isNotifyToMezon=" + _isNotifyToMezon + " => stop");
                return;
            }

            if (string.IsNullOrEmpty(mezonUrl))
            {
                Logger.LogError("mezonUrl null or empty");
                return;
            }
            Post(mezonUrl, new { type = "hook", message = mezonMessage });
        }

        public void SendDirectMessageToUser(InputMezonDM input, string mezonUrl, string userName)
        {
            if (_isNotifyToMezon != "true")
            {
                Logger.LogInformation("_isNotifyToMezon=" + _isNotifyToMezon + " => stop");
                return;
            }
            if (string.IsNullOrEmpty(mezonUrl))
            {
                Logger.LogError("channelUrlToSend null or empty");
                return;
            }
            var url = $"{mezonUrl}/{userName}";
            Post(url, input);
           
        }

    }
}
