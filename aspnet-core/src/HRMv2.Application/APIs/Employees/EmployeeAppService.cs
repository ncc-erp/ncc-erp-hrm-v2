using Abp.Authorization;
using HRMv2.Authorization;
using HRMv2.Manager.Employees;
using HRMv2.Manager.Employees.Dto;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NccCore.Paging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMv2.APIs.Employees
{
    [AbpAuthorize]
    public class EmployeeAppService : HRMv2AppServiceBase
    {
        private readonly EmployeeManager _employeeManager;
        public EmployeeAppService(EmployeeManager employeeManager)
        {
            _employeeManager = employeeManager;
        }

        [HttpGet]
        public List<GetEmployeeDto> GetAll()
        {
            return _employeeManager.GetAll();
        }

        [HttpGet]
        [AbpAuthorize(PermissionNames.Employee_EmployeeDetail_TabPersonalInfo_View, PermissionNames.SalaryChangeRequest_SalaryChangeRequestDetail)]
        public GetEmployeeInfoDto Get(long id)
        {
            return _employeeManager.Get(id);
        }
               

        [HttpPost]
        [AbpAuthorize(PermissionNames.Employee_View)]
        public async Task<GridResult<GetEmployeeDto>> GetEmployeeExcept(GetEmployeeToAddDto input)
        {
            return await _employeeManager.GetEmployeeExcept(input);
        }


        [HttpPost]
        [AbpAuthorize(PermissionNames.Employee_Create)]
        public async Task<CreateUpdateEmployeeDto> Create(CreateUpdateEmployeeDto input, long? tempEmployeeId)
        {
            return await _employeeManager.CreateEmployee(input, tempEmployeeId, true);
        }

        [HttpPut]
        [AbpAuthorize(PermissionNames.Employee_EmployeeDetail_TabPersonalInfo_Edit, PermissionNames.Employee_Edit)]
        public async Task<CreateUpdateEmployeeDto> Update(CreateUpdateEmployeeDto input)
        {
            return await _employeeManager.Update(input);
        }

        [HttpDelete]
        [AbpAuthorize(PermissionNames.Employee_Delete)]
        public async Task<long> Delete(long id)
        {
            return await _employeeManager.Delete(id);
        }


        [HttpPost]
        [AbpAuthorize(PermissionNames.Employee_UploadAvatar, PermissionNames.Employee_EmployeeDetail_TabPersonalInfo_UploadAvatar)]
        public async Task<string> UploadAvatar([FromForm] AvatarDto input)
        {
            return await _employeeManager.UploadAvatar(input);
        }

        [HttpPost]
        [AbpAuthorize(PermissionNames.Employee_EmployeeDetail_TabPersonalInfo_ChangeBranch)]
        public void ChangeEmployeeBranch(ChangeBranchDto input)
        {
            _employeeManager.ChangeEmployeeBranch(input);
        }

        [HttpPost]
        [AbpAuthorize(PermissionNames.Employee_Export)]
        public async Task<FileBase64Dto> ExportEmployee(GetEmployeeToAddDto input)
        {
            return await _employeeManager.ExportEmployee(input);
        }
        [HttpGet]
        public async Task<FileBase64Dto> GetDataMetaToCreateEmployeeByFile() { 
            return await _employeeManager.GetDataMetaToCreateEmployeeByFile();
        }

        [HttpGet]
        public async Task<FileBase64Dto> GetDataMetaToUpdateEmployeeByFile()
        {
            return await _employeeManager.GetDataMetaToUpdateEmployeeByFile();
        }

        [HttpPost]
        public async Task<Object> CreateEmployeeFromFile([FromForm] InputFileDto input)
        {
            return await _employeeManager.CreateEmployeeFromFile(input);
        }
        [HttpPost]
        public async Task<Object> UpdateEmployeeFromFile([FromForm] InputFileDto input)
        {
            return await _employeeManager.UpdateEmployeeFromFile(input);
        }

        [HttpGet]
        public List<GetEmployeeBasicInfoDto> GetAllEmployeeBasicInfo()
        {
            return _employeeManager.GetAllEmployeeBasicInfo();
        }

        [HttpGet]
        public GetEmployeeBasicInfoForBreadcrumbDto GetEmployeeBasicInfoForBreadcrumb(long employeeId)
        {
            return _employeeManager.GetEmployeeBasicInfoForBreadcrumb(employeeId);
        }

        [HttpGet]
        public void ReCreateEmployeeToOtherTool(long employeeId)
        {
            _employeeManager.ReCreateEmployeeToOtherTool(employeeId);
        }

        [HttpPost]
        public async Task<FileBase64Dto> ExportEmployeeStatistic(InputExportEmployeeStatisticDto input)
        {
           return await _employeeManager.ExportEmployeeStatistic(input);
        }

        [HttpPost]
        public void QuitJobToOtherTool(EmployeeIdDto input)
        {
            _employeeManager.QuitJobToOtherTool(input);
        }

        [HttpPost]
        public void UpdateEmployeeInfoToOtherTool(EmployeeIdDto input)
        {
            _employeeManager.UpdateEmployeeInfoToOtherTool(input);
        }
        [HttpPost]
        [AbpAuthorize(PermissionNames.Employee_SyncUpdateEmployeesInforToOtherTools)]
        public void UpdateAllWorkingEmployeeInfoToOtherTools()
        {
            _employeeManager.UpdateAllWorkingEmployeeInfoToOtherTools();
        }
    }
}
