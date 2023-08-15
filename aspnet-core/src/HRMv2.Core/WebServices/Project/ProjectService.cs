using Abp.Dependency;
using Abp.Runtime.Session;
using AutoMapper.Configuration;
using HRMv2.Manager;
using HRMv2.Manager.ChangeEmployeeWorkingStatuses.Dto;
using HRMv2.NccCore;
using HRMv2.WebServices.Dto;
using HRMv2.WebServices.Project.Dto;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace HRMv2.WebServices.Project
{
    public class ProjectService : BaseWebService
    {
        public ProjectService(
            HttpClient httpClient,
            IAbpSession abpSession,
            IIocResolver iocResovler
        ) : base(httpClient, abpSession, iocResovler)
        {
        }

        public void UpdateAvatarToProject(UploadAvatarDto input)
        {
            var url = $"/api/services/app/Hrmv2/UpdateAvatarFromHrm";
            Post(url, input);
        }
        public void CreateProjectUser(CreateOrUpdateUserOtherToolDto input)
        {
            Post("api/services/app/Hrmv2/CreateUserByHRM", input);
        }

        public void UpdateProjectUser(CreateOrUpdateUserOtherToolDto input)
        {
            Post("api/services/app/Hrmv2/UpdateUserFromHRM", input);
        }

        public void PlanUserQuitJob(InputToUpdateUserStatusDto input)
        {
            Post("api/services/app/Hrmv2/PlanUserQuit", input);
        }

        public void PlanUserPause(InputToUpdateUserStatusDto input)
        {
            Post("api/services/app/Hrmv2/PlanUserPause", input);
        }
        public void PlanUserMaternityLeave(InputToUpdateUserStatusDto input)
        {
            Post("api/services/app/Hrmv2/PlanUserMaternityLeave", input);
        }
        public void ConfirmUserQuit(InputToUpdateUserStatusDto input)
        {
            Post("api/services/app/Hrmv2/ConfirmUserQuit", input);
        }
        public void ConfirmUserBackToWork(InputToUpdateUserStatusDto input)
        {
            Post("api/services/app/Hrmv2/ConfirmUserBackToWork", input);
        }
        public void ConfirmUserPause(InputToUpdateUserStatusDto input)
        {
            Post("api/services/app/Hrmv2/ConfirmUserPause", input);
        }
        public void ConfirmUserMaternityLeave(InputToUpdateUserStatusDto input)
        {
            Post("api/services/app/Hrmv2/ConfirmUserMaternityLeave", input);
        }
        public GetResultConnectDto CheckConnectToProject()
        {
            var res = GetAsync<AbpResponseResult<GetResultConnectDto>>("api/services/app/Public/CheckConnect").Result;
            if (res == null)
            {
                return new GetResultConnectDto
                {
                    IsConnected = false,
                    Message = "Can not connect to Project"
                };
            }
            if (res.Error != null)
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
