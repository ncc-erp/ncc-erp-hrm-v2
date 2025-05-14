
using Abp.UI;
using Amazon.S3.Model;
using HRMv2.Entities;
using HRMv2.Manager.Benefits.Dto;
using HRMv2.Manager.Employees;
using HRMv2.Manager.Employees.Dto;
using HRMv2.Manager.Report.Dto;
using HRMv2.Manager.Salaries.Dto;
using HRMv2.Manager.Salaries.Payrolls.Dto;
using HRMv2.Manager.Salaries.Payslips;
using HRMv2.Manager.Salaries.Payslips.Dto;
using HRMv2.NccCore;
using HRMv2.Net.MimeTypes;
using HRMv2.Utils;
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

        public Dictionary<long, string> GetDicPayrollIdToName()
        {
            var dic = WorkScope.GetAll<Payroll>()
                .Where(x => x.Status == PayrollStatus.Executed)
                .Select(x => new PayrollWithStatusExecute
                {
                    ApplyDate = x.ApplyMonth,
                    Value = x.Id
                }).ToList()
                .ToDictionary(x => x.Value, x => x.Name);
            return dic;
        }
        private List<ReportSalaryDto> GetReportSalary(IQueryable<GetPayslipDto> query)
        {
            var dicPayrollIdToName = GetDicPayrollIdToName();
            var queryResult = query.ToList()
                .GroupBy(x => x.EmployeeId)
               .Select(x => new ReportSalaryDto
               {
                   InfoEmployee = x.Select(s => new InfoEmployeeDto
                   {
                       AvatarFullPath = FileUtil.FullFilePath(s.Avatar),
                       Email = s.Email,
                       BranchInfo = s.BranchInfo,
                       JobPositionInfo = s.JobPositionInfo,
                       UserTypeInfo = s.UserTypeInfo,
                       LevelInfo = s.LevelInfo,
                       EmployeeId = s.EmployeeId,
                       FullName = s.FullName,
                       Sex = s.Sex,
                       TeamIds = s.TeamIds,
                   }).First(),
                   ResultReports = x.Select(r => new ResultReport
                   {
                       PayrollName = dicPayrollIdToName[r.PayrollId],
                       Salary = r.Salary,
                   }).ToList()
               }).ToList();
            return queryResult;
        }

        private IQueryable<GetPayslipDto> ApplyFilterEmployee(IQueryable<GetPayslipDto> query, InputMultiFilterReportSalaryPagingDto input)
        {
            if (input.UserTypes != null && input.UserTypes.Count == 1) query = query.Where(x => input.UserTypes[0] == x.UserType);
            else if (input.UserTypes != null && input.UserTypes.Count > 1) query = query.Where(x => input.UserTypes.Contains(x.UserType));

            if (input.BranchIds != null && input.BranchIds.Count > 1) query = query.Where(x => input.BranchIds.Contains(x.BranchId));
            else if (input.BranchIds != null && input.BranchIds.Count == 1) query = query.Where(x => input.BranchIds[0] == x.BranchId);

            if (input.JobPositionIds != null && input.JobPositionIds.Count == 1) query = query.Where(x => input.JobPositionIds[0] == x.JobPositionId);
            else if (input.JobPositionIds != null && input.JobPositionIds.Count > 1) query = query.Where(x => input.JobPositionIds.Contains(x.JobPositionId));

            if (input.LevelIds != null && input.LevelIds.Count == 1) query = query.Where(x => input.LevelIds[0] == x.LevelId);
            else if (input.LevelIds != null && input.LevelIds.Count > 1) query = query.Where(x => input.LevelIds.Contains(x.LevelId));

            if (input.TeamIds != null && input.TeamIds.Count == 1) query = query.Where(x => x.TeamIds.Contains(input.TeamIds[0]));
            else if (input.TeamIds != null && input.TeamIds.Count > 1) query = query.Where(x => x.TeamIds.Any(s => input.TeamIds.Contains(s)));
            return query;

        }

        private IQueryable<GetPayslipDto> ApplyFilterPaySlip(IQueryable<GetPayslipDto> query, InputMultiFilterReportSalaryPagingDto input)
        {
            if (input.BranchPayslipIds != null && input.BranchPayslipIds.Count == 1) query = query.Where(x => input.BranchPayslipIds[0] == x.BranchPayslipId);
            else if (input.BranchPayslipIds != null && input.BranchPayslipIds.Count > 1) query = query.Where(x => input.BranchPayslipIds.Contains(x.BranchPayslipId));

            if (input.JobPositionPayslipIds != null && input.JobPositionPayslipIds.Count == 1) query = query.Where(x => input.JobPositionPayslipIds[0] == x.JobPositionPayslipId);
            else if (input.JobPositionPayslipIds != null && input.JobPositionPayslipIds.Count > 1) query = query.Where(x => input.JobPositionPayslipIds.Contains(x.JobPositionPayslipId));

            if (input.UserTypePayslips != null && input.UserTypePayslips.Count == 1) query = query.Where(x => input.UserTypePayslips[0] == x.UserTypePayslip);
            else if (input.UserTypePayslips != null && input.UserTypePayslips.Count > 1) query = query.Where(x => input.UserTypePayslips.Contains(x.UserTypePayslip));

            if (input.LevelPayslipIds != null && input.LevelPayslipIds.Count == 1) query = query.Where(x => input.LevelPayslipIds[0] == x.LevelPayslipId);
            else if (input.LevelPayslipIds != null && input.LevelPayslipIds.Count > 1) query = query.Where(x => input.LevelPayslipIds.Contains(x.LevelPayslipId));

            if (input.TeamPayslipIds != null && input.TeamPayslipIds.Count == 1) query = query.Where(x => x.PayslipTeamIds.Contains(input.TeamPayslipIds[0]));
            else if (input.TeamPayslipIds != null && input.TeamPayslipIds.Count > 1) query = query.Where(x => x.PayslipTeamIds.Any(x => input.TeamPayslipIds.Contains(x)));

            return query;

        }
        public List<ReportSalaryDto> GetAllReportFilter(InputMultiFilterReportSalaryPagingDto input)
        {

            var query = payslipManager.QueryAllPayslip();

            if (input.PayrollIds != null && input.PayrollIds.Count > 1) query = query.Where(x => input.PayrollIds.Contains(x.PayrollId));
            else if (input.PayrollIds != null && input.PayrollIds.Count == 1) query = query.Where(x => input.PayrollIds[0] == x.PayrollId);

            if (input.EmployeeIds != null && input.EmployeeIds.Count > 1) query = query.Where(x => input.EmployeeIds.Contains(x.EmployeeId));
            else if (input.EmployeeIds != null && input.EmployeeIds.Count == 1) query = query.Where(x => input.EmployeeIds[0] == x.EmployeeId);

            query = ApplyFilterEmployee(query, input);

            query = ApplyFilterPaySlip(query, input);

            var result = GetReportSalary(query);
            return result;
        }

        public async Task<ResultReportSalary> GetAllPaging(InputMultiFilterReportSalaryPagingDto input)
        {
            if(input.PayrollIds.Count > 0)
            {
                throw new UserFriendlyException("You must select at least one payroll.");
            }
            var result = GetAllReportFilter(input);
            var totalSalaryByMonth = result.SelectMany(x => x.ResultReports)
                .GroupBy(x => x.PayrollName)
                .Select(t => new ResultReport
                {
                    PayrollName = t.Key,
                    Salary = t.Sum(r => r.Salary),
                }).ToList();


            var totalCount = result.Count;

            var dicPayrollIdToName = GetDicPayrollIdToName();
            var listPayrollNames = new List<string>();


            listPayrollNames = input.PayrollIds
                    .Where(id => dicPayrollIdToName.ContainsKey(id))
                    .Select(id => dicPayrollIdToName[id])
                    .ToList();
            

            var pagedResult = result.Skip(input.GridParam.SkipCount)
                .Take(input.GridParam.MaxResultCount)
                .ToList();

            return new ResultReportSalary
            {
                Result = new GridResult<ReportSalaryDto>(pagedResult, totalCount),
                Payroll = listPayrollNames,
                ResultReport = totalSalaryByMonth

            };
        }

        public async Task<FileBase64Dto> ExportReportSalary(InputMultiFilterReportSalaryPagingDto input)
        {
            var templateFilePath = Path.Combine(templateFolder, "Export-ReportSalary.xlsx");
            using (var memoryStream = new MemoryStream(File.ReadAllBytes(templateFilePath)))
            {
                using (var package = new ExcelPackage(memoryStream))
                {
                    ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                    await FillDataToExport(package, input);

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

        private string GetNameTeam(List<long> teamIds, Dictionary<long, string> teamDic)
        {
            return string.Join(", ", teamIds
                .Where(x => teamDic.ContainsKey(x))
                .Select(x => teamDic[x]));
        }

        public async Task FillDataToExport(ExcelPackage package, InputMultiFilterReportSalaryPagingDto input)
        {
            input.GridParam.MaxResultCount = int.MaxValue;
            input.GridParam.SkipCount = 0;

            var reportData = await GetAllPaging(input);
            var reportSalarys = reportData.Result.Items;
            var worksheet = package.Workbook.Worksheets[0];
            var rowIndex = 2;
            var columnIndex = 9;

            var columns = reportSalarys
                .SelectMany(x => x.ResultReports.Select(s => s.PayrollName))
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

            var dicEmployeeTeam = workScope.GetAll<Team>()
               .ToDictionary(x => x.Id, x => x.Name);

            foreach (var report in reportSalarys)
            {
                var team = GetNameTeam(report.InfoEmployee.TeamIds, dicEmployeeTeam);
                worksheet.Cells[rowIndex, 1].Value = rowIndex - 1;
                worksheet.Cells[rowIndex, 2].Value = report.InfoEmployee.Email;
                worksheet.Cells[rowIndex, 3].Value = report.InfoEmployee.BranchInfo.Name;
                worksheet.Cells[rowIndex, 4].Value = report.InfoEmployee.JobPositionInfo.Name;
                worksheet.Cells[rowIndex, 5].Value = report.InfoEmployee.LevelInfo.Name;
                worksheet.Cells[rowIndex, 6].Value = report.InfoEmployee.UserTypeInfo.Name;
                worksheet.Cells[rowIndex, 7].Value = team;
                worksheet.Cells[rowIndex, 8].Value = report.TotalSalary;

                var salaryMap = report.ResultReports
                      .GroupBy(r => r.PayrollName)
                      .ToDictionary(g => g.Key, g => g.First().Salary);


                foreach (var date in columns)
                {
                    if (salaryMap.ContainsKey(date))
                    {
                        worksheet.Cells[rowIndex, columnMappings[date]].Value = salaryMap[date];
                    }
                }
                rowIndex++;
            }

            var totalAllSalary = reportData.ResultReport.Sum(x => x.Salary);

            worksheet.Cells[rowIndex, 1].Value = "Tổng";
            worksheet.Cells[rowIndex, 1, rowIndex, 7].Merge = true;
            worksheet.Row(rowIndex).Style.Font.Bold = true;
            worksheet.Cells[rowIndex, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
            worksheet.Cells[rowIndex, 8].Value = totalAllSalary;

            var totalPerMonth = reportData.ResultReport
                                         .GroupBy(x => x.PayrollName)
                                         .ToDictionary(x => x.Key, x => x.Sum(a => a.Salary));
            foreach (var date in columns)
            {
                if (totalPerMonth.ContainsKey(date))
                {
                    worksheet.Cells[rowIndex, columnMappings[date]].Value = totalPerMonth[date];
                }
            }

        }

    }
}
