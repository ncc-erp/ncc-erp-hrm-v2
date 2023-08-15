using Abp.Authorization;
using HRMv2.Authorization;
using HRMv2.Manager.Debts;
using HRMv2.Manager.Debts.Dto;
using HRMv2.Manager.Notifications.Email.Dto;
using Microsoft.AspNetCore.Mvc;
using NccCore.Paging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMv2.APIs.Debts
{

    public class DebtAppService : HRMv2AppServiceBase
    {
        private readonly DebtManager _debtManager;
        public DebtAppService(DebtManager debtManager)
        {
            _debtManager = debtManager;
        }

        [HttpGet]
        public List<DebtDto> GetAll()
        {
            return _debtManager.GetAll();
        }

        [HttpGet]
        [AbpAuthorize(PermissionNames.Debt_DebtDetail_View)]
        public DebtDto Get(long id)
        {
            return _debtManager.Get(id);
        }

        [HttpPost]
        [AbpAuthorize(PermissionNames.Debt_View)]
        public async Task<GridResult<DebtDto>> GetAllPaging(GetDebtEmployeeInputDto input)
        {
            return await _debtManager.GetAllPaging(input);
        }

        [HttpPost]
        [AbpAuthorize(PermissionNames.Employee_EmployeeDetail_TabDebt_View)]
        public async Task<GridResult<DebtDto>> GetByEmployeeId(long id, GridParam input)
        {
            return await _debtManager.GetByEmployeeId(id, input);
        }

        [HttpPost]
        [AbpAuthorize(PermissionNames.Debt_Create, PermissionNames.Employee_EmployeeDetail_TabDebt_Add)]
        public async Task<long> Create(CreateDebtDto input)
        {
            return await _debtManager.Create(input);
        }

        [HttpPut]
        [AbpAuthorize(PermissionNames.Debt_DebtDetail_Edit)]
        public async Task<UpdateDebtDto> Update(UpdateDebtDto input)
        {
            return await _debtManager.Update(input);
        }

        [HttpGet]
        [AbpAuthorize(PermissionNames.Debt_DebtDetail_SetDone)]
        public long SetDone(long id)
        {
            return _debtManager.SetDone(id);
        }

        [HttpDelete]
        [AbpAuthorize(PermissionNames.Debt_Delete)]
        public async Task<long> Delete(long id)
        {
            return await _debtManager.Delete(id);
        }

        [HttpPost]
        public void SendMailToOneEmployee(SendMailDto input)
        {
            _debtManager.SendMailToOneEmployee(input);
        }

        [HttpGet]
        public MailPreviewInfoDto GetDebtTemplate(long debtId)
        {
            return _debtManager.GetDebtTemplate(debtId);
        }

        [HttpPost]
        public string SendMailToAllEmployee(GetDebtEmployeeInputDto input)
        {
           return _debtManager.SendMailToAllEmployee(input);
        }
    }
}
