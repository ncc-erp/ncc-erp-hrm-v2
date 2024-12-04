using Abp.Configuration;
using Abp.Domain.Services;
using HRMv2.Manager.Notifications.SendDMToMezon.Dto;
using HRMv2.NccCore;
using HRMv2.WebServices.Mezon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMv2.Manager.Notifications.SendDMToMezon
{
    public class SendDmToMezon : DomainService
    {
        private readonly MezonService _mezonService;
        private readonly ISettingManager _settingManager;
        public SendDmToMezon(IWorkScope workScope, MezonService mezonService, ISettingManager settingManager)
        {
            _mezonService = mezonService;
            _settingManager = settingManager;
        }

        public void SendDMToMezon(DmMezonDto input)
        {
            _mezonService.SendDMToMezon(input);
        }


    }
}
