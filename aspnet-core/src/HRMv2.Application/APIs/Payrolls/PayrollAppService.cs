using Abp.Authorization;
using HRMv2.Authorization;
using HRMv2.Manager.Payrolls;
using HRMv2.Manager.Payrolls.Dto;
using Microsoft.AspNetCore.Mvc;
using NccCore.Paging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMv2.APIs.Payrolls
{
    [AbpAuthorize]
    public class PayrollAppService : HRMv2AppServiceBase
    {
        private readonly PayrollManager _payrollManager;
        public PayrollAppService(PayrollManager payrollManager)
        {
            _payrollManager = payrollManager;
        }

        [HttpPost]
        [AbpAuthorize(PermissionNames.Payroll_View)]
        public async Task<GridResult<GetPayrollDto>> GetAllPaging(GridParam input)
        {
            return await _payrollManager.GetAllPaging(input);
        }

        [HttpGet]
        public List<DateTime> GetListDateFromPayroll()
        {
            return _payrollManager.GetListDateFromPayroll();
        }

        [HttpGet]
        public List<GetPayrollDto> GetAll()
        {
            return _payrollManager.GetAll();
        }

        [HttpGet]
        public GetPayrollDto Get(long id)
        {
            return _payrollManager.Get(id);
        }

        [HttpPost]
        [AbpAuthorize(PermissionNames.Payroll_Create)]
        public async Task<CreatePayrollDto> Create(CreatePayrollDto input)
        {
            return await _payrollManager.Create(input);
        }

        [HttpPut]
        [AbpAuthorize(PermissionNames.Payroll_Edit)]
        public async Task<UpdatePayrollDto> Update(UpdatePayrollDto input)
        {
            return await _payrollManager.Update(input);
        }

        [AbpAuthorize(PermissionNames.Payroll_ApproveAndSendtToCEO, PermissionNames.Payroll_ApproveByCEO,
           PermissionNames.Payroll_ApproveByKT, PermissionNames.Payroll_SendToAccountant, PermissionNames.Payroll_RejectByKT, PermissionNames.Payroll_RejectByCEO,
           PermissionNames.Payroll_Payslip_ApproveAndSendtToCEO, PermissionNames.Payroll_Payslip_ApproveByCEO, PermissionNames.Payroll_Payslip_ApproveByKT, 
           PermissionNames.Payroll_Payslip_SendToAccountant, PermissionNames.Payroll_Payslip_RejectByKT, PermissionNames.Payroll_Payslip_RejectByCEO)]
        public async Task<string> ChangeStatus(ChangePayrollStatusDto input)
        {
            return await _payrollManager.ChangeStatus(input);
        }

        [HttpDelete]
        [AbpAuthorize(PermissionNames.Payroll_Delete)]
        public async Task<long> Delete(long id)
        {
            return await _payrollManager.Delete(id);
        }

        [HttpPut]
        public string ExecuatePayroll(long payrollId)
        {
            return _payrollManager.ExecuatePayroll(payrollId);
        }

        [HttpPost]
        public void CreateFinfastOutcomeEntry(long payrollId)
        {
           _payrollManager.CreateFinfastOutcomeEntry(payrollId);
        }

        [HttpGet]
        public object ValidFinfastBranch(long payrollId)
        {
           return _payrollManager.ValidCreateFinfastOutcomeEntry(payrollId);
        }
    }
}
