using Abp.Authorization;
using HRMv2.Authorization;
using HRMv2.Manager.Punishments.Dto;
using HRMv2.Manager.Punishments;
using Microsoft.AspNetCore.Mvc;
using NccCore.Paging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HRMv2.NCC;
using HRMv2.Manager.Salaries.Payslips.Dto;
using HRMv2.Manager.Salaries.Payslips;

namespace HRMv2.APIs.VoucherTool
{
    [AbpAuthorize]
    public class VoucherToolAppService : HRMv2AppServiceBase
    {
        private readonly PayslipManager _payslipManager;
        public VoucherToolAppService(PayslipManager payslipManager)
        {
            _payslipManager = payslipManager;
        }

        [HttpPost]
        [NccAuthentication]
        public async Task<List<ResponseApplyVoucherDto>> ApplyVoucher(List<InputApplyVoucherDto> input)
        {
            return await _payslipManager.ApplyVoucherToAllEmployee(input);
        }
    }
}

