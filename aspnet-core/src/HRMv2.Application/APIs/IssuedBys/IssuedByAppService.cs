using Abp.Authorization;
using HRMv2.Authorization;
using HRMv2.Manager.Categories.IssuedBys;
using HRMv2.Manager.Categories.IssuedBys.Dto;
using Microsoft.AspNetCore.Mvc;
using NccCore.Paging;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HRMv2.APIs.IssuedBy
{
    [AbpAuthorize]
    public class IssuedByAppService : HRMv2AppServiceBase
    {
        private readonly IssuedByManager _issuedByManager;
        public IssuedByAppService(IssuedByManager issuedByManager)
        {
            _issuedByManager = issuedByManager;
        }

        [HttpGet]
        public List<IssuedByDto> GetAll()
        {
            return _issuedByManager.GetAll();
        }

        [HttpPost]
        [AbpAuthorize(PermissionNames.Category_IssuedBy_View)]
        public async Task<GridResult<IssuedByDto>> GetAllPaging(GridParam input)
        {
            return await _issuedByManager.GetAllPaging(input);
        }

        [HttpPost]
        [AbpAuthorize(PermissionNames.Category_IssuedBy_Create)]
        public async Task<IssuedByDto> Create(IssuedByDto input)
        {
            return await _issuedByManager.Create(input);

        }

        [HttpPut]
        [AbpAuthorize(PermissionNames.Category_IssuedBy_Edit)]
        public async Task<IssuedByDto> Update(IssuedByDto input)
        {
            return await _issuedByManager.Update(input);
        }

        [HttpDelete]
        [AbpAuthorize(PermissionNames.Category_IssuedBy_Delete)]
        public async Task<long> Delete(long id)
        {
            return await _issuedByManager.Delete(id);
        }
    }
}