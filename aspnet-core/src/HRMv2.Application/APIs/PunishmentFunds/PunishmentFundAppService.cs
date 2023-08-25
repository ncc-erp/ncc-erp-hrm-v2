using Abp.Authorization;
using HRMv2.Authorization;
using HRMv2.Manager.PunishmentFunds;
using HRMv2.Manager.PunishmentFunds.Dto;
using Microsoft.AspNetCore.Mvc;
using NccCore.Paging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace HRMv2.APIs.PunishmentFunds
{
    [AbpAuthorize]
    public class PunishmentFundAppService : HRMv2AppServiceBase
    {
        public readonly PunishmentFundManager _punishmentFundManager;
        public  PunishmentFundAppService(PunishmentFundManager punishmentFundManager)
        {
            _punishmentFundManager = punishmentFundManager;
        }
        [HttpPost]
        public async Task<GridResult<GetAllPunishmentFundsDto>> GetAllPaging(InputToGetAllPagingDto input)
        {
            return await _punishmentFundManager.GetAllPaging(input);
        }
        [HttpPost]
        [AbpAuthorize(PermissionNames.PunishmentFund_Add)]
        public async Task<AddEditPunishmentFundDto> Add (AddEditPunishmentFundDto input)
        {
            return await _punishmentFundManager.Add(input);
        }
        [HttpPut]
        [AbpAuthorize(PermissionNames.PunishmentFund_Edit)]
        public async Task<AddEditPunishmentFundDto> Update(AddEditPunishmentFundDto input)
        {
            return await _punishmentFundManager.Update(input);
        }
        [HttpDelete]
        [AbpAuthorize(PermissionNames.PunishmentFund_Delete)]
        public async Task<long> Delete(long Id)
        {
            return await _punishmentFundManager.Delete(Id);
        }
        [HttpPost]
        [AbpAuthorize(PermissionNames.PunishmentFund_Disburse)]
        public async Task<AddEditPunishmentFundDto> Disburse(AddEditPunishmentFundDto input)
        {
            return await _punishmentFundManager.Disburse(input);
        }
    }
}
