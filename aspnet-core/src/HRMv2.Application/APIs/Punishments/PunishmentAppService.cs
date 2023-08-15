using Abp.Authorization;
using HRMv2.Authorization;
using HRMv2.Manager.Employees.Dto;
using HRMv2.Manager.Punishments;
using HRMv2.Manager.Punishments.Dto;
using Microsoft.AspNetCore.Mvc;
using NccCore.Paging;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMv2.APIs.Punishments
{
    [AbpAuthorize]
    public class PunishmentAppService : HRMv2AppServiceBase
    {
        private readonly PunishmentManager _punishmentManager;

        public PunishmentAppService( PunishmentManager punishmentManager)
        {
            _punishmentManager = punishmentManager;
          
        }

        [HttpPost]

        [AbpAuthorize(PermissionNames.Punishment)]
        public async Task<GridResult<GetPunishmentDto>> GetAllPaging(GridParam input)
        {
            return await _punishmentManager.GetAllPaging(input);
        }

        [HttpGet]
        public List<PunishmentDto> GetAll()
        {
            return _punishmentManager.GetAll();
        }
        [HttpGet]
        public PunishmentDto GetPunishmentById(long id)
        {
            return _punishmentManager.GetPunishmentById(id);
        }

        [HttpPost]
        [AbpAuthorize(PermissionNames.Employee_EmployeeDetail_TabPunishment_View)]
        public async Task<GridResult<GetPunishmentsOfEmployeeDto>> GetPunishmentByEmployeeId(long id, GridParam input)
        {
            return await _punishmentManager.GetPunishmentByEmployeeId(id, input);
        }

        [HttpPost]

        [AbpAuthorize(PermissionNames.Punishment_Create)]
        public async Task<CreatePunishmentDto> Create(CreatePunishmentDto input)
        {
            return await _punishmentManager.Create(input);
        }

        [HttpPut]

        [AbpAuthorize(PermissionNames.Punishment_Edit)]
        public async Task<UpdatePunishmentDto> Update(UpdatePunishmentDto input)
        {
            return await _punishmentManager.Update(input);
        }

        [HttpDelete]

        [AbpAuthorize(PermissionNames.Punishment_Delete)]
        public async Task<long> Delete(long id)
        {
            return await _punishmentManager.Delete(id);
        }

        [HttpGet]
        public async Task<bool> IsPunishmentHasEmployee(long punishId)
        {
            return await _punishmentManager.IsPunishmentHasEmployee(punishId);
        }

        [HttpPost]
        [AbpAuthorize(PermissionNames.Punishment_Active, PermissionNames.Punishment_Deactive)]
        public async Task<long> ChangeStatus(long id)
        {
            return await _punishmentManager.ChangeStatus(id);
        }
        [HttpPost]

        [AbpAuthorize(PermissionNames.Punishment_PunishmentDetail_View)]
        public async Task<GridResult<GetPunishmentDetailDto>> GetAllEmployeeInPunishment(long id, GetEmployeePunishment input)
        {
            return await _punishmentManager.GetAllEmployeeInPunishment(id, input);
        }
        [HttpPost]

        [AbpAuthorize(PermissionNames.Punishment_PunishmentDetail_AddEmployee)]
        public async Task<AddEmployeeToPunishmentDto> AddEmployeeToPunishment(AddEmployeeToPunishmentDto input)
        {
            return await _punishmentManager.AddEmployeeToPunishment(input);
        }

        [HttpPut]

        [AbpAuthorize(PermissionNames.Punishment_PunishmentDetail_Edit)]
        public async Task<UpdateEmployeeInPunishmentDto> UpdateEmployeeInPunishment(UpdateEmployeeInPunishmentDto input)
        {
            return await _punishmentManager.UpdateEmployeeInPunishment(input);
        }

        [HttpPut]
        public async Task<UpdateEmployeeInPunishmentDto> UpdatePunishmentOfEmployee(UpdateEmployeeInPunishmentDto input)
        {
            return await _punishmentManager.UpdatePunishmentOfEmployee(input);
        }

        [HttpDelete]
        [AbpAuthorize(PermissionNames.Punishment_PunishmentDetail_Delete)]
        public async Task<long> DeleteEmployeeFromPunishment(long id)
        {
            return await _punishmentManager.DeleteEmployeeFromPunishment(id);
        }

        [HttpGet]
        public List<GetEmployeeBasicInfoDto> GetAllEmployeeNotInPunishment(long punishmentId)
        {
            return _punishmentManager.GetAllEmployeeNotInPunishment(punishmentId);
        }

        [HttpGet]
        public List<DateTime> GetListDate()
        {
            return _punishmentManager.GetListDate();
        }

        [HttpGet]
        public List<DateTime> GetDateFromPunishments()
        {
            return _punishmentManager.GetDateFromPunishments();
        }

        [HttpGet]
        public List<DateTime> GetDateFromPunishmentsOfEmployee(long id)
        {
            return _punishmentManager.GetDateFromPunishmentsOfEmployee(id);
        }

        [HttpPost]
        [AbpAuthorize(PermissionNames.Punishment_PunishmentDetail_Import)]
        public Object ImportEmployeePunishmentsFromFile([FromForm] ImportFileDto input)
        {
            return  _punishmentManager.ImportEmployeePunishmentsFromFile(input);
        }

        [HttpPost]
        [AbpAuthorize(PermissionNames.Punishment_Generate)]
        public async Task<object> GeneratePunishmentsByPunishmentType(GeneratePunishmentDto input)
        {
            return await this._punishmentManager.GeneratePunishmentsByPunishmentType(input);
        }

        

    }
    
}
