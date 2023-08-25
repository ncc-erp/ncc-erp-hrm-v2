using NccCore.Extension;
using NccCore.Paging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HRMv2.Entities;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Abp.UI;
using Microsoft.AspNetCore.Mvc;
using HRMv2.Manager.Categories;
using Abp.Authorization;
using HRMv2.Authorization;

namespace HRMv2.APIs.Branchs
{
    [AbpAuthorize]
    public class BranchAppService:HRMv2AppServiceBase
    {
        private readonly BranchManager _branhcManager;
        public BranchAppService(BranchManager branchManager)
        {
            _branhcManager = branchManager;
        }

        [HttpGet]
        public List<BranchDto> GetAll()
        {
            return  _branhcManager.GetAll();
        }
        [HttpPost]

        [AbpAuthorize(PermissionNames.Category_Branch_View)]
        public async Task<GridResult<BranchDto>> GetAllPaging(GridParam input)
        {
            return await _branhcManager.GetAllPaging(input);
        }

        [HttpPost]
        [AbpAuthorize(PermissionNames.Category_Branch_Create)]
        public async Task<BranchDto> Create(BranchDto input)
        {
            return await _branhcManager.Create(input);
        }

        [HttpPut]
        [AbpAuthorize(PermissionNames.Category_Branch_Edit)]
        public async Task<BranchDto> Update(BranchDto input)
        {
            return await _branhcManager.Update(input);
        }

        [HttpDelete]
        [AbpAuthorize(PermissionNames.Category_Branch_Delete)]
        public async Task<long> Delete(long id)
        {
            return await _branhcManager.Delete(id);
        }
    }
}
