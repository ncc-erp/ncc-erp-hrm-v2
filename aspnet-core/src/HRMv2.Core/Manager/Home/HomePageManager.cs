using HRMv2.Entities;
using HRMv2.Manager.Categories.Charts.ChartDetails;
using HRMv2.Manager.Categories.Charts.DisplayChartDto;
using HRMv2.Manager.Employees.Dto;
using HRMv2.Manager.Home.Dtos;

using HRMv2.Manager.WorkingHistories;
using HRMv2.Manager.WorkingHistories.Dtos;
using HRMv2.NccCore;
using HRMv2.Net.MimeTypes;
using NccCore.Extension;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using static HRMv2.Constants.Enum.HRMEnum;


namespace HRMv2.Manager.Home
{
    public class HomePageManager : BaseManager
    {
        protected readonly WorkingHistoryManager _workingHistoryManager;
        protected readonly ChartDetailManager _chartDetailManager;
        private readonly string templateFolder = Path.Combine("wwwroot", "template");

        public HomePageManager(
            IWorkScope workScope,
            WorkingHistoryManager workingHistoryManager,
            ChartDetailManager chartDetailManager
            ) : base(workScope)
        {
            _workingHistoryManager = workingHistoryManager;
            _chartDetailManager = chartDetailManager;
        }

        public List<HomepageEmployeeStatisticDto> GetAllEmployeeWorkingHistoryByTimeSpan(DateTime startDate, DateTime endDate)
        {
            List<LastEmployeeWorkingHistoryDto> empWorkingHistories = _workingHistoryManager.GetLastEmployeeWorkingHistories(startDate, endDate);

            var resultList = new List<HomepageEmployeeStatisticDto>();
            var wholeCompanyEmployees = GetEmployeeStatisticDto(empWorkingHistories, null, "Toàn công ty", startDate);
            resultList.Add(wholeCompanyEmployees);

            var branchList = empWorkingHistories.Select(s => new { s.BranchInfo.Name, s.BranchId }).Distinct().ToList();

            branchList.ForEach(branch =>
            {
                var statisticOfBranch = GetEmployeeStatisticDto(empWorkingHistories, branch.BranchId, branch.Name, startDate);

                resultList.Add(statisticOfBranch);
            });

            return resultList.OrderBy(i => i.BranchName != "Toàn công ty").ThenBy(x => x.BranchName).ToList();

        }

        public List<LastEmployeeWorkingHistoryDto> GetLastEmployeeWorkingHistories(DateTime startDate, DateTime endDate)
        {
            return _workingHistoryManager.GetLastEmployeeWorkingHistories(startDate, endDate);
        }

        private HomepageEmployeeStatisticDto GetEmployeeStatisticDto(List<LastEmployeeWorkingHistoryDto> empWorkingHistories, long? branchId, string branchName, DateTime startDate)
        {
            var histories = branchId.HasValue ? empWorkingHistories.Where(s => s.BranchId == branchId.Value) : empWorkingHistories;

            var qWorkingHistories = histories.Where(s => s.LastStatus == EmployeeStatus.Working);

            //var qOnboard = qWorkingHistories.Where(s => s.DateAt >= startDate.Date);
            //var qQuit = histories.Where(s => s.LastStatus == EmployeeStatus.Quit)
            //    .Where(s => s.DateAt >= startDate.Date);


            var item = new HomepageEmployeeStatisticDto()
            {
                OnboardEmployees = histories.Where(s => s.WorkingHistories.Any(x => x.Status == EmployeeStatus.Working && x.DateAt >= startDate.Date)).ToList(),

                QuitEmployees = histories.Where(s => s.WorkingHistories.Any(x => x.Status == EmployeeStatus.Quit && x.DateAt >= startDate.Date)).ToList(),

                PausingEmployees = histories.Where(s => s.WorkingHistories.Any(x => x.Status == EmployeeStatus.Pausing && x.DateAt >= startDate.Date)).ToList(),

                MatenityLeaveEmployees = histories.Where(s => s.WorkingHistories.Any(x => x.Status == EmployeeStatus.MaternityLeave && x.DateAt >= startDate.Date)).ToList(),

                OnboardAndQuitEmployees = histories.Where(s => s.IsOnboardAndQuitInTimeSpan).ToList(),

                EmployeeTotal = qWorkingHistories.Count(),
                InternCount = qWorkingHistories.Where(s => s.UserType == UserType.Internship).Count(),
                StaffCount = qWorkingHistories.Where(s => s.UserType == UserType.Staff).Count(),
                CTVCount = qWorkingHistories.Where(s => s.UserType == UserType.Collaborators).Count(),
                TViecCount = qWorkingHistories.Where(s => s.UserType == UserType.ProbationaryStaff).Count(),
                VendorCount = qWorkingHistories.Where(s => s.UserType == UserType.Vendor).Count(),


                BranchName = branchName,
            };

            return item;
        }

