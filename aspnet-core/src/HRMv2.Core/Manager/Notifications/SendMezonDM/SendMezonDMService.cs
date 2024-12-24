using Abp.Configuration;
using Abp.Domain.Services;
using Abp.UI;
using HRMv2.Configuration;
using HRMv2.Entities;
using HRMv2.Manager.Notifications.Email;
using HRMv2.Manager.Notifications.Email.Dto;
using HRMv2.Manager.Notifications.SendMezonDM.Dto;
using HRMv2.NccCore;
using HRMv2.WebServices.Mezon;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMv2.Manager.Notifications.SendMezonDM
{
    public class SendMezonDMService : DomainService
    {
        private readonly MezonWebService _mezonWebService;
        private readonly ISettingManager _settingManager;
        public SendMezonDMService(MezonWebService mezonService, ISettingManager settingManager)
        {
            _mezonWebService = mezonService;
            _settingManager = settingManager;
        }

        public void SendDMToUser(MezonPreviewInfoDto input)
        {
           
            var url = _settingManager.GetSettingValueForApplication(AppSettingNames.MezonClanWebhookURL);

            var message = input.BodyMessage;
            InputMezonDM inputMezonDM = JsonConvert.DeserializeObject<InputMezonDM>(message);            
            _mezonWebService.SendDirectMessageToUser(inputMezonDM,url,input.MezonUsername);
        }

        
    }
}
