using Abp.Authorization;
using HRMv2.Authorization;
using HRMv2.Manager.Categories.PunishmentTypes;
using HRMv2.Manager.Categories.PunishmentTypes.Dto;
using Microsoft.AspNetCore.Mvc;
using NccCore.Paging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMv2.APIs.PunishmentTypes
{
    [AbpAuthorize]
    public class PunishmentTypeAppService : HRMv2AppServiceBase
    {
        private readonly PunishmentTypeManager _punishmentTypeManager;
        public PunishmentTypeAppService(PunishmentTypeManager punishmentTypeManager)
        {
            _punishmentTypeManager = punishmentTypeManager;
        }

        [HttpPost]
        [AbpAuthorize(PermissionNames.Category_PunishmentType_View)]
        public async Task<GridResult<PunishmentTypeDto>> GetAllPaging(GridParam input)
        {
            return await _punishmentTypeManager.GetAllPaging(input);
        }

        [HttpPost]
        [AbpAuthorize(PermissionNames.Category_PunishmentType_Create)]
        public async Task<PunishmentTypeDto> Create(PunishmentTypeDto input)
        {
            return await _punishmentTypeManager.Create(input);
        }

        [HttpPut]
        [AbpAuthorize(PermissionNames.Category_PunishmentType_Edit)]
        public async Task<PunishmentTypeDto> Update(PunishmentTypeDto input)
        {
            return await _punishmentTypeManager.Update(input);
        }

        [HttpDelete]
        [AbpAuthorize(PermissionNames.Category_PunishmentType_Delete)]
        public async Task<long> Delete(long id)
        {
            return await _punishmentTypeManager.Delete(id);
        }

        [HttpGet]
        public List<PunishmentTypeDto> GetAll()
        {
            return _punishmentTypeManager.GetAll();
        }
    }
}
