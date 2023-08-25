using Abp.Dependency;
using Abp.Runtime.Session;
using HRMv2.Manager.ChangeEmployeeWorkingStatuses.Dto;
using HRMv2.WebServices.Dto;
using HRMv2.WebServices.Talent.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace HRMv2.WebServices.Talent
{
    public class TalentWebService : BaseWebService
    {
        public TalentWebService(HttpClient httpClient, IAbpSession abpSession, IIocResolver iocResovler) : base(httpClient, abpSession, iocResovler)
        {

        }
        public void CreateTalentUser(CreateOrUpdateUserOtherToolDto input)
        {
            Post($"api/services/app/Hrmv2/CreateUserFromHRM", input);
        }
        public void UpdateTalentUser(CreateOrUpdateUserOtherToolDto input)
        {
            Post($"api/services/app/Hrmv2/UpdateUserFromHRM", input);
        }

        public void ConfirmUserQuit(InputToUpdateUserStatusDto input)
        {
            Post("api/services/app/Hrmv2/ConfirmUserQuit", input);
        }
        public void ConfirmUserPause(InputToUpdateUserStatusDto input)
        {
            Post("api/services/app/Hrmv2/ConfirmUserPause", input);
        }
        public void ConfirmUserMaternityLeave(InputToUpdateUserStatusDto input)
        {
            Post("api/services/app/Hrmv2/ConfirmUserMaternityLeave", input);
        }
        public void ConfirmUserBackToWork(InputToUpdateUserStatusDto input)
        {
            Post("api/services/app/Hrmv2/ConfirmUserBackToWork", input);
        }
        public void UpdateOnboardStatus(UpdateTalentOnboardDto input)
        {
            Post("api/services/app/Hrmv2/UpdateOnboardStatus", input);
        }
        public  GetResultConnectDto CheckConnectToTalent()
        {
            var res =  GetAsync<AbpResponseResult<GetResultConnectDto>>("api/services/app/Public/CheckConnect").Result;
            if (res == null)
            {
                return new GetResultConnectDto
                {
                    IsConnected = false,
                    Message = "Can not connect to Talent"
                };
            }
            if(res.Error != null)
            {
                return new GetResultConnectDto
                {
                    IsConnected = false,
                    Message = res.Error.Message
                };
            }
            return res.Result;

        }

    }
}
