using Abp.Authorization;
using HRMv2.Authorization;
using HRMv2.Entities;
using HRMv2.Manager.Benefits.Dto;
using HRMv2.Manager.Categories.Benefits;
using HRMv2.Manager.Categories.Benefits.Dto;
using HRMv2.Manager.Employees.Dto;
using Microsoft.AspNetCore.Mvc;
using NccCore.Paging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMv2.APIs.Benefits
{
    [AbpAuthorize]
    public class BenefitAppService : HRMv2AppServiceBase
    {
        private readonly BenefitManager _benefitManager;
        public BenefitAppService(BenefitManager benefitManager)
        {
            _benefitManager = benefitManager;
        }

        [HttpPost]

        [AbpAuthorize(PermissionNames.Benefit_View)]
        public async Task<GridResult<GetBenefitDto>> GetAllPaging(GridParam input)
        {
            return await _benefitManager.GetAllPaging(input);
        }

        [HttpPost]
        [AbpAuthorize(PermissionNames.Benefit_BenefitDetail_TabEmployee_View)]
        public async Task<GridResult<GetbenefitEmployeeDto>> GetEmployeeInBenefit(long benefitId, GetbenefitEmployeeInputDto input)
        {
            return await _benefitManager.GetEmployeeInBenefitPaging(benefitId, input);
        }

        [HttpGet]
        public GetBenefitDto Get(long id)
        {
            return _benefitManager.Get(id);
        }
        [HttpGet]
        public List<GetBenefitDto> GetAll()
        {
            return _benefitManager.GetAll();
        }
        [HttpGet]
        public async Task<List<DateTime>> GetListMonthFilter()
        {
            return await _benefitManager.GetListMonthFilter();
        }

        [HttpGet]
        public List<long> GetListEmployeeIdInBenefit(long benefitId)
        {
            return _benefitManager.GetListEmployeeIdInBenefit(benefitId);
        }

        [HttpGet]

        public List<GetEmployeeDto> GetAllEmployeeNotInBenefit(long benefitId)
        {
            return _benefitManager.GetAllEmployeeNotInBenefit(benefitId);
        }

        [HttpPost]
        [AbpAuthorize(PermissionNames.Employee_EmployeeDetail_TabBenefit_View)]
        public async Task<GridResult<GetBenefitsOfEmployeeDto>> GetBenefitByEmployeeId(long id, GridParam input)

        {
            return await _benefitManager.GetBenefitByEmployeeId(id, input);
        }

        [HttpPost]
        [AbpAuthorize(PermissionNames.Benefit_Create)]
        public async Task<BenefitDto> Create(BenefitDto input)
        {
            return await _benefitManager.Create(input);
        }

        [HttpPost]
        [AbpAuthorize(PermissionNames.Benefit_BenefitDetail_TabEmployee_Add)]
        public async Task<AddEmployeeToBenefitDto> AddEmployeeToBenefit(AddEmployeeToBenefitDto input)
        {
            return await _benefitManager.AddEmployeeToBenefit(input);
        }

        [HttpPost]
        [AbpAuthorize(PermissionNames.Benefit_BenefitDetail_TabEmployee_QuickAdd, PermissionNames.Employee_EmployeeDetail_TabBenefit_Add)]
        public async Task<QuickAddEmployeeDto> QuickAddEmployee(QuickAddEmployeeDto input)
        {
            return await _benefitManager.QuickAddEmployee(input);
        }

        [HttpPost]
        [AbpAuthorize(PermissionNames.Benefit_BenefitDetail_TabInformation_Clone)]
        public async Task CloneBenefit(CloneBenefitDto input)
        {
            await _benefitManager.CloneBenefit(input);
        }

        [HttpPut]
        [AbpAuthorize(PermissionNames.Benefit_BenefitDetail_TabEmployee_UpdateAllStartDate)]
        public UpdateEmployeeDateDto UpdateAllStartDate(UpdateEmployeeDateDto input)
        {
            return _benefitManager.UpdateAllStartDate(input);
        }

        [HttpPut]
        [AbpAuthorize(PermissionNames.Benefit_BenefitDetail_TabEmployee_UpdateAllEndDate)]
        public UpdateEmployeeEndDateDto UpdateAllEndDate(UpdateEmployeeEndDateDto input)
        {
            return _benefitManager.UpdateAllEndDate(input);
        }

        [HttpPut]
        [AbpAuthorize(PermissionNames.Benefit_Edit)]
        public async Task<BenefitDto> Update(BenefitDto input)
        {
            return await _benefitManager.Update(input);
        }

        [HttpPut]
        [AbpAuthorize(PermissionNames.Benefit_BenefitDetail_TabEmployee_Edit, PermissionNames.Employee_EmployeeDetail_TabBenefit_Edit)]
        public async Task<UpdateBEDto> UpdateBenefitEmployee(UpdateBEDto input)
        {
            return await _benefitManager.UpdateBenefitEmployee(input);
        }

        [HttpPut]
        [AbpAuthorize(PermissionNames.Benefit_Active, PermissionNames.Benefit_Deactive)]
        public async Task<Benefit> UpdateStatus(UpdateBenefitStatusDto input)
        {
            return await _benefitManager.ChangeStatus(input);
        }

        [HttpDelete]
        [AbpAuthorize(PermissionNames.Benefit_Delete)]
        public async Task<long> Delete(long id)
        {
            return await _benefitManager.Delete(id);
        }

        [HttpDelete]
        [AbpAuthorize(PermissionNames.Benefit_BenefitDetail_TabEmployee_Delete, PermissionNames.Employee_EmployeeDetail_TabBenefit_Delete)]
        public async Task<long> RemoveEmployeeFromBenefit(long id)
        {
            return await _benefitManager.RemoveEmployeeFromBenefit(id);
        }

        [HttpGet]
        public List<GetBenefitsOfEmployeeDto> GetAllBenefitsByEmployeeId(long id)
        {
            return _benefitManager.GetAllBenefitsByEmployeeId(id);
        }



    }
}
