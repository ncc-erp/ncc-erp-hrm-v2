using Abp.Configuration;
using Abp.Domain.Services;
using Abp.UI;
using HRMv2.Configuration;
using HRMv2.Entities;
using HRMv2.Manager.Notifications.Email;
using HRMv2.Manager.Notifications.Email.Dto;
using HRMv2.Manager.Notifications.SendDMToMezon.Dto;
using HRMv2.Manager.Notifications.SendDMToMezon.Dto.HRMv2.Manager.Notifications.SendDMToMezon.Dto;
using HRMv2.NccCore;
using HRMv2.WebServices.Mezon;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMv2.Manager.Notifications.SendDMToMezon
{
    public class SendDMService : DomainService
    {
        private readonly MezonService _mezonService;
        private readonly ISettingManager _settingManager;
        private readonly EmailManager _emailManager;
        private readonly IWorkScope _workScope;
        public SendDMService(IWorkScope workScope, MezonService mezonService, ISettingManager settingManager,EmailManager emailManager)
        {
            _mezonService = mezonService;
            _settingManager = settingManager;
            _emailManager = emailManager;
            _workScope = workScope;
        }

        public void SendDMToUser(MezonPreviewInfoDto input)
        {
           
            var url = _settingManager.GetSettingValueForApplication(AppSettingNames.MezonClanWebhookURL);

            var message = input.BodyMessage;
            InputMezonDM inputMezonDM = JsonConvert.DeserializeObject<InputMezonDM>(message);
            var start = inputMezonDM.Content.Text.IndexOf("http");
            var end = inputMezonDM.Content.Text.IndexOf(" ", start);
            inputMezonDM.Content.Link = new List<StartEnd>();
            inputMezonDM.Content.Link.Add(new StartEnd
            {
                Start = start,
                End = end
            });
            _mezonService.SendDirectMessageToUser(inputMezonDM,url,input.SendToUser);
        }

        
    }
}