        public async Task<FileBase64Dto> ExportOnboardQuitEmployees(InputDateRangeDto input)
        {
            var templateFilePath = Path.Combine(templateFolder, "OnboardQuitEmployees.xlsx");
            using (var memoryStream = new MemoryStream(File.ReadAllBytes(templateFilePath)))
            {
                using (var package = new ExcelPackage(memoryStream))
                {
                    ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

                    // get data to export
                    var employees = GetExportData(input);

                    // fill data here
                    FillDataToExport(package, employees);

                    string fileBase64 = Convert.ToBase64String(package.GetAsByteArray());
                    var file = new FileBase64Dto()
                    {
                        FileName = $"{input.StartDate:yyyyMMdd}-{input.EndDate:yyyyMMdd}-OnboardQuitEmployees",
                        FileType = MimeTypeNames.ApplicationVndOpenxmlformatsOfficedocumentSpreadsheetmlSheet,
                        Base64 = fileBase64
                    };
                    return file;

                }
            }
        }

        private void FillDataToExport(ExcelPackage package, OnboardQuitEmployeesToExportDto data)
        {
            var onboardWorksheet = package.Workbook.Worksheets[0];
            var quitWorksheet = package.Workbook.Worksheets[1];
            var onboardAndQuitSheet = package.Workbook.Worksheets[2];

            // Fill data to Onboard Employees Sheet
            var rowIndex = 2;
            foreach (var emp in data.OnboardEmployees)
            {
                onboardWorksheet.Cells[rowIndex, 1].Value = rowIndex - 1;
                onboardWorksheet.Cells[rowIndex, 2].Value = emp.FullName;
                onboardWorksheet.Cells[rowIndex, 3].Value = emp.Email;
                onboardWorksheet.Cells[rowIndex, 4].Value = emp.Sex.ToString();
                onboardWorksheet.Cells[rowIndex, 5].Value = emp.BranchInfo.Name;
                onboardWorksheet.Cells[rowIndex, 6].Value = emp.UserTypeInfo.Name;
                onboardWorksheet.Cells[rowIndex, 7].Value = emp.LevelInfo.Name;
                onboardWorksheet.Cells[rowIndex, 8].Value = emp.JobPositionInfo.Name;
                onboardWorksheet.Cells[rowIndex, 9].Value = emp.DateAt;
                rowIndex++;
            }

            // Fill data to Quit Employees Sheet
            rowIndex = 2;
            foreach (var emp in data.QuitEmployees)
            {
                quitWorksheet.Cells[rowIndex, 1].Value = rowIndex - 1;
                quitWorksheet.Cells[rowIndex, 2].Value = emp.FullName;
                quitWorksheet.Cells[rowIndex, 3].Value = emp.Email;
                quitWorksheet.Cells[rowIndex, 4].Value = emp.Sex.ToString();
                quitWorksheet.Cells[rowIndex, 5].Value = emp.BranchInfo.Name;
                quitWorksheet.Cells[rowIndex, 6].Value = emp.UserTypeInfo.Name;
                quitWorksheet.Cells[rowIndex, 7].Value = emp.LevelInfo.Name;
                quitWorksheet.Cells[rowIndex, 8].Value = emp.JobPositionInfo.Name;
                quitWorksheet.Cells[rowIndex, 9].Value = emp.DateAt;
                rowIndex++;
            }

            // Fill data to Onboard and Quit Employees Sheet
            rowIndex = 2;
            foreach (var emp in data.OnboardAndQuitEmployees)
            {
                onboardAndQuitSheet.Cells[rowIndex, 1].Value = rowIndex - 1;
                onboardAndQuitSheet.Cells[rowIndex, 2].Value = emp.FullName;
                onboardAndQuitSheet.Cells[rowIndex, 3].Value = emp.Email;
                onboardAndQuitSheet.Cells[rowIndex, 4].Value = emp.Sex.ToString();
                onboardAndQuitSheet.Cells[rowIndex, 5].Value = emp.BranchInfo.Name;
                onboardAndQuitSheet.Cells[rowIndex, 6].Value = emp.UserTypeInfo.Name;
                onboardAndQuitSheet.Cells[rowIndex, 7].Value = emp.LevelInfo.Name;
                onboardAndQuitSheet.Cells[rowIndex, 8].Value = emp.JobPositionInfo.Name;
                onboardAndQuitSheet.Cells[rowIndex, 9].Value = emp.DateAt;
                rowIndex++;
            }
        }

        private OnboardQuitEmployeesToExportDto GetExportData(InputDateRangeDto input)
        {
            var empWorkingHistories = _workingHistoryManager.GetLastEmployeeWorkingHistories(input.StartDate, input.EndDate);
            var result = new OnboardQuitEmployeesToExportDto()
            {
                OnboardEmployees = empWorkingHistories.Where(s => s.WorkingHistories.Any(s => s.Status == EmployeeStatus.Working && s.DateAt >= input.StartDate)).ToList(),
                QuitEmployees = empWorkingHistories.Where(s => s.WorkingHistories.Any(s => s.Status == EmployeeStatus.Quit && s.DateAt >= input.StartDate)).ToList(),
                OnboardAndQuitEmployees = empWorkingHistories.Where(s => s.IsOnboardAndQuitInTimeSpan).ToList(),
            };
            return result;
        }

