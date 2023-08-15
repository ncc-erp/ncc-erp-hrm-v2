using Abp.Authorization;
using HRMv2.Authorization;
using HRMv2.Entities;
using HRMv2.Manager.Bonuses.Dto;
using HRMv2.Manager.Categories.Bonuss;
using HRMv2.Manager.Employees.Dto;
using HRMv2.Manager.Notifications.Email.Dto;
using HRMv2.Utils;
using Microsoft.AspNetCore.Mvc;
using NccCore.Paging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMv2.APIs.Bonuses
{
    [AbpAuthorize]
    public class BonusAppService : HRMv2AppServiceBase
    {
        private readonly BonusManager _bonusManager;
        public BonusAppService(BonusManager BonusManager)
        {
            _bonusManager = BonusManager;
        }

        [HttpPost]
        [AbpAuthorize(PermissionNames.Bonus_View)]
        public async Task<GridResult<GetAllBonusDto>> GetAllPaging(GridParam input)
        {
            return await _bonusManager.GetAllPaging(input);
        }

        [HttpGet]
        public List<GetEmployeeBasicInfoDto> GetAllEmployeeNotInBonus(long bonusId)
        {
            return _bonusManager.GetAllEmployeeNotInBonus(bonusId);
        }

        [HttpPost]
        [AbpAuthorize(PermissionNames.Bonus_Create)]
        public async Task<BonusDto> Create(BonusDto input)
        {
            return await _bonusManager.Create(input);
        }

        [HttpPut]
        [AbpAuthorize(PermissionNames.Bonus_Edit)]
        public async Task<EditBonusDto> Update(EditBonusDto input)
        {
            return await _bonusManager.Update(input);
        }

        [HttpDelete]
        [AbpAuthorize(PermissionNames.Bonus_View)]
        public async Task<long> Delete(long id)
        {
            return await _bonusManager.Delete(id);
        }

        [HttpGet]
        public async Task<bool> IsBonusHasEmployee(long bonusId)
        {
            return await _bonusManager.IsBonusHasEmployee(bonusId);
        }

        public List<DateTime> GetListDate()
        {
            return _bonusManager.GetListDate();
        }


        [HttpGet]
        public Task<List<DateTime>> GetListMonthFilter()
        {
            return _bonusManager.GetListMonthFilter();
        }

        [HttpPost]
        [AbpAuthorize(PermissionNames.Bonus_Active, PermissionNames.Bonus_Deactive)]
        public async Task<long> ChangeStatus(long id)
        {
            return await _bonusManager.ChangeStatus(id);
        }

        [HttpGet]

        [AbpAuthorize(PermissionNames.Bonus_BonusDetail_TabInformation_View, PermissionNames.Bonus_BonusDetail)]
        public async Task<GetDetailBonusDto> GetBonusDetail(long id)
        {
            return await _bonusManager.GetBonusDetail(id);
        }


        [HttpPost]

        [AbpAuthorize(PermissionNames.Bonus_BonusDetail_TabEmployee_View)]
        public async Task<GridResult<GetBonusEmployeeDto>> GetAllBonusEmployee(long id, GetBonusEmployeeInputDto input)
        {
            return await _bonusManager.GetAllBonusEmployee(id, input);
        }

        [HttpPost]

        [AbpAuthorize(PermissionNames.Bonus_BonusDetail_TabEmployee_QuickAdd)]
        public async Task<AddEmployeeToBonusDto> QuickAddEmployeeToBonus(AddEmployeeToBonusDto input)
        {
            return await _bonusManager.QuickAddEmployeeToBonus(input);
        }

        [HttpPost]

        [AbpAuthorize(PermissionNames.Bonus_BonusDetail_TabEmployee_Add)]
        public async Task<AddEmployeeToBonusDto> MultipleAddEmployeeToBonus(AddEmployeeToBonusDto input)
        {
            return await _bonusManager.MultipleAddEmployeeToBonus(input);
        }

        [HttpPut]

        [AbpAuthorize(PermissionNames.Bonus_BonusDetail_TabEmployee_Edit)]
        public async Task<EditEmployeeToBonusDto> UpdateEmployeeInBonus(EditEmployeeToBonusDto input)
        {
            return await _bonusManager.UpdateEmployeeInBonus(input);
        }

        [HttpDelete]

        [AbpAuthorize(PermissionNames.Bonus_BonusDetail_TabEmployee_Delete)]
        public async Task<long> DeleteEmployeeFromBonus(long id, long bonusId)
        {
            return await _bonusManager.DeleteEmployeeFromBonus(id, bonusId);
        }


        [HttpGet]
        public async Task<List<long>> GetAllEmployeeInBonus(long bonusId)
        {
            return await _bonusManager.GetAllEmployeeInBonus(bonusId);
        }

        [HttpPost]
        [AbpAuthorize(PermissionNames.Employee_EmployeeDetail_TabBonus_View)]
        public async Task<GridResult<GetBonusesOfEmployeeDto>> GetAllPagingBonusesByEmployeeId(long employeeId, GridParam input)
        {
            return await _bonusManager.GetAllPagingBonusesByEmployeeId(employeeId, input);
        }

        [HttpGet]
        public Task<List<DateTime>> GetListMonthFilterOfEmployee(long employeeId)
        {
            return _bonusManager.GetListMonthFilterOfEmployee(employeeId);
        }

        [HttpPost]

        [AbpAuthorize(PermissionNames.Bonus_BonusDetail_TabEmployee_Import)]
        public async Task<Object> ImportEmployeeToBonus([FromForm] ImportFileDto input)
        {
            return await _bonusManager.ImportEmployeeToBonus(input);
        }

        [HttpGet]
        public List<GetBonusDto> GetAll()
        {
            return _bonusManager.GetAll();
        }

        [HttpPost]
        public async Task<AddBonusForEmployeeDto> AddBonusForEmployee(AddBonusForEmployeeDto input)
        {
            return await _bonusManager.AddBonusForEmployee(input);
        }

        [HttpGet]
        public MailPreviewInfoDto GetBonusTemplate(long bonusEmployeeId)
        {
            return _bonusManager.GetBonusTemplate(bonusEmployeeId);
        }

        [HttpPost]
        public void SendMailToOneEmployee(SendMailBonusDto input)
        {
             _bonusManager.SendMailToOneEmployee(input);
        }

        [HttpPost]
        public string SendMailToAllEmployee(long id, GetBonusEmployeeInputDto input)
        {
            return _bonusManager.SendMailToAllEmployee(id, input);
        }

    }
}
