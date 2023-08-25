using Abp.Authorization;
using HRMv2.Manager.Employees.Dto;
using HRMv2.Manager.Punishments.Dto;
using HRMv2.Manager.Refunds;
using HRMv2.Manager.Refunds.Dto;
using Microsoft.AspNetCore.Mvc;
using NccCore.Paging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMv2.APIs.Refunds
{
    [AbpAuthorize]
    public class RefundAppService : HRMv2AppServiceBase
    {
        private readonly RefundManager _refundManager;

        public RefundAppService(RefundManager refundManager)
        {
            _refundManager = refundManager;
        }

        [HttpGet]
        public List<GetRefundDto> GetAll()
        {
            return _refundManager.GetAll();
        }

        [HttpGet]
        public GetRefundDto Get(long id)
        {
            return _refundManager.Get(id);
        }

        [HttpPost]
        public async Task<GridResult<GetRefundDto>> GetAllPaging(GridParam input)
        {
            return await _refundManager.GetAllPaging(input);
        }

        public List<GetEmployeeBasicInfoDto> GetAllEmployeeNotInRefund(long id)
        {
            return _refundManager.GetAllEmployeeNotInRefund(id);
        }

        [HttpPost]
        public async Task<CreateRefundDto> Create(CreateRefundDto input)
        {
            return await _refundManager.Create(input);
        }

        [HttpPut]
        public async Task<UpdateRefundDto> Update(UpdateRefundDto input)
        {
            return await _refundManager.Update(input);
        }

        [HttpGet]
        public async Task<long> ActiveRefund(long id)
        {
            return await _refundManager.ActiveRefund(id);
        }

        [HttpGet]
        public async Task<long> DeActiveRefund(long id)
        {
            return await _refundManager.DeActiveRefund(id);
        }

        [HttpDelete]
        public async Task<long> Delete(long id)
        {
            return await _refundManager.Delete(id);
        }

        public List<string> GetListRefundDate()
        {
            return _refundManager.GetListRefundDate();
        }
        public List<DateTime> GetListMonthsForCreate()
        {
            return _refundManager.GetListMonthsForCreate();
        }

        [HttpPost]
        public async Task<GridResult<GetRefundEmployeeDto>> GetRefundEmployeesPaging(long id, GetEmployeePunishment input)
        {
            return await _refundManager.GetRefundEmployeesPaging(id, input);
        }

        [HttpPost]
        public async Task<AddEmployeeToRefundDto> AddEmployeeToRefund(AddEmployeeToRefundDto input)
        {
            return await _refundManager.AddEmployeeToRefund(input);
        }

        [HttpPut]
        public async Task<UpdateRefundemployeeDto> UpdateRefundEmployee(UpdateRefundemployeeDto input)
        {
            return await _refundManager.UpdateRefundEmployee(input);
        }

        [HttpDelete]
        public async Task<long> DeleteRefundEmployee(long id)
        {
            return await _refundManager.DeleteRefundEmployee(id);
        }
    }
}
