using Abp.Dependency;
using Abp.Runtime.Session;
using HRMv2.Manager.ChangeEmployeeWorkingStatuses.Dto;
using HRMv2.Manager.Salaries.CalculateSalary.Dto;
using HRMv2.NccCore;
using HRMv2.WebServices.Dto;
using HRMv2.WebServices.Timesheet.Dto;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace HRMv2.WebServices.Timesheet
{
    public class TimesheetWebService: BaseWebService
    {
        public TimesheetWebService(
            HttpClient httpClient,
            IAbpSession abpSession,
            IIocResolver iocResovler
        ) : base(httpClient, abpSession, iocResovler)
        {
        }

        public void UpdateAvatarToTimesheet(UploadAvatarDto input)
        {
            var url = $"/api/services/app/HRMv2/UpdateAvatarFromHrm";
            Post(url, input);
        }

        public List<TimesheetOTDto> GetOTTimesheets(InputCollectDataForPayslipDto input)
        {
            var response = PostAsync<AbpResponseResult<List<TimesheetOTDto>>>("api/services/app/HRMv2/GetOTTimesheets", input).Result;
            return response?.Result;
        }
        public List<GetRequestDateDto> GetAllRequestDays(InputCollectDataForPayslipDto input)
        {
            var response = PostAsync<AbpResponseResult<List<GetRequestDateDto>>>("api/services/app/HRMv2/GetAllRequestDay", input).Result;
            return response?.Result;
        }
       
        public HashSet<DateTime> GetSettingOffDates(int year, int month)
        {
            var response = GetAsync<AbpResponseResult<HashSet<DateTime>>>("api/services/app/HRMv2/GetSettingOffDates?year=" + year + "&month=" + month).Result;
            return response?.Result;
        }

        public List<ChamCongInfoDto> GetChamCongInfo(InputCollectDataForPayslipDto input)
        {
            var response = PostAsync<AbpResponseResult<List<ChamCongInfoDto>>>("api/services/app/HRMv2/GetChamCongInfo", input).Result;
            return response?.Result;
        }

        public void  CreateTimesheetUser(CreateOrUpdateUserOtherToolDto input)
        {
             Post("api/services/app/HRMv2/CreateUser", input);
        }

        public void UpdateTimesheetUser(CreateOrUpdateUserOtherToolDto input)
        {
            Post("api/services/app/HRMv2/UpdateUser", input);
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

        public GetResultConnectDto CheckConnectToTimesheet()
        {
            var res = GetAsync<AbpResponseResult<GetResultConnectDto>>("api/services/app/Public/CheckConnect").Result;
            if (res == null)
            {
                return new GetResultConnectDto
                {
                    IsConnected = false,
                    Message = "Can not connect to Timesheet"
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
