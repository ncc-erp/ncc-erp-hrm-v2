using Abp.Authorization;
using HRMv2.Manager.Categories.Banks.Dto;
using HRMv2.Manager.Timesheet;
using HRMv2.Manager.Timesheet.Dto;
using HRMv2.NCC;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HRMv2.Manager.Timesheet.Dto.InpuReviewInternFromTSDto;

namespace HRMv2.APIs.Timesheet
{
    [AbpAuthorize]
    public class TimesheetAppService: HRMv2AppServiceBase
    {
        private readonly TimesheetManager _timesheetManager;
        public TimesheetAppService(TimesheetManager timesheetManager)
        {
            _timesheetManager = timesheetManager;
        }

        [HttpPost]
        [AbpAllowAnonymous]
        [NccAuthentication]

        public async Task UpdateAvatarFromTimesheet (AvatarDto input)
        {
            await _timesheetManager.UpdateAvatarFromTimesheet(input);
        }

        [AbpAllowAnonymous]
        [NccAuthentication]
        public Task ReviewInternFromTimesheet(InputCreateRequestHrmv2Dto input)
        {
          return _timesheetManager.ReviewInternFromTimesheet(input);
        }

        [AbpAllowAnonymous]
        [HttpPost]
        public async Task<string> ComplainPayslipMail(InputcomplainPayslipDto input)
        {
           return await _timesheetManager.ComplainPayslipMail(input);
        }

        [AbpAllowAnonymous]
        [HttpPost]
        public async Task<string> ConfirmPayslipMail(InputConfirmPayslipMailDto input)
        {
            return await _timesheetManager.ConfirmPayslipMail(input);
        }
        [AbpAllowAnonymous]
        [HttpGet]
        public GetUserInfoByEmailDto GetUserInfoByEmail(string email)
        {
            return  _timesheetManager.GetUserInfoByEmail(email);
        }
        [AbpAllowAnonymous]
        [HttpGet]
        public List<ItemInfoDto> GetAllBanks()
        {
            return  _timesheetManager.GetAllBanks();
        }

        [AbpAllowAnonymous]
        [HttpGet]
        public GetInfoToUPDateProfile GetInfoToUpdate(string email)
        {
            return _timesheetManager.GetInfoToUpdate(email);
        }

        [AbpAllowAnonymous]
        [HttpPost]
        public async Task<ResultUpdateInfo> CreateRequestUpdateUserInfo(UpdateUserInfoDto input)
        {
            return await _timesheetManager.CreateRequestUpdateUserInfo(input);
        }







    }
}