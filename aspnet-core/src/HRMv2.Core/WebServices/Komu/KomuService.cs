using Abp.Dependency;
using Abp.Runtime.Session;
using Amazon.Runtime.Internal.Util;
using HRMv2.Constants;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace HRMv2.WebServices.Komu
{
    public class KomuService : BaseWebService
    {
        private const string serviceName = "KomuService";
        private readonly string _channelIdDevMode;
        private readonly string _isNotifyToKomu;

        public KomuService(
            HttpClient httpClient,
            IAbpSession abpSession,
            IConfiguration configuration,
            IIocResolver iocResovler
            ) : base(httpClient, abpSession, iocResovler)
        {
            _channelIdDevMode = configuration.GetValue<string>($"{serviceName}:ChannelIdDevMode", "");
            _isNotifyToKomu = configuration.GetValue<string>($"{serviceName}:EnableKomuNotification", "true");
        }
        public void NotifyToChannel(string komuMessage, string channelId)
        {
            if (_isNotifyToKomu != "true")
            {
                Logger.Error("_isNotifyToKomu=" + _isNotifyToKomu + " => stop");
                return;
            }
            var channelIdToSend = string.IsNullOrEmpty(_channelIdDevMode) ? channelId : _channelIdDevMode;
            if (string.IsNullOrEmpty(channelIdToSend))
            {
                Logger.Error("channelIdToSend null or empty");
                return;
            }
            Post(KomuUrlConstant.KOMU_CHANNELID, new { message = komuMessage, channelid = channelIdToSend });
        }
    }
}
