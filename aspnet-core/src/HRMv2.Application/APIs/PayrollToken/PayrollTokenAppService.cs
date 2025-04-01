using Abp.Authorization;
using HRMv2.Authorization;
using HRMv2.Manager.PayrollTokens;
using HRMv2.Manager.PayrollTokens.Dto;
using HRMv2.Manager.Salaries.SalaryCalculators.Dto;
using Microsoft.AspNetCore.Mvc;
using NccCore.Paging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMv2.APIs.PayrollToken
{
    public class PayrollTokenAppService : HRMv2AppServiceBase
    {
        private readonly PayrollTokneManager _payrollTokneManager;

        public PayrollTokenAppService(PayrollTokneManager payrollTokneManager)
        {
            _payrollTokneManager = payrollTokneManager;
        }
        [HttpPost]
        [AbpAuthorize(PermissionNames.Payroll_Token_View)]
        public async Task<GridResult<PayrollTokenDto>> GetAllPaging(GridParam input)
        {
            return await _payrollTokneManager.GetAllPaging(input);
        }

        [HttpPost]
        [AbpAuthorize(PermissionNames.Payroll_PaySlip_Token_View)]
        public async Task<GridResult<PayslipTokenDto>> GetAllPagingPayslip(long payrollId, GridParam input)
        {
            return await _payrollTokneManager.GetAllPagingPayslip(input, payrollId);
        }

        [HttpDelete]
        [AbpAuthorize(PermissionNames.Payroll_Token_Delete)]
        public async Task DeletePayrollToken(long payrollId)
        {
            _payrollTokneManager.DeletePayrollToken(payrollId);
        }
        [HttpDelete]
        [AbpAuthorize(PermissionNames.Payroll_PaySlip_Token_Delete)]
        public async Task DeletePaySlipToken(long id)
        {
            _payrollTokneManager.DeletePaySlipToken(id);
        }
        [HttpPost]
        [AbpAuthorize(PermissionNames.Payroll_PaySlip_Token_Send)]
        public async Task SendToken(long paySlipTokenId)
        {
            _payrollTokneManager.SendToken(paySlipTokenId);
        }
    }
}
