using Abp.Authorization;
using HRMv2.Authorization;
using HRMv2.Manager.Employees.Dto;
using HRMv2.Manager.MezonTokens;
using HRMv2.Manager.MezonTokens.Dto;
using HRMv2.Manager.Salaries.SalaryCalculators.Dto;
using HRMv2.WebServices.Mezon.Dto;
using Microsoft.AspNetCore.Mvc;
using NccCore.Paging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMv2.APIs.PayrollToken
{
    public class MezonTokenAppService : HRMv2AppServiceBase
    {
        private readonly MezonTokenManager _mezonTokneManager;

        public MezonTokenAppService(MezonTokenManager mezonTokneManager)
        {
            _mezonTokneManager = mezonTokneManager;
        }
        [HttpPost]
        [AbpAuthorize(PermissionNames.Mezon_Token_View)]
        public async Task<ResultMezonToken> GetListMezonToken(GridParam input)
        {
            return await _mezonTokneManager.GetAllPaging(input);
        }

        [HttpDelete]
        [AbpAuthorize(PermissionNames.Mezon_Token_Delete)]
        public async Task DeleteMezonToken(long id)
        {
            await _mezonTokneManager.DeleteMezonTokenById(id);
        }
        [HttpDelete]
        [AbpAuthorize(PermissionNames.Mezon_Token_DeleteAll)]
        public async Task DeleteAllPending()
        {
           await _mezonTokneManager.DeleteAllPending();
        }

        [HttpPost]
        [AbpAuthorize(PermissionNames.Mezon_Token_Create)]
        public async Task<MezonTokenDto> Create(MezonTokenDto input)
        {
            return await _mezonTokneManager.Create(input);
        }
        [HttpPut]
        [AbpAuthorize(PermissionNames.Mezon_Token_Edit)]
        public async Task<MezonTokenDto> EditMezonToken(MezonTokenDto input)
        {
            return await _mezonTokneManager.EditMezonToken(input);
        }
        [HttpPost]
        [AbpAuthorize(PermissionNames.Mezon_Token_Export)]
        public async Task<FileBase64Dto> ExportMezonToken(GridParam input)
        {
            return await _mezonTokneManager.ExportMezonToken(input);
        }

        [HttpPost]
        [AbpAuthorize(PermissionNames.Mezon_Token_SendToken)]
        public async Task<AuthResponse> SendToken(InputSendMezonToken input)
        {
           return await _mezonTokneManager.SendToken(input);
        }

        [HttpPost]
        [AbpAuthorize(PermissionNames.Mezon_Token_SendTokenAll)]
        public async Task<string> SendTokenToAllPending()
        {
          return await _mezonTokneManager.SentTokenToAllPending();
        }

    }
}
