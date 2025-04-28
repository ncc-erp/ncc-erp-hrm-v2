using Abp.Authorization;
using HRMv2.Authorization;
using HRMv2.Manager.Employees.Dto;
using HRMv2.Manager.Employees;
using HRMv2.Manager.PunishmentFunds.Dto;
using HRMv2.Manager.Report;
using HRMv2.Manager.Report.Dto;
using HRMv2.Manager.Salaries.Payslips.Dto;
using Microsoft.AspNetCore.Mvc;
using NccCore.Paging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMv2.APIs.Report
{
    public class ReportAppService : HRMv2AppServiceBase
    {
        private readonly ReportManager _reportManager;
        public ReportAppService(ReportManager reportManager) {
          _reportManager = reportManager;
        }

        [HttpPost]
        [AbpAuthorize(PermissionNames.Report_Salary_View)]
        public async  Task<ResultReportSalary> GetListReportSalary(InputMultiFilterReportSalaryPagingDto input)
        {
            return await _reportManager.GetAllPaging(input);
        }

        [HttpPost]
        [AbpAuthorize(PermissionNames.Report_Salary_Export)]
        public async Task<FileBase64Dto> ExportReportSalary(InputMultiFilterReportSalaryPagingDto input)
        {
            return await _reportManager.ExportReportSalary(input);
        }
       
    }
}
