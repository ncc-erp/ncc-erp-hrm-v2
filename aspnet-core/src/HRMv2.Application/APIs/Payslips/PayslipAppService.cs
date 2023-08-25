using Abp.Authorization;
using HRMv2.Manager.Notifications.Email;
using HRMv2.Manager.Notifications.Email.Dto;
using HRMv2.Authorization;
using HRMv2.Manager.Salaries.CalculateSalary;
using HRMv2.Manager.Salaries.Payslips;
using HRMv2.Manager.Salaries.Payslips.Dto;
using Microsoft.AspNetCore.Mvc;
using NccCore.Paging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HRMv2.Constants.Enum.HRMEnum;
using HRMv2.Entities;
using HRMv2.Manager.Employees.Dto;
using HRMv2.NccCore;
using HRMv2.Manager.Punishments.Dto;
using HRMv2.Manager.Punishments;
using HRMv2.Manager.Bonuses.Dto;

namespace HRMv2.APIs.Payslips
{
    [AbpAuthorize]
    public class PayslipAppService : HRMv2AppServiceBase
    {
        private readonly PayslipManager _payslipManager;
        public PayslipAppService(PayslipManager payslipManager)
        {
            _payslipManager = payslipManager;
        }

        [HttpPost]
        public List<GenerateErrorDto> GenerateAllPayslip(CollectPayslipDto input)
        {
            input.CurrentUserLoginId = AbpSession.UserId;
            input.TenantId = AbpSession.TenantId;
            var result = _payslipManager.AddGeneratePayslipsToBackgroundJob(input);
            return result?.ErrorList;
        }

        public CalculatingSalaryInfoDto GetCalculatingSalaryInfoDto()
        {
            var result = _payslipManager.GetCalculatingSalaryInfoDto(AbpSession.TenantId);
            return result;
        }

        [HttpPost]
        [AbpAuthorize(PermissionNames.Payroll_Payslip_View)]
        public async Task<GridResult<GetPayslipEmployeeDto>> GetPayslipEmployeePaging(long payrollId, InputGetPayslipEmployeeDto input)
        {
            return await _payslipManager.GetPayslipEmployeePaging(payrollId, input);
        }

        [HttpGet]
        [AbpAuthorize(PermissionNames.Payroll_Payslip_PayslipDetail_TabSalary_View, PermissionNames.Payroll_Payslip_PayslipDetail_TabPayslipPreview_View)]
        public GetPayslipDetailDto GetPayslipDetail(long id)
        {
            return _payslipManager.GetPayslipDetail(id);
        }

        [HttpGet]
        public List<long> GetEmployeeIdsInPayroll(long id)
        {
            return _payslipManager.GetEmployeeIdsInPayroll(id);
        }

        [HttpGet]
        [AbpAuthorize(PermissionNames.Payroll_Payslip_PayslipDetail_TabSalary_View, PermissionNames.Payroll_Payslip_PayslipDetail_TabPayslipPreview_View)]
        public List<GetSalaryDetailDto> GetPayslipResult(long payslippId)
        {
            return _payslipManager.GetPayslipResult(payslippId);
        }

        [HttpGet]
        public GetPayslipMailContentDto GetEmailTemplate(long payslippId)
        {
            return _payslipManager.GetPayslipMailTemplate(payslippId);
        }

        [HttpPost]
        [AbpAuthorize(PermissionNames.Payroll_Payslip_Add)]
        public GeneratePayslipResultDto AddEmployeesToPayroll(CollectPayslipDto input)
        {            
            return _payslipManager.AddEmployeesToPayroll(input);
        }

        [HttpPost]
        [AbpAuthorize(PermissionNames.Payroll_Payslip_Add)]
        public GeneratePayslipResultDto ReGeneratePayslip(long payslipId)
        {
            return _payslipManager.ReGeneratePayslip(payslipId);            
        }

        [HttpGet]
        [AbpAuthorize(PermissionNames.Payroll_Payslip_CalculateSalary)]
        public void CalculateSalaryForAllPayslip(long payrollId)
        {
            _payslipManager.ReCalculateAllPayslipFromDetail(payrollId);
        }

        [HttpPut]
        [AbpAuthorize(PermissionNames.Payroll_Payslip_PayslipDetail_TabSalary_ReCalculateSalary)]
        public async Task<double> ReCalculate(ReCalculateDto input)
        {
            return await _payslipManager.ReCalculatePayslipFromDetail(input.PayslipId);
        }

        [HttpDelete]
        [AbpAuthorize(PermissionNames.Payroll_Payslip_Delete)]
        public async Task<long> Delete(long id)
        {
            return await _payslipManager.Delete(id);
        }

        [HttpPost]
        public List<string> TestNccCalculator(InpuTestNccCalculator dto)
        {
            return _payslipManager.TestNccCalculator(dto);
        }

        [HttpGet]
        [AbpAuthorize(PermissionNames.Payroll_Payslip_PayslipDetail_TabBenefit_View, PermissionNames.Payroll_Payslip_PayslipDetail_TabBonus_View,
            PermissionNames.Payroll_Payslip_PayslipDetail_TabDebt_View, PermissionNames.Payroll_Payslip_PayslipDetail_TabPunishment_View)]
        public List<GetPayslipDetailByTypeDto> GetPayslipDetailByType(long payslipId, PayslipDetailType type)
        {
            return _payslipManager.GetPayslipDetailByType(payslipId, type);
        }

