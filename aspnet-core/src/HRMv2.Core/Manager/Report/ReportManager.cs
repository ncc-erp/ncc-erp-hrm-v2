
using Amazon.S3.Model;
using HRMv2.Entities;
using HRMv2.Manager.Benefits.Dto;
using HRMv2.Manager.Employees;
using HRMv2.Manager.Employees.Dto;
using HRMv2.Manager.Report.Dto;
using HRMv2.Manager.Salaries.Payrolls.Dto;
using HRMv2.Manager.Salaries.Payslips;
using HRMv2.Manager.Salaries.Payslips.Dto;
using HRMv2.NccCore;
using HRMv2.Net.MimeTypes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using NccCore.Extension;
using NccCore.Paging;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HRMv2.Constants.Enum.HRMEnum;

namespace HRMv2.Manager.Report
{
    public class ReportManager : BaseManager
    {
        private readonly IWorkScope workScope;
        private readonly PayslipManager payslipManager;
        private readonly EmployeeManager employeeManager;
        private readonly string templateFolder = Path.Combine("wwwroot", "template");
        public ReportManager(IWorkScope workScope, PayslipManager payslipManager, EmployeeManager employeeManager) : base(workScope)
        {
            this.workScope = workScope;
            this.payslipManager = payslipManager;
            this.employeeManager = employeeManager;
        }

        public Dictionary<long, string> GetApplyDatePayroll()
        {
            var dic = WorkScope.GetAll<Payroll>()
                .Where(x => x.Status == PayrollStatus.Executed)              
                .Select(x => new PayrollWithStatusExecute
                {
                    ApplyDate = x.ApplyMonth,
                    Value = x.Id
                })
                .ToDictionary(x => x.Value, x => x.Name);
            return dic;
        }
        private List<ReportSalaryDto> GetReportSalary(IQueryable<GetPayslipDto> query)
        {
            var dic = GetApplyDatePayroll();
            var queryResult = query.GroupBy(x => x.EmployeeId)
               .Select(x => new ReportSalaryDto
               {
                   InfoEmployee = x.Select(s => new InfoEmployeeDto
                   {
                       AvatarFullPath = s.Avatar,
                       Email = s.Email,
                       BranchInfo = s.BranchInfo,
                       JobPositionInfo = s.JobPositionInfo,
                       UserTypeInfo = s.UserTypeInfo,
                       EmployeeId = s.EmployeeId,
                       FullName = s.FullName,
                       Sex = s.Sex,
                   }).First(),
                   ResultReports = x.Select(r => new ResultReport
                   {
                       ApplyDate = dic[r.PayrollId],
                       Salary = r.Salary,
                   }).ToList()
               }).ToList();
            return queryResult;
        }
        public async Task<List<ReportSalaryDto>> GetAllReport(InputMultiFilterReportSalaryPagingDto input)
        {
            var query = payslipManager.QueryAllPayslip();


            if (input.PayrollIds != null && input.PayrollIds.Count > 1) query = query.Where(x => input.PayrollIds.Contains(x.PayrollId));
            else if (input.PayrollIds != null && input.PayrollIds.Count == 1) query = query.Where(x => input.PayrollIds[0] == x.PayrollId);


            if (input.EmployeeIds != null && input.EmployeeIds.Count > 1) query = query.Where(x => input.EmployeeIds.Contains(x.EmployeeId));
            else if (input.EmployeeIds != null && input.EmployeeIds.Count == 1) query = query.Where(x => input.EmployeeIds[0] == x.EmployeeId);


            if (input.BranchIds != null && input.BranchIds.Count == 1) query = query.Where(x => input.BranchIds[0] == x.ToBranchId);
            else if (input.BranchIds != null && input.BranchIds.Count > 1) query = query.Where(x => input.BranchIds.Contains(x.ToBranchId));

            if (input.JobPositionIds != null && input.JobPositionIds.Count == 1) query = query.Where(x => input.JobPositionIds[0] == x.ToJobPositionId);
            else if (input.JobPositionIds != null && input.JobPositionIds.Count > 1) query = query.Where(x => input.JobPositionIds.Contains(x.ToJobPositionId));

            if (input.UserTypes != null && input.UserTypes.Count == 1) query = query.Where(x => input.UserTypes[0] == x.ToUserType);
            else if (input.UserTypes != null && input.UserTypes.Count > 1) query = query.Where(x => input.UserTypes.Contains(x.ToUserType));

            if (input.TeamIds == null || input.TeamIds.Count == 0)
            {
                var res = GetReportSalary(query);
                return res;
            }
            if (input.TeamIds.Count == 1 || !input.IsAndCondition)
            {
                var employeeHaveAnyTeams = QueryEmployeeHaveAnyTeams(input.TeamIds).Distinct();

                query = from employee in query
                        join employeeId in employeeHaveAnyTeams
                        on employee.EmployeeId equals employeeId
                        select employee;
                var res = GetReportSalary(query);
                return res;
            }
            
            var employeeIds = employeeManager.QueryEmployeeHaveAllTeams(input.TeamIds).Result;
            query = query.Where(x => employeeIds.Contains(x.Id));


            var result = GetReportSalary(query);
            return result;

        }
        public async Task<FileBase64Dto> ExportReportSalary(InputMultiFilterReportSalaryPagingDto input) 
        {
            var templateFilePath = Path.Combine(templateFolder, "Export-ReportSalary.xlsx");
            using (var memoryStream = new MemoryStream(File.ReadAllBytes(templateFilePath)))
            {
                using (var package = new ExcelPackage(memoryStream))
                {
                    ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                    FillDataToExport(package, input);

                    string fileBase64 = Convert.ToBase64String(package.GetAsByteArray());
                    return new FileBase64Dto
                    {
                        FileName = "ExportReportSalary",
                        FileType = MimeTypeNames.ApplicationVndOpenxmlformatsOfficedocumentSpreadsheetmlSheet,
                        Base64 = fileBase64
                    };
                }
            }
        }
       
        public async Task FillDataToExport(ExcelPackage package, InputMultiFilterReportSalaryPagingDto input)
        {
            var reportSalarys = await GetAllReport(input);
            var worksheet = package.Workbook.Worksheets[0];
            var rowIndex = 2;
            var columnIndex = 4;

            var columns = reportSalarys
                .SelectMany(x => x.ResultReports.Select(s => s.ApplyDate))
                .Distinct()
                .OrderBy(date => date) 
                .ToList();

            var columnMappings = new Dictionary<String, int>();
            foreach (var date in columns)
            {
                worksheet.Cells[1, columnIndex].Value = date;
                columnMappings[date] = columnIndex;
                columnIndex++;
            }


            foreach (var report in reportSalarys)
            {
                worksheet.Cells[rowIndex, 1].Value = rowIndex - 1;
                worksheet.Cells[rowIndex, 2].Value = report.InfoEmployee.Email;
                worksheet.Cells[rowIndex, 3].Value = report.TotalSalary;
                worksheet.Cells[rowIndex, 3].Style.Numberformat.Format = "0";
                var salaryMap = report.ResultReports.ToDictionary(r => r.ApplyDate, r => r.Salary);


                foreach (var date in columns)
                {
                    if (salaryMap.ContainsKey(date))
                    {
                        worksheet.Cells[rowIndex, columnMappings[date]].Value = salaryMap[date];
                        worksheet.Cells[rowIndex, columnMappings[date]].Style.Numberformat.Format = "0";
                    }

                }
                rowIndex++;
            }
        }

    }
}
