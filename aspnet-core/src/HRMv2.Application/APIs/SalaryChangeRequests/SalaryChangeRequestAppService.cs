using Abp.Authorization;
using HRMv2.Authorization;
using HRMv2.Manager.Employees.Dto;
using HRMv2.Manager.Notifications.Email.Dto;
using HRMv2.Manager.SalaryRequestEmployees.Dto;
using HRMv2.Manager.SalaryRequests;
using HRMv2.Manager.SalaryRequests.Dto;
using Microsoft.AspNetCore.Mvc;
using NccCore.Paging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMv2.APIs.SalaryChangeRequests
{
    [AbpAuthorize]
    public class SalaryChangeRequestAppService : HRMv2AppServiceBase
    {
        private readonly SalaryRequestManager _salaryRequestManager;

        public SalaryChangeRequestAppService(SalaryRequestManager salaryRequestManager)
        {
            _salaryRequestManager = salaryRequestManager;
        }

        [HttpGet]
        public List<GetSalaryRequestDto> GetAll()
        {
            return _salaryRequestManager.GetAll();
        }

        [HttpGet]
        public GetSalaryRequestDto Get(long id)
        {
            return _salaryRequestManager.Get(id);
        }

        [HttpPost]
        [AbpAuthorize(PermissionNames.SalaryChangeRequest_SalaryChangeRequestDetail_View)]
        public async Task<GridResult<GetRequestEmployeeDto>> GetEmployeesInSalaryRequest(long requestId, InputGetEmployeeInSalaryRequestDto input)
        {
            return await _salaryRequestManager.GetEmployeesInSalaryRequest(requestId, input);
        }

        [HttpGet]
        public List<DateTime> GetListDateFromSalaryRequest()
        {
            return _salaryRequestManager.GetListDateFromSalaryRequest();
        }

        [HttpGet]
        public GetRequestEmployeeDto GetRequestEmployeeById(long id)
        {
            return _salaryRequestManager.GetRequestEmployeeById(id);
        }

        [HttpGet]
        public List<GetEmployeeNotInRequestDto> GetEmployeeNotInRequest(long requestId)
        {
            return _salaryRequestManager.GetEmployeeNotInRequest(requestId);
        }

        [HttpPost]
        [AbpAuthorize(PermissionNames.SalaryChangeRequest_SalaryChangeRequestDetail_Add)]
        public async Task<long> AddEmployeeTosalaryRequest(AddOrUpdateEmployeeRequestDto input)
        {
            return await _salaryRequestManager.AddEmployeeTosalaryRequest(input);
        }

        [HttpPost]

        [AbpAuthorize(PermissionNames.SalaryChangeRequest)]
        public async Task<GridResult<GetSalaryRequestDto>> GetAllPaging(GridParam input)
        {
            return await _salaryRequestManager.GetAllPaging(input);
        }

        [HttpPost]
        [AbpAuthorize(PermissionNames.SalaryChangeRequest_Create)]
        public CreateSalaryRequestDto Create(CreateSalaryRequestDto input)
        {
            return _salaryRequestManager.Create(input);
        }

        public async Task UpdateRequestStatus(UpdateChangeRequestDto input)
        {
            await _salaryRequestManager.UpdateRequestStatus(input);
        }

        [HttpPut]
        [AbpAuthorize(PermissionNames.SalaryChangeRequest_Edit)]
        public async Task<UpdateSalaryRequestDto> Update(UpdateSalaryRequestDto input)
        {
            return await _salaryRequestManager.Update(input);
        }

        [HttpPut]
        [AbpAuthorize(PermissionNames.SalaryChangeRequest_SalaryChangeRequestDetail_SalaryChangeRequestEmployeeDetail_Edit)]
        public async Task<AddOrUpdateEmployeeRequestDto> UpdateSalaryRequestemployee(AddOrUpdateEmployeeRequestDto input)
        {
            return await _salaryRequestManager.UpdateSalaryRequestEmployee(input);
        }

        [HttpDelete]
        [AbpAuthorize(PermissionNames.SalaryChangeRequest_Delete)]
        public long Delete(long id)
        {
            return _salaryRequestManager.Delete(id);
        }

        [HttpDelete]
        [AbpAuthorize(PermissionNames.SalaryChangeRequest_SalaryChangeRequestDetail_Delete)]
        public async Task DeleteSalaryRequestEmployee(long id)
        {
            await _salaryRequestManager.DeleteSalaryRequestEmployee(id);
        }

        [HttpPut]
        public async Task<UpdateRequestEmployeeInfoDto> UpdateRequestEmployeeInfo(UpdateRequestEmployeeInfoDto input)
        {
            return await _salaryRequestManager.UpdateRequestEmployeeInfo(input);
        }

        [HttpPost]
        public async Task<Object> ImportCheckpoint([FromForm] ImportCheckpointDto input)
        {
            return await _salaryRequestManager.ImportCheckpoint(input);
        }

        [HttpGet]
        public MailPreviewInfoDto GetCheckpointTemplate(long requestId)
        {
            return _salaryRequestManager.GetCheckpointTemplate(requestId);
        }
        [HttpPost]
        public string SendMailToAllEmployee(long id, InputGetEmployeeInSalaryRequestDto input)
        {
            return _salaryRequestManager.SendMailToAllEmployee(id, input);
        }
        [HttpPost]
        public void SendMailToOneEmployee(SendMailCheckpointDto input)
        {
            _salaryRequestManager.SendMailToOneEmployee(input);
        }
        [HttpGet]
        public async Task<FileBase64Dto> GetTemplateToImportCheckpoint()
        {
            return await _salaryRequestManager.GetTemplateToImportCheckpoint();
        }

    }
}
