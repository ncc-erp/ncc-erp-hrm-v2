using Abp.Dependency;
using Abp.Runtime.Session;
using DocumentFormat.OpenXml.Spreadsheet;
using HRMv2.Configuration;
using HRMv2.Manager.Notifications.SendDMToMezon.Dto;
using Microsoft.Extensions.Configuration;
using NodaTime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace HRMv2.WebServices.Mezon
{
    public class MezonService : BaseWebService
    {
        private const string serviceName = "MezonService";
        private readonly string _mezonDevChannelUrl;
        private readonly string _isNotifyToMezon;

        public MezonService(HttpClient httpClient, IConfiguration configuration, IAbpSession abpSession, IIocResolver iocResovler) : base(httpClient, abpSession, iocResovler)
        {
            _mezonDevChannelUrl = configuration.GetValue<string>($"{serviceName}:DevModeUrl", "");
            _isNotifyToMezon = configuration.GetValue<string>($"{serviceName}:EnableKomuNotification", "true");
        }

        public void NotifyToChannel(string mezonMessage, string mezonUrl)
        {
            if (_isNotifyToMezon != "true")
            {
                Logger.Info("_isNotifyToMezon=" + _isNotifyToMezon + " => stop");
                return;
            }
            var channelUrlToSend = string.IsNullOrEmpty(_mezonDevChannelUrl) ? mezonUrl : _mezonDevChannelUrl;
            if (string.IsNullOrEmpty(channelUrlToSend))
            {
                Logger.Error("channelUrlToSend null or empty");
                return;
            }
            Post(channelUrlToSend, new { type = "HRM", message = new { username = "HRM", t = mezonMessage } });
        }

        public void SendDMToMezon(DmMezonDto input)
        {
            if (_isNotifyToMezon != "true")
            {
                Logger.Info("_isNotifyToMezon=" + _isNotifyToMezon + " => stop");
                return;
            }
            var channelUrlToSend = string.IsNullOrEmpty(_mezonDevChannelUrl) ? input.Url : _mezonDevChannelUrl;
            if (string.IsNullOrEmpty(channelUrlToSend))
            {
                Logger.Error("channelUrlToSend null or empty");
                return;
            }
            var url = $"{input.Url}/{input.UserName}";
            Post(url, new { content = input.Content });
        }

    }
}
