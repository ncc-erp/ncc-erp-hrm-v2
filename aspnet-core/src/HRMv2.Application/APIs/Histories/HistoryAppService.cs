using Abp.Authorization;
using HRMv2.Authorization;
using HRMv2.Manager.Histories;
using HRMv2.Manager.Histories.Dto;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMv2.APIs.Histories
{
    [AbpAuthorize]
    public  class HistoryAppService : HRMv2AppServiceBase
    {
        private readonly HistoryManager _historyManager;
        public HistoryAppService(HistoryManager historyManager)
        {
            _historyManager = historyManager;
        }

        [HttpGet]
        [AbpAuthorize(PermissionNames.Employee_EmployeeDetail_TabBranchHistory_View)]
        public List<EmployeeBranchHistoryDto> GetAllEmployeeBranchHistory(long employeeId)
        {
            return _historyManager.GetAllEmployeeBranchHistory(employeeId);
        }

        [HttpGet]
        [AbpAuthorize(PermissionNames.Employee_EmployeeDetail_TabWorkingHistory_View)]
        public List<EmployeeWorkingHistoryDto> GetAllEmployeeWorkingHistory(long employeeId)
        {
            return _historyManager.GetAllEmployeeWorkingHistory(employeeId);
        }

        [HttpGet]
        [AbpAuthorize(PermissionNames.Employee_EmployeeDetail_TabSalaryHistory_View)]
        public List<EmployeeSalaryHistoryDto> GetAllEmployeeSalaryHistory(long employeeId)
        {
            return _historyManager.GetAllEmployeeSalaryHistory(employeeId);
        }

        [HttpGet]
        [AbpAuthorize(PermissionNames.Employee_EmployeeDetail_TabPayslipHistory_View)]
        public List<EmployeePayslipHistoryDto> GetAllEmployeePayslipHistory(long employeeId)
        {
            return _historyManager.GetAllEmployeePayslipHistory(employeeId);
        } 
        
        [HttpDelete]
        [AbpAuthorize(PermissionNames.Employee_EmployeeDetail_TabBranchHistory_Delete)]
        public async Task<long> DeleteBranchHistory(long id)
        {
            return await _historyManager.DeleteBranchHistory(id);
        }

        [HttpDelete]
        [AbpAuthorize(PermissionNames.Employee_EmployeeDetail_TabWorkingHistory_Delete)]
        public async Task<long> DeleteWorkingHistory(long id , long employeeId)
        {
            return await _historyManager.DeleteWorkingHistory(id, employeeId);
        }

        [HttpDelete]
        [AbpAuthorize(PermissionNames.Employee_EmployeeDetail_TabSalaryHistory_Delete)]
        public async Task<long> DeleteSalaryHistory(long id)
        {
            return await _historyManager.DeleteSalaryHistory(id);
        }

        [HttpPut]
        [AbpAuthorize(PermissionNames.Employee_EmployeeDetail_TabBranchHistory_EditNote)]
        public async Task<UpdateNoteBranchHistoryDto> UpdateNoteInBranchHistory(UpdateNoteBranchHistoryDto input)
        {
            return await _historyManager.UpdateNoteInBranchHistory(input);
        }

        [HttpPut]
        [AbpAuthorize(PermissionNames.Employee_EmployeeDetail_TabWorkingHistory_EditNote)]
        public async Task<UpdateNoteWorkingHistoryDto> UpdateNoteInWorkingHistory(UpdateNoteWorkingHistoryDto input)
        {
            return await _historyManager.UpdateNoteInWorkingHistory(input); 
        }

        [HttpPut]
        [AbpAuthorize(PermissionNames.Employee_EmployeeDetail_TabSalaryHistory_EditNote)]
        public async Task<UpdateNoteWorkingHistoryDto> UpdateNoteInSalaryHistory(UpdateNoteWorkingHistoryDto input)
        {
            return await _historyManager.UpdateNoteInSalaryHistory(input);
        }
        [HttpPut]
        [AbpAuthorize(PermissionNames.Employee_EmployeeDetail_TabWorkingHistory_EditDate)]
        public async Task<UpdateDateWorkingHistoryDto> UpdateDateInWorkingHistory(UpdateDateWorkingHistoryDto input)
        {
            return await _historyManager.UpdateDateInWorkingHistory(input);
        }
    }
}
