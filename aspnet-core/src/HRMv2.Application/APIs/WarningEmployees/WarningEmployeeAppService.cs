using Abp.Authorization;
using HRMv2.Authorization;
using HRMv2.Manager.Employees.Dto;
using HRMv2.Manager.WarningEmployees;
using HRMv2.Manager.WarningEmployees.Dto;
using Microsoft.AspNetCore.Mvc;
using NccCore.Paging;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HRMv2.APIs.WarningEmployees
{
    [AbpAuthorize(PermissionNames.WarningEmployee)]
    public class WarningEmployeeAppService: HRMv2AppServiceBase
    {
        public readonly WarningEmployeeManager _warningEmployeeManager;

        public WarningEmployeeAppService(WarningEmployeeManager warningEmployeeManager)
        {
            _warningEmployeeManager = warningEmployeeManager;
        }

        [HttpPost]
        [AbpAuthorize(PermissionNames.WarningEmployee_ContractExpired_View)]
        public async Task<GridResult<GetEmployeeContractDto>> GetAllEmployeesToUpdateContract(InputMultiFilterEmployeePagingDto input)
        {
            return await _warningEmployeeManager.GetAllEmployeesToUpdateContract(input);
        }

        [HttpPost]
        [AbpAuthorize(PermissionNames.WarningEmployee_BackToWork_View)]
        public async Task<GridResult<GetEmployeeBackToWork>> GetAllEmployeesBackToWork(InputMultiFilterEmployeePagingDto input)
        {
            return await _warningEmployeeManager.GetAllEmployeesBackToWork(input);
        }

        [HttpPut]
        public async Task<UpdateEmployeeBackdateDto> UpdateEmployeeBackDate(UpdateEmployeeBackdateDto input)
        {
            return await _warningEmployeeManager.UpdateEmployeeBackDate(input);
        }

        [HttpPost]
        public async Task<GridResult<GetTempEmployeeTalentDto>> GetTempEmployeeTalentPaging(InputGetTemEmployeeTalentDto input)
        {
            var hasPermissionViewSalary = IsGranted(PermissionNames.WarningEmployee_PlanOnboard_ViewSalary);
            return await _warningEmployeeManager.GetTempEmployeeTalentPaging(input, hasPermissionViewSalary);
        }

        [HttpGet]
        public GetTempEmployeeTalentDto GetTempEmployeeTalentById(long id)
        {
            return _warningEmployeeManager.GetTempEmployeeTalentById(id);
        }

        [HttpPut]
        [AbpAuthorize(PermissionNames.WarningEmployee_PlanOnboard_Edit)]
        public async Task<UpdateTempEmployeeTalentDto> UpdateTempEmployeeTalent(UpdateTempEmployeeTalentDto input)
        {
            return await _warningEmployeeManager.UpdateTempEmployeeTalent(input);
        }

        public async Task<long> DeleteTempEmployeeTalent(long id)
        {
            return await _warningEmployeeManager.DeleteTempEmployeeTalent(id);
        }

        [HttpPost]
        public async Task<GridResult<GetRequestUpdateInfoDto>> GetRequestUpdateInfo(InputMultiFilterRequestDto input)
        {
            return await _warningEmployeeManager.GetRequestUpdateInfo(input);
        }

        [HttpPut]
        public async Task ApproveRequestUpdateInfo(ApproveChangeInfoDto input)
        {
             await _warningEmployeeManager.ApproveRequestUpdateInfo(input);
        }

        [HttpPut]
        public async Task RejectRequestUpdateInfo(RejectChangeInfoDto input)
        {
            await _warningEmployeeManager.RejectRequestUpdateInfo(input);
        }

        [HttpGet]
        public GetRequestDetailDto GetRequestDetailById(long id)
        {
            return _warningEmployeeManager.GetRequestDetailById(id);
        }

        public List<GetPlanQuitEmployeeDto> GetPlanQuitEmployee()
        {
            return _warningEmployeeManager.GetPlanQuitEmployee();
        }
        [AbpAuthorize(PermissionNames.WarningEmployee_PlanQuitEmployee_Detele)]
        public async Task<long> DeletePlanQuitBgJob(long id)
        {
            return await _warningEmployeeManager.DeletePlanQuitBgJob(id);
        }
        [AbpAuthorize(PermissionNames.WarningEmployee_PlanQuitEmployee_Edit)]
        public async Task<UpdatePlanToQuitEmployeeDto> UpdatePlanQuitBgJob(UpdatePlanToQuitEmployeeDto input)
        {
            return await _warningEmployeeManager.UpdatePlanQuitBgJob(input);
        }
    }
}