        public async Task<FileBase64Dto> ExportChartDetailData(InputChartDetailDto input)
        {
            var templateFilePath = Path.Combine(templateFolder, "ChartDetailDataTemplate.xlsx");
            using (var memoryStream = new MemoryStream(File.ReadAllBytes(templateFilePath)))
            {
                using (var package = new ExcelPackage(memoryStream))
                {
                    ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                    var employees = GetExportDataFromChartDetail(input);
                    FillChartDataToExport(package, employees);

                    string fileBase64 = Convert.ToBase64String(package.GetAsByteArray());
                    var file = new FileBase64Dto()
                    {
                        FileName = $"{input.StartDate.ToString("yyyyMMdd")}_{input.EndDate.ToString("yyyyMMdd")}-{employees.ChartName}_{employees.ChartDetailName}.xlsx",
                        FileType = MimeTypeNames.ApplicationVndOpenxmlformatsOfficedocumentSpreadsheetmlSheet,
                        Base64 = fileBase64
                    };
                    return file;
                }

            }
        }

        private void FillChartDataToExport(ExcelPackage package, DataFromChartDetailDto data)
        {
            var worksheet = package.Workbook.Worksheets[0];
            worksheet.Name = $"{data.ChartName} - {data.ChartDetailName}";
            var rowIndex = 2;
            foreach (var emp in data.Employees)
            {
                worksheet.Cells[rowIndex, 1].Value = data.Employees.IndexOf(emp) + 1;
                worksheet.Cells[rowIndex, 2].Value = emp.FullName;
                worksheet.Cells[rowIndex, 3].Value = emp.Email;
                worksheet.Cells[rowIndex, 4].Value = emp.Gender;
                var lastWorkingHistory = emp.MonthlyEmployeeDetails.LastOrDefault();
                worksheet.Cells[rowIndex, 5].Value = lastWorkingHistory.BranchInfo.Name;
                worksheet.Cells[rowIndex, 6].Value = lastWorkingHistory.UserTypeInfo.Name;
                worksheet.Cells[rowIndex, 7].Value = lastWorkingHistory.LevelInfo.Name;
                worksheet.Cells[rowIndex, 8].Value = lastWorkingHistory.JobPositionInfo.Name;
                worksheet.Cells[rowIndex, 10].Value = emp.Avatar;

                // fill detail by month
                var maxRowsForDetailByMonth = emp.MonthlyEmployeeDetails.Count;
                for (var i = 0; i < maxRowsForDetailByMonth; i++)
                {
                    var empDetail = emp.MonthlyEmployeeDetails[i];
                    worksheet.Cells[rowIndex + i, 9].Value = $"{empDetail.BranchInfo.Name} {empDetail.UserTypeInfo.Name} {empDetail.LevelInfo.Name} {empDetail.JobPositionInfo.Name} {empDetail.StatusMonth.Month}/{empDetail.StatusMonth.Year}";
                }
                // merge cell
                if (emp.MonthlyEmployeeDetails.Count > 1)
                {
                    worksheet.Cells[rowIndex, 1, rowIndex + maxRowsForDetailByMonth - 1, 1].Merge = true;
                    worksheet.Cells[rowIndex, 2, rowIndex + maxRowsForDetailByMonth - 1, 2].Merge = true;
                    worksheet.Cells[rowIndex, 3, rowIndex + maxRowsForDetailByMonth - 1, 3].Merge = true;
                    worksheet.Cells[rowIndex, 4, rowIndex + maxRowsForDetailByMonth - 1, 4].Merge = true;
                    worksheet.Cells[rowIndex, 5, rowIndex + maxRowsForDetailByMonth - 1, 5].Merge = true;
                    worksheet.Cells[rowIndex, 6, rowIndex + maxRowsForDetailByMonth - 1, 6].Merge = true;
                    worksheet.Cells[rowIndex, 7, rowIndex + maxRowsForDetailByMonth - 1, 7].Merge = true;
                    worksheet.Cells[rowIndex, 8, rowIndex + maxRowsForDetailByMonth - 1, 8].Merge = true;
                    worksheet.Cells[rowIndex, 10, rowIndex + maxRowsForDetailByMonth - 1, 10].Merge = true;
                }

                rowIndex += maxRowsForDetailByMonth > 0 ? maxRowsForDetailByMonth : 1;
            }
        }

        private DataFromChartDetailDto GetExportDataFromChartDetail(InputChartDetailDto input)
        {
            var chartDetailData = WorkScope.GetAll<ChartDetail>()
                                .Where(c => c.Id == input.ChartDetailId)
                                .Select(s => new DataFromChartDetailDto()
                                {
                                    ChartName = s.Chart.Name,
                                    ChartDetailName = s.Name
                                }).FirstOrDefault();            
            chartDetailData.Employees = _chartDetailManager.GetDetailDataChart(input.ChartDetailId, input.ChartDataType, input.StartDate, input.EndDate).Result;
            return chartDetailData;
        }
    }
}