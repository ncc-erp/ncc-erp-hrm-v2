using Abp.Dependency;
using Abp.Runtime.Session;
using HRMv2.Manager.Notifications.SendMezonDM.Dto;
using Microsoft.Extensions.Configuration;
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

        public void NotifyToChannel(string mezonMessage, string mezonUrl)
        {
            if (_isNotifyToMezon != "true")
            {
                Logger.Info("_isNotifyToMezon=" + _isNotifyToMezon + " => stop");
                return;
            }

            if (string.IsNullOrEmpty(mezonUrl))
            {
                Logger.Error("mezonUrl null or empty");
                return;
            }
            Post(mezonUrl, new { type = "HRM", message = new { username = "HRM", t = mezonMessage } });
        }

        public void SendDirectMessageToUser(InputMezonDM input, string mezonUrl, string userName)
        {
            if (_isNotifyToMezon != "true")
            {
                Logger.Info("_isNotifyToMezon=" + _isNotifyToMezon + " => stop");
                return;
            }
            if (string.IsNullOrEmpty(mezonUrl))
            {
                Logger.Error("channelUrlToSend null or empty");
                return;
            }
            var url = $"{mezonUrl}/{userName}";
            Post(url, input);
           
        }

    }
}
