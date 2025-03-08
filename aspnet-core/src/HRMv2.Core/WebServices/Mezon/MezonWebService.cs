using Abp.Dependency;
using Abp.Runtime.Session;
using Amazon.S3.Model;
using Google.Apis.Auth.OAuth2.Responses;
using HRMv2.Configuration;
using HRMv2.Manager.Notifications.NotifyToChannel.Dto;
using HRMv2.Manager.Notifications.SendMezonDM.Dto;
using HRMv2.WebServices.Mezon.Dto;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Formats.Asn1;
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
        private readonly IConfiguration _configuration;
        public MezonWebService(HttpClient httpClient, IConfiguration configuration, IAbpSession abpSession, IIocResolver iocResovler) : base(httpClient, abpSession, iocResovler)
        {
            _isNotifyToMezon = configuration.GetValue<string>($"{serviceName}:EnableKomuNotification", "true");
            _configuration = configuration;
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
                Logger.LogInformation("mezonUrl null or empty");
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
                Logger.LogInformation("channelUrlToSend null or empty");
                return;
            }
            var url = $"{mezonUrl}/{userName}";
            Post(url, input);
           
        }


        public async Task<AuthOauth2Mezon> GetTokenForOauth2Mezon(string code)
        {
            var url = _configuration.GetValue<string>("Oauth2Mezon:Url_Oauth2Mezon");
            var urlInfo = _configuration.GetValue<string>("Oauth2Mezon:Url_UserInfo");
            var client_id = _configuration.GetValue<string>("Oauth2Mezon:Client_Id");
            var client_secret = _configuration.GetValue<string>("Oauth2Mezon:Client_Secret");
            var grant_type = _configuration.GetValue<string>("Oauth2Mezon:Grant_Type");
            var redirect_uri = _configuration.GetValue<string>("Oauth2Mezon:Redirect_URI");

            var formData = new Dictionary<string, string>
            {
               { "client_id", client_id },
                       { "client_secret", client_secret },
                       { "grant_type", grant_type },
                       { "redirect_uri", redirect_uri },
                       { "code", code }
             };

            var response = await PostFormUrlEncodedAsync<TokenResponse>(url, formData);
            SetAuthorizationToken(response.AccessToken);

            var infoAuth =  await PostAsync<AuthOauth2Mezon>(urlInfo, null);

            return infoAuth;
            
        }

    }
}