        [HttpPost]
        [AbpAuthorize(PermissionNames.Payroll_Payslip_PayslipDetail_TabPunishment_Add)]
        public async Task<string> CreatePayslipDetailPunishmentAndCreateEmployeePunishment(CreatePayslipDetailPunishmentDto input)
        {
            return await _payslipManager.CreatePayslipDetailPunishmentAndCreateEmployeePunishment(input);
        }

        [HttpPut]
        [AbpAuthorize(PermissionNames.Payroll_Payslip_PayslipDetail_TabPunishment_Edit)]
        public async Task<string> UpdatePayslipDetailPunishment(UpdatePayslipDetailDto input)
        {
            return await _payslipManager.UpdatePayslipDetailPunishment(input);
        }

        [HttpPost]
        public void SendMailToOneEmployee(SendMailOneemployeeDto input)
        {
            _payslipManager.SendMailToOneEmployee(input);
        }

        [HttpPost]
        public string SendMailToAllEmployee(SendMailAllEmployeeDto input)
        {
            return _payslipManager.SendMailToAllEmployee(input);
        }

        [HttpGet]
        [AbpAuthorize(PermissionNames.Payroll_Payslip_PayslipDetail_TabBonus_View)]
        public List<GetBonusDto> GetAvailableBonuses(long payslipId)
        {
            return _payslipManager.GetAvailableBonuses(payslipId);
        }

        [HttpPost]
        [AbpAuthorize(PermissionNames.Payroll_Payslip_PayslipDetail_TabBonus_Add)]
        public async Task<string> CreatePayslipDetailBonus(CreatePayslipBonusDto input)
        {
            return await _payslipManager.CreatePayslipDetailBonus(input);
        }

        [HttpPut]
        [AbpAuthorize(PermissionNames.Payroll_Payslip_PayslipDetail_TabBonus_Edit)]
        public async Task<string> UpdatePayslipDetailBonus(UpdatePayslipDetailDto input)
        {
            return await _payslipManager.UpdatePayslipDetailBonus(input);
        }

        [HttpPut]
        [AbpAuthorize(PermissionNames.Payroll_Payslip_UpdatePayslipDetail)]
        public async Task<UpdatePayslipInfoDto> UpdatePayslipDetail(UpdatePayslipInfoDto input)
        {
            var updateinput = await _payslipManager.UpdatePayslipDetail(input);
            return updateinput;
        }

        [HttpGet]
        [AbpAuthorize(PermissionNames.Payroll_Payslip_UpdatePayslipDetail)]
        public GetPayslipInfoBeforeUpdateDto GetPayslipBeforeUpdateInfo(long payslipId)
        {
            var updateinput = _payslipManager.GetPayslipBeforeUpdateInfo(payslipId);
            return updateinput;
        }


        [HttpDelete]
        [AbpAuthorize(PermissionNames.Payroll_Payslip_PayslipDetail_TabBonus_Delete , PermissionNames.Payroll_Payslip_PayslipDetail_TabPunishment_Delete)]
        public async Task<long> DeletePayslipDetail(long id)
        {
            return await _payslipManager.DeletePayslipDetail(id);
        }

        [HttpGet]
        public List<SumaryInfoDto> GetSumaryInfomation(long payrollId)
        {
           return _payslipManager.GetSumaryInfomation(payrollId);
        }

        public async Task<UpdateDeadlineDto> UpdatePayslipDeadline(UpdateDeadlineDto input)
        {
           return await _payslipManager.UpdatePayslipDeadline(input);
        }

        [HttpGet]
        [AbpAuthorize(PermissionNames.Payroll_Payslip_ExportTechcombank)]
        public FileBase64Dto ExportTechcombank(long payrollId)
        {
            return  _payslipManager.ExportTechcombank(payrollId);
        }

        [HttpGet]
        [AbpAuthorize(PermissionNames.Payroll_Payslip_ExportOutsideTech)]
        public FileBase64Dto ExportOutsideTech(long payrollId)
        {
            return  _payslipManager.ExportOutsideTech(payrollId);
        }

        [HttpPost]
        [AbpAuthorize(PermissionNames.Payroll_Payslip_ExportPayroll)]
        public async Task<FileBase64Dto> ExportPayroll(long payrollId, InputGetPayslipEmployeeDto input)
        {
            return await _payslipManager.ExportPayroll(payrollId, input);
        }


        [HttpGet]
        [AbpAuthorize(PermissionNames.Payroll_Payslip_ExportPayrollIncludeLastMonth)]
        public FileBase64Dto ExportPayrollIncludeLastMonth(long payrollId)
        {
            return  _payslipManager.ExportPayrollIncludeLastMonth(payrollId);
        }

        [HttpPost]
        [AbpAuthorize(PermissionNames.Payroll_Payslip_UpdateRemainLeaveDaysAfter)]
        public async Task<Object> UpdateEmployeeRemainLeaveDaysAfterCalculatingSalary([FromForm] InputToUpdateRemainLeaveDaysDto input)
        {
            return await _payslipManager.UpdateEmployeeRemainLeaveDaysAfterCalculatingSalary(input);
        }

        [HttpGet]
        [AbpAuthorize(PermissionNames.Payroll_Payslip_View)]
        public async Task<List<GetNotPayslipEmployeeDto>> GetAllPenaltyNotCollected(long payrollId)
        {
            return await _payslipManager.GetAllPenaltyNotCollected(payrollId);
        }

        [HttpGet]
        public Task<List<PunishmentDto>> GetAvailablePunishmentsInMonth(long payslipId)
        {
            return _payslipManager.GetAvailablePunishmentsInMonth(payslipId);
        }

    }
}
