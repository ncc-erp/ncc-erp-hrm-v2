using Abp.Authorization;
using HRMv2.Authorization;
using HRMv2.Manager.Categories.Banks;
using HRMv2.Manager.Categories.Banks.Dto;
using Microsoft.AspNetCore.Mvc;
using NccCore.Paging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMv2.APIs.Banks
{

    [AbpAuthorize]
    public class BankAppService : HRMv2AppServiceBase
    {
        private readonly BankManager _bankManager;
        public BankAppService(BankManager bankManager)
        {
            _bankManager = bankManager;
        }

        [HttpGet]
        public List<BankDto> GetAll()
        {
            return _bankManager.GetAll();
        }

        [HttpPost]
        [AbpAuthorize(PermissionNames.Category_Bank_View)]
        public async Task<GridResult<BankDto>> GetAllPaging(GridParam input)
        {
            return await _bankManager.GetAllPaging(input);
        }

        [HttpPost]

        [AbpAuthorize(PermissionNames.Category_Bank_Create)]
        public async Task<BankDto> Create(BankDto input)
        {
            return await _bankManager.Create(input);
        }

        [HttpPut]

        [AbpAuthorize(PermissionNames.Category_Bank_Edit)]
        public async Task<BankDto> Update(BankDto input)
        {
            return await _bankManager.Update(input);
        }

        [HttpDelete]

        [AbpAuthorize(PermissionNames.Category_Bank_Delete)]
        public async Task<long> Delete(long id)
        {
            return await _bankManager.Delete(id);
        }
    }
}
