using Abp.Authorization;
using HRMv2.Authorization;
using HRMv2.Manager.EmployeeContracts;
using HRMv2.Manager.EmployeeContracts.Dto;
using HRMv2.Manager.Notifications.Email.Dto;
using Microsoft.AspNetCore.Mvc;
using NccCore.Paging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HRMv2.Constants.Enum.HRMEnum;

namespace HRMv2.APIs.EmployeeContracts
{
    [AbpAuthorize]
    public class EmployeeContractAppService : HRMv2AppServiceBase
    {
        private readonly ContractManager _contracManager;

        public EmployeeContractAppService(ContractManager contracManager)
        {
            _contracManager = contracManager;
        }

        [HttpGet]
        public List<EmployeeContractDto> GetAll()
        {
            return _contracManager.GetAll();
        }

        public EmployeeContractDto GetContractBySalaryRequest(long requestEmployeeId)
        {
            return _contracManager.GetContractBySalaryRequest(requestEmployeeId);
        }


        [HttpPost]
        [AbpAuthorize(PermissionNames.Employee_EmployeeDetail_TabContract_View)]
        public async Task<GridResult<EmployeeContractDto>> GetAllPaging(GridParam input)
        {
            return await _contracManager.GetAllPaging(input);
        }

        [HttpPut]
        [AbpAuthorize(PermissionNames.Employee_EmployeeDetail_TabContract_EditNote)]
        public async Task<UpdateContractNoteDto> UpdateNote(UpdateContractNoteDto input)
        {
            return await _contracManager.UpdateNote(input);
        }
        [HttpPost]
        [AbpAuthorize(PermissionNames.Employee_EmployeeDetail_TabContract_ImportContractFile)]
        public async Task<string> UploadContractFile([FromForm] ContractFileDto input)
        {
            return await _contracManager.UploadContractFile(input);
        }

        [HttpPost]
        [AbpAuthorize(PermissionNames.Employee_EmployeeDetail_TabContract_DeleteContractFile)]
        public async Task<long> DeleteContractFile(long id)
        {
            return await _contracManager.DeleteContractFile(id);
        }

        [HttpDelete]
        [AbpAuthorize(PermissionNames.Employee_EmployeeDetail_TabContract_Delete)]
        public async Task DeleteContract(long id)
        {
            await _contracManager.DeleteContract(id);
        }

        [HttpPut]
        public async Task<UpdateContractDto> UpdateEmployeeContract(UpdateContractDto input)
        {
            return await _contracManager.UpdateEmployeeContract(input);
        }

        [HttpGet]
        public MailPreviewInfoDto GetContractTemplate(long contractId, MailFuncEnum type)
        {
            return _contracManager.GetContractTemplate(contractId, type);
        }

    }
}
