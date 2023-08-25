using Abp.Authorization;
using HRMv2.Authorization;
using HRMv2.Manager.Debts;
using HRMv2.Manager.Debts.PaymentPlansManager.Dto;
using Microsoft.AspNetCore.Mvc;
using NccCore.Paging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMv2.APIs.DebtPaymentPlans
{
    [AbpAuthorize]
    public class DebtPaymentPlanAppService : HRMv2AppServiceBase
    {
        private readonly DebtManager _debtManager;
        public DebtPaymentPlanAppService(DebtManager debtManager)
        {
            _debtManager = debtManager;
        }

        [HttpGet]
        public List<PaymentPlanDto> GetAll()
        {
            return _debtManager.PaymentPlan.GetAll();
        }

        [HttpGet]
        public PaymentPlanDto Get(long id)
        {
            return _debtManager.PaymentPlan.Get(id);
        }

        [HttpGet]
        public List<PaymentPlanDto> GetPaymentPlansByDebId(long debtId)
        {
            return _debtManager.PaymentPlan.GetPaymentPlansByDebId(debtId);
        }

        [HttpPost]
        public async Task<GridResult<PaymentPlanDto>> GetAllPaging(GridParam input)
        {
            return await _debtManager.PaymentPlan.GetAllPaging(input);
        }

        [AbpAuthorize(PermissionNames.Debt_DebtDetail_GeneratePaymentPlan)]
        public async Task<GeneratePlanDto> GeneratePlan(GeneratePlanDto input)
        {
            return await _debtManager.PaymentPlan.GeneratePlan(input);
        }

        [HttpPost]
        [AbpAuthorize(PermissionNames.Debt_DebtDetail_AddPaymentPlan)]
        public async Task<CreatePaymentPlanDto> Create(CreatePaymentPlanDto input)
        {
            return await _debtManager.PaymentPlan.Create(input);
        }

        [HttpPut]
        [AbpAuthorize(PermissionNames.Debt_DebtDetail_EditPaymentPlan)]
        public async Task<UpdatePaymentPlanDto> Update(UpdatePaymentPlanDto input)
        {
            return await _debtManager.PaymentPlan.Update(input);
        }

        [HttpDelete]
        [AbpAuthorize(PermissionNames.Debt_DebtDetail_DeletePaymentPlan)]
        public async Task<long> Delete(long id)
        {
            return await _debtManager.PaymentPlan.Delete(id);
        }
    }
}
