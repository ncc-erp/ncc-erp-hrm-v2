using Abp.Authorization;
using HRMv2.Authorization;
using HRMv2.Manager.Debts;
using HRMv2.Manager.Debts.PaidsManagger.Dto;
using Microsoft.AspNetCore.Mvc;
using NccCore.Paging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMv2.APIs.DebtPaids
{
    [AbpAuthorize]
    public class DebtPaidAppService : HRMv2AppServiceBase
    {
        private readonly DebtManager _debtManager;
        public DebtPaidAppService(DebtManager debtManager)
        {
            _debtManager = debtManager;
        }

        [HttpGet]
        public List<DebtPaidDto> GetAll()
        {
            return _debtManager.Paid.GetAll();
        }

        [HttpGet]
        public DebtPaidDto Get(long id)
        {
            return _debtManager.Paid.Get(id);
        }

        [HttpGet]
        public List<DebtPaidDto> GetPaidsByDebtId(long debtId)
        {
            return _debtManager.Paid.GetPaidsByDebtId(debtId);
        }

        [HttpPost]
        public async Task<GridResult<DebtPaidDto>> GetAllPaging(GridParam input)
        {
            return await _debtManager.Paid.GetAllPaging(input);
        }

        [HttpPost]
        [AbpAuthorize(PermissionNames.Debt_DebtDetail_AddDebtPaid)]
        public async Task<CreatePaidDto> Create(CreatePaidDto input)
        {
            return await _debtManager.Paid.Create(input);
        }

        [HttpPut]
        [AbpAuthorize(PermissionNames.Debt_DebtDetail_EditDebtPaid)]
        public async Task<UpdatePaidDto> Update(UpdatePaidDto input)
        {
            return await _debtManager.Paid.Update(input);
        }

        [HttpDelete]
        [AbpAuthorize(PermissionNames.Debt_DebtDetail_DeleteDebtPaid)]
        public async Task<long> Delete(long id)
        {
            return await _debtManager.Paid.Delete(id);
        }
    }
}
