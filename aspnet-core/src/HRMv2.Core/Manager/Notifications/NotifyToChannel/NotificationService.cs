using Abp.Configuration;
using Abp.Domain.Services;
using HRMv2.Configuration;
using HRMv2.Constants;
using HRMv2.Manager.Notifications.NotifyToChannel.Dto;
using HRMv2.NccCore;
using HRMv2.Utils;
using HRMv2.WebServices.Komu;
using HRMv2.WebServices.Mezon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMv2.Manager.Notifications.NotifyToChannel
{
    public class NotificationService: DomainService
    {
        private readonly KomuService _komuService;
        private readonly MezonService _mezonService;
        private readonly string _platform;
        private readonly ISettingManager _settingManager;
        public NotificationService(IWorkScope workScope, KomuService komuService, MezonService mezonService, ISettingManager settingManager) 
        {
            _komuService = komuService;
            _mezonService = mezonService;
            _settingManager = settingManager;
            _platform = _settingManager.GetSettingValueForApplication(AppSettingNames.NotifyToPlatform);
        }

        public void NotifyToITChannel(string message)
        {
            if (_platform == AppConsts.NotifyToMezon)
            {
                var channelUrl = _settingManager.GetSettingValueForApplication(AppSettingNames.ITMezonChannel);
                _mezonService.NotifyToChannel(message, channelUrl);
            } 
            else if (_platform == AppConsts.NotifyToKomu) 
            {
                var channelId = _settingManager.GetSettingValueForApplication(AppSettingNames.KomuITChannelId);
                _komuService.NotifyToChannel(message, channelId);
            }
        }

        public void NotifyToPayrollChannel(string message)
        {
            if (_platform == AppConsts.NotifyToMezon)
            {
                var channelUrl = _settingManager.GetSettingValueForApplication(AppSettingNames.PayrollMezonChannel); 
                _mezonService.NotifyToChannel(message, channelUrl);
            }
            else if ( _platform == AppConsts.NotifyToKomu)
            {
                var channelId = _settingManager.GetSettingValueForApplication(AppSettingNames.PayrollChannelId);
                _komuService.NotifyToChannel(message, channelId);
            }
        }

        public async Task ChangeNotifySettingAsync(NotifyToChannelDto input)
        {
            await _settingManager.ChangeSettingForApplicationAsync(AppSettingNames.NotifyToPlatform, input.NotifyPlatform);
            if (input.NotifyPlatform == AppConsts.NotifyToMezon)
            {
                await _settingManager.ChangeSettingForApplicationAsync(AppSettingNames.ITMezonChannel, input.ITChannel);
                await _settingManager.ChangeSettingForApplicationAsync(AppSettingNames.PayrollMezonChannel, input.PayrollChannel);
            } 
            else if (input.NotifyPlatform == AppConsts.NotifyToKomu)
            {
                await _settingManager.ChangeSettingForApplicationAsync(AppSettingNames.KomuITChannelId, input.ITChannel);
                await _settingManager.ChangeSettingForApplicationAsync(AppSettingNames.PayrollChannelId, input.PayrollChannel);
            }
        }

        public async Task<NotifyToChannelDto> GetNotifySettingAsync()
        {
            var platform = await _settingManager.GetSettingValueForApplicationAsync(AppSettingNames.NotifyToPlatform);
            var setting = new NotifyToChannelDto();
            setting.NotifyPlatform = platform;
            if (platform == AppConsts.NotifyToMezon)
            {
                setting.ITChannel = await _settingManager.GetSettingValueForApplicationAsync(AppSettingNames.ITMezonChannel);
                setting.PayrollChannel = await _settingManager.GetSettingValueForApplicationAsync(AppSettingNames.PayrollMezonChannel);
            }
            else if (platform == AppConsts.NotifyToKomu)
            {

                setting.ITChannel = await _settingManager.GetSettingValueForApplicationAsync(AppSettingNames.KomuITChannelId);
                setting.PayrollChannel = await _settingManager.GetSettingValueForApplicationAsync(AppSettingNames.PayrollChannelId);
            }
            return setting;
        }

        public string GetTagUser(string email, string platform = "")
        {
            var tagPlatform = string.IsNullOrEmpty(platform) ? _platform : platform;
            if (tagPlatform == AppConsts.NotifyToMezon)
            {
                return "@" + CommonUtil.GetUserNameByEmail(email);
            }
            else if (tagPlatform == AppConsts.NotifyToKomu)
            {
                return "${" + CommonUtil.GetUserNameByEmail(email) + "}";
            }
            else
            {
                return "";
            }
        }
    }
}
