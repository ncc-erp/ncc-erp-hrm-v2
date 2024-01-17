using Abp.Collections.Extensions;
using ClosedXML.Excel;
using HRMv2.Entities;
using HRMv2.Manager.Categories;
using HRMv2.Manager.Categories.Charts;
using HRMv2.Manager.Categories.Charts.ChartDetails;
using HRMv2.Manager.Categories.Charts.ChartDetails.Dto;
using HRMv2.Manager.Categories.Charts.Dto;
using HRMv2.Manager.Categories.JobPositions;
using HRMv2.Manager.Categories.Levels;
using HRMv2.Manager.Categories.Teams;
using HRMv2.Manager.Histories;
using HRMv2.Manager.Common.Dto;
using HRMv2.Manager.Home.Dtos;
using HRMv2.Manager.Home.Dtos.ChartDto;
using HRMv2.Manager.Salaries.Payslips.Dto;
using HRMv2.Manager.Salaries.Dto;
using HRMv2.Manager.WorkingHistories;
using HRMv2.Manager.WorkingHistories.Dtos;
using HRMv2.NccCore;
using Microsoft.EntityFrameworkCore;
using NccCore.Extension;
using NccCore.Uitls;
using OfficeOpenXml.Table.PivotTable;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using static HRMv2.Constants.Enum.HRMEnum;
using Chart = HRMv2.Entities.Chart;
using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
using HRMv2.Manager.Benefits.Dto;

namespace HRMv2.Manager.Home
{
    public class HomePageManager : BaseManager
    {
        protected readonly WorkingHistoryManager _workingHistoryManager;

        public HomePageManager(
            IWorkScope workScope,
            WorkingHistoryManager workingHistoryManager
            ) : base(workScope)
        {
            _workingHistoryManager = workingHistoryManager;
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

        public IQueryable<PayslipDetailDataChartDto> QueryAllPayslipDetail(DateTime startDate, DateTime endDate)
        {
            var result = WorkScope.GetAll<PayslipDetail>()
                .Join(WorkScope.GetAll<Payslip>(),
                    payslipDetail => payslipDetail.PayslipId,
                    payslip => payslip.Id,
                    (payslipDetail, payslip) => new { PayslipDetail = payslipDetail, Payslip = payslip }
                )
                .Join(WorkScope.GetAll<Payroll>(),
                    payslipGroup => payslipGroup.Payslip.PayrollId,
                    payroll => payroll.Id,
                    (payslipGroup, payroll) => new
                    {
                        PayslipDetail = payslipGroup.PayslipDetail,
                        Payslip = payslipGroup.Payslip,
                        Payroll = payroll
                    })
                .Where(pd => pd.Payroll.Status >= PayrollStatus.ApprovedByCEO && // accept payroll status: 6 , 7
                    pd.Payroll.ApplyMonth >= startDate && pd.Payroll.ApplyMonth <= endDate)
                .Select(pd => new PayslipDetailDataChartDto
                {
                    Id = pd.PayslipDetail.Id,
                    PayslipId = pd.PayslipDetail.PayslipId,
                    Payslip = new PayslipDataChartDto
                    {
                        Id = pd.Payslip.Id,
                        EmployeeId = pd.Payslip.EmployeeId,
                        FullName = pd.Payslip.Employee.FullName,
                        Gender = pd.Payslip.Employee.Sex,
                        Salary = pd.Payslip.Salary, // lương thực nhận < 0 => == 0
                        BranchId = pd.Payslip.BranchId,
                        JobPositionId = pd.Payslip.JobPositionId,
                        LevelId = pd.Payslip.LevelId,
                        TeamIds = pd.Payslip.PayslipTeams.Select(team => team.TeamId).ToList(),
                        UserType = pd.Payslip.UserType,
                        ApplyMonth = pd.Payroll.ApplyMonth,
                        PayrollId = pd.Payroll.Id
                    },
                    Money = pd.PayslipDetail.Money, // Case: Punishment value is minus
                    Type = pd.PayslipDetail.Type,
                    ApplyMonth = pd.Payroll.ApplyMonth
                });

            return result;
        }

        public async Task<List<ChartSettingDto>> GetAllActiveChartSetting()
        {
            var query = WorkScope.GetAll<Chart>()
                .Where(s => s.IsActive == true);

            var listChartInfo = await query
                .Select(s => new ChartSettingDto
                {
                    Id = s.Id,
                    Name = s.Name,
                    ChartType = s.ChartType,
                    ChartDataType = s.ChartDataType,
                    Details = s.ChartDetails.Where(x => x.IsActive == true)
                                            .Select(x => new ChartDetailDto
                                            {
                                                Id = x.Id,
                                                ChartId = x.ChartId,
                                                Name = x.Name,
                                                Color = x.Color,
                                                JobPositionIds = x.JobPositionIds,
                                                LevelIds = x.LevelIds,
                                                BranchIds = x.BranchIds,
                                                TeamIds = x.TeamIds,
                                                PayslipDetailTypes = x.PayslipDetailTypes,
                                                UserTypes = x.UserTypes,
                                                WorkingStatuses = x.WorkingStatuses,
                                                Gender = x.Gender,
                                            }).ToList()
                }).ToListAsync();

            return listChartInfo;
        }

        public async Task<List<ResultChartDto>> GetAllDataCharts(DateTime startDate, DateTime endDate)
        {
            var charts = await GetAllActiveChartSetting();

            var lineChartIds = charts.Where(c => c.ChartType == ChartType.Line).Select(c => c.Id).ToList();
            var resultLineCharts = await GetDataLineCharts(lineChartIds, startDate, endDate);

            var circleChartIds = charts.Where(c => c.ChartType == ChartType.Circle).Select(c => c.Id).ToList();
            var resultCircleCharts = await GetDataCircleCharts(circleChartIds, startDate, endDate);

            var result = new List<ResultChartDto>();

            result.AddRange(resultLineCharts.Select(c => new ResultChartDto
            {
                Id = c.Id,
                ChartName = c.ChartName,
                ChartType = ChartType.Line,
                Lines = c.Lines
            }));

            result.AddRange(resultCircleCharts.Select(c => new ResultChartDto
            {
                Id = c.Id,
                ChartName = c.ChartName,
                ChartType = ChartType.Circle,
                Pies = c.Pies
            }));

            return result;
        }

        public async Task<List<ResultLineChartDto>> GetDataLineCharts(
                List<long> chartIds,
                [Required] DateTime startDate,
                [Required] DateTime endDate)
        {
            var listChartInfo = (await GetAllActiveChartSetting())
                .Where(c => c.ChartType == ChartType.Line && chartIds.Contains(c.Id))
                .ToList();

            if (listChartInfo.IsNullOrEmpty())
            {
                return null;
            }

            var payslipDetails = QueryAllPayslipDetail(startDate, endDate).ToList();

            var totalResult = new List<ResultLineChartDto>();

            foreach (var chartInfo in listChartInfo)
            {
                var result = GetDataLineChart(chartInfo, startDate, endDate, payslipDetails);
                totalResult.Add(result);
            }

            return totalResult;

        }

        public ResultLineChartDto GetDataLineChart(
            ChartSettingDto chartInfo,
            [Required] DateTime startDate,
            [Required] DateTime endDate,
            List<PayslipDetailDataChartDto> payslipDetails)
        {
            var allMonths = DateTimeUtils.GetMonthYearLabelDateTime(DateTimeUtils.GetFirstDayOfMonth(startDate), endDate);
            var labels = allMonths.Select(x => x.ToString("MM-yyyy")).ToList();
            var employeeMonthlyDetail = GetEmployeeMonthlyDetail(allMonths);

            var result = new ResultLineChartDto
            {
                Id = chartInfo.Id,
                Labels = labels,
                ChartName = chartInfo.Name,
                ChartType = chartInfo.ChartType,
            };

            foreach (var detail in chartInfo.Details)
            {
                var chart = new LineChartData
                {
                    LineName = detail.Name,
                    Color = detail.Color,
                    Data = chartInfo.ChartDataType switch
                    {
                        ChartDataType.Employee => GetDataEmployeeLineChart(employeeMonthlyDetail, detail, labels),
                        ChartDataType.Salary => GetDataPayslipLineChart(detail, payslipDetails, labels),
                        _ => new List<double>(),
                    }
                };
                result.Lines.Add(chart);
            }
            return result;
        }

        public List<EmployeeDetailDto> GetEmployeeMonthlyDetail(List<DateTime> allMonths)
        {
            DateTime firstDayOfCurrentMonth = DateTimeUtils.GetFirstDayOfMonth(DateTime.Now); //lấy ngày đầu tiên của tháng hiện tại
            var previousMonths = allMonths.Where(m => m < firstDayOfCurrentMonth).ToList(); //lấy các tháng trước tháng hiện tại

            var employeesDetail = GetEmployeeDetailFromPreviousMonths(previousMonths).ToList();

            var listEmployeeIds = employeesDetail.Select(emd => emd.EmployeeId).Distinct().ToList();

            var allEmloyeeWorkingHistories = _workingHistoryManager.QueryAllWorkingHistoryForChart().ToList();

            if (allMonths.Contains(firstDayOfCurrentMonth)) //nếu tháng được chọn có chứa tháng hiện tại
            {
                var employeeInCurrentMonth = GetEmployeeDetailFromCurrentMonth(firstDayOfCurrentMonth, allEmloyeeWorkingHistories);
                employeesDetail = employeeInCurrentMonth != null
                                ? employeesDetail.Concat(employeeInCurrentMonth).ToList()
                                : employeesDetail;
            }

            foreach (var employee in employeesDetail) //set trạng thái employee cho từng tháng dựa trên EmployeeWorkingHistory
            {
                employee.Status = GetMontlyStatus(employee, allEmloyeeWorkingHistories);
            }

            return employeesDetail;
        }

        public List<double> GetDataEmployeeLineChart(List<EmployeeDetailDto> employeeMonthlyDetail, ChartDetailDto detail, List<string> labels)
        {
            //lấy data theo chart setting 
            var employeeMonthlyDetailForChart = FilterDataEmployeeLineChart(employeeMonthlyDetail, detail);
            // lấy data string dựa trên label
            List<double> result = labels.Select(label => employeeMonthlyDetailForChart.ContainsKey(label)
                                                        ? (double)employeeMonthlyDetailForChart[label].ToList().Count : 0).ToList();
            return result;
        }

        public Dictionary<string, List<EmployeeDetailDto>> FilterDataEmployeeLineChart(List<EmployeeDetailDto> employeeMonthlyDetail, ChartDetailDto detail)
        {
            var employeeMonthlyDetailForChart = employeeMonthlyDetail
                        .WhereIf(detail.ListJobPositionIds.Any(), x => detail.ListJobPositionIds.Contains(x.JobPositionId))
                        .WhereIf(detail.ListLevelIds.Any(), x => detail.ListLevelIds.Contains(x.LevelId))
                        .WhereIf(detail.ListBranchIds.Any(), x => detail.ListBranchIds.Contains(x.BranchId))
                        .WhereIf(detail.ListTeamIds.Any(), x => detail.ListTeamIds.Any(teamIds => x.TeamIds.Contains(teamIds)))
                        .WhereIf(detail.ListUserTypes.Any(), x => detail.ListUserTypes.Contains(x.UserType))
                        .WhereIf(detail.ListGender.Any(), x => detail.ListGender.Contains(x.Gender))
                        .WhereIf(detail.ListWorkingStatuses.Any(), x => detail.ListWorkingStatuses.Contains(x.Status))
                        .OrderBy(x => x.Month)
                        .GroupBy(x => x.MonthYear)
                        .ToDictionary(
                            g => g.Key,
                            g => g.ToList()
                        );
            return employeeMonthlyDetailForChart;
        }

        public IEnumerable<EmployeeDetailDto> GetEmployeeDetailFromPreviousMonths(List<DateTime> previousMonths)
        {
            var employeesInPreviousMonth = WorkScope.GetAll<Payslip>()
                .Select(p => new EmployeeDetailDto
                {
                    EmployeeId = p.EmployeeId,
                    FullName = p.Employee.FullName,
                    JobPositionId = p.JobPositionId,
                    LevelId = p.LevelId,
                    TeamIds = p.PayslipTeams.Select(team => team.TeamId).ToList(),
                    UserType = p.UserType,
                    BranchId = p.BranchId,
                    Month = p.Payroll.ApplyMonth,
                    Gender = p.Employee.Sex
                })
                .Where(payslip => payslip.Month >= DateTimeUtils.GetFirstDayOfMonth(previousMonths.FirstOrDefault())
                                && payslip.Month <= DateTimeUtils.GetLastDayOfMonth(previousMonths.LastOrDefault()));
            return employeesInPreviousMonth;
        }

        public List<EmployeeDetailDto> GetEmployeeDetailFromCurrentMonth(DateTime firstDayOfCurrentMonth, List<EmployeeWorkingHistoryDetailDto> allEmloyeeWorkingHistories)
        {
            //lấy các Employee đang working và MaternityLeave ở hiện tại
            var lastDayOfCurrentMonth = DateTimeUtils.GetLastDayOfMonth(firstDayOfCurrentMonth);

            var workingEmployees = WorkScope.GetAll<Employee>()
                    .Select(x => new EmployeeDetailDto
                    {
                        EmployeeId = x.Id,
                        FullName = x.FullName,
                        JobPositionId = x.JobPositionId,
                        LevelId = x.LevelId,
                        BranchId = x.BranchId,
                        TeamIds = x.EmployeeTeams.Select(t => t.TeamId).ToList(),
                        UserType = x.UserType,
                        Gender = x.Sex,
                        Status = x.Status,
                        Month = firstDayOfCurrentMonth,
                    })
                    .Where(x => x.Status == EmployeeStatus.Working || x.Status == EmployeeStatus.MaternityLeave)
                    .ToList();

            //lấy các Employee có working history là quit, pause, working, MaternityLeave nằm trong tháng hiện tại

            var otherEmployees = allEmloyeeWorkingHistories
                .Where(x => x.DateAt >= firstDayOfCurrentMonth && x.DateAt <= lastDayOfCurrentMonth)
                .GroupBy(x => x.EmployeeId)
                .Select(group => group.OrderBy(x => x.DateAt).Last())
                .Select(x => new EmployeeDetailDto
                {
                    EmployeeId = x.EmployeeId,
                    FullName = x.FullName,
                    JobPositionId = x.JobPositionId,
                    LevelId = x.LevelId,
                    BranchId = x.BranchId,
                    TeamIds = x.TeamIds,
                    UserType = x.UserType,
                    Gender = x.Gender,
                    Month = DateTimeUtils.GetFirstDayOfMonth(x.DateAt)
                }).ToList();

            workingEmployees = workingEmployees ?? new List<EmployeeDetailDto>();
            otherEmployees = otherEmployees ?? new List<EmployeeDetailDto>();

            //ghép 2 list, chỉ lấy những nhân viên workingEmployees không nằm trong otherEmployees để ghép
            var employeesInCurrentMonth = otherEmployees
                .Where(x => !workingEmployees.Any(w => w.EmployeeId == x.EmployeeId))
                .Union(workingEmployees)
                .ToList();
            return employeesInCurrentMonth;
        }

        public EmployeeStatus GetMontlyStatus(EmployeeDetailDto employee, List<EmployeeWorkingHistoryDetailDto> allEmloyeeWorkingHistories)
        {
            // Tìm kiếm bản ghi lịch sử làm việc tương ứng với tháng
            var matchingHistory = allEmloyeeWorkingHistories
                .Where(wh => wh.EmployeeId == employee.EmployeeId && DateTimeUtils.GetFirstDayOfMonth(wh.DateAt) == DateTimeUtils.GetFirstDayOfMonth(employee.Month))
                .OrderByDescending(wh => wh.DateAt)
                .FirstOrDefault();

            if (matchingHistory != null)
            {
                // Cập nhật trạng thái dựa trên lịch sử làm việc 
                employee.Status = matchingHistory.Status switch
                {
                    EmployeeStatus.Pausing or EmployeeStatus.MaternityLeave => matchingHistory.Status,
                    EmployeeStatus.Working => IsBackToWork(employee, matchingHistory, allEmloyeeWorkingHistories)
                                            ? EmployeeStatus.BackToWork
                                            : EmployeeStatus.Onboard,
                    EmployeeStatus.Quit => IsOnOffInMonth(employee, matchingHistory, allEmloyeeWorkingHistories)
                                            ? EmployeeStatus.OnOffInMonth
                                            : EmployeeStatus.Quit,
                    _ => employee.Status
                };
            }
            else
            {
                // Nếu không có lịch sử làm việc phù hợp, kiểm tra trạng thái gần nhất trước ApplyMonth
                var lastStatusBeforeApplyMonth = allEmloyeeWorkingHistories
                    .Where(wh => wh.EmployeeId == employee.EmployeeId && wh.DateAt < employee.Month)
                    .OrderByDescending(wh => wh.DateAt)
                    .Select(wh => wh.Status)
                    .FirstOrDefault();

                employee.Status = lastStatusBeforeApplyMonth == EmployeeStatus.MaternityLeave ? EmployeeStatus.MaternityLeave : EmployeeStatus.Working;
            }
            return employee.Status;
        }

        public bool IsBackToWork(EmployeeDetailDto employee, EmployeeWorkingHistoryDetailDto matchingHistory, List<EmployeeWorkingHistoryDetailDto> allEmloyeeWorkingHistories)
        {
            var historiesBeforeWorking = allEmloyeeWorkingHistories
                        .Where(wh => wh.EmployeeId == employee.EmployeeId
                                        && wh.DateAt < matchingHistory.DateAt)
                        .OrderByDescending(wh => wh.DateAt)
                        .FirstOrDefault();
            if (historiesBeforeWorking != null && (historiesBeforeWorking.Status == EmployeeStatus.Pausing || historiesBeforeWorking.Status == EmployeeStatus.MaternityLeave))
                return true;
            return false;
        }


        public bool IsOnOffInMonth(EmployeeDetailDto employee, EmployeeWorkingHistoryDetailDto matchingHistory, List<EmployeeWorkingHistoryDetailDto> allEmloyeeWorkingHistories)
        {
            var historiesBeforeQuit = allEmloyeeWorkingHistories
                        .Where(wh => wh.EmployeeId == employee.EmployeeId
                                        && wh.DateAt < matchingHistory.DateAt)
                        .OrderByDescending(wh => wh.DateAt)
                        .Take(2)
                        .ToList();

            if (historiesBeforeQuit.Count == 2
                && historiesBeforeQuit[0].Status == EmployeeStatus.Working
                && (historiesBeforeQuit[1].Status == EmployeeStatus.Pausing || historiesBeforeQuit[1].Status == EmployeeStatus.MaternityLeave))
            {
                return false;
            }
            else if (historiesBeforeQuit.Any()
                     && historiesBeforeQuit.First().Status == EmployeeStatus.Working
                     && historiesBeforeQuit.First().DateAt >= DateTimeUtils.GetFirstDayOfMonth(matchingHistory.DateAt))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Get Data each line in SalaryChartType
        /// </summary>
        /// <returns>List data</returns>
        public List<double> GetDataPayslipLineChart(
            ChartDetailDto detail,
            List<PayslipDetailDataChartDto> payslipDetails,
            List<string> labels)
        {
            var salaryMonthlyDetailForChart = FilterDataSalaryLineChart(detail, payslipDetails);
            // lấy data string dựa trên label
            List<double> result = labels
                .Select(label => salaryMonthlyDetailForChart.ContainsKey(label) ?
                                salaryMonthlyDetailForChart[label] : 0)
                .ToList();
            return result;
        }

        //Filter data and render to Dictionary
        public Dictionary<string, double> FilterDataSalaryLineChart(
            ChartDetailDto detail,
            List<PayslipDetailDataChartDto> payslipDetails)
        {
            if (detail.ListPayslipDetailTypes.Any())
            {
                var result = payslipDetails
                    .Where(pd => detail.ListPayslipDetailTypes.Contains(pd.Type))
                    .WhereIf(detail.ListGender.Any(), p => detail.ListGender.Contains(p.Payslip.Gender))
                    .WhereIf(detail.ListBranchIds.Any(), p => detail.ListBranchIds.Contains(p.Payslip.BranchId))
                    .WhereIf(detail.ListJobPositionIds.Any(), p => detail.ListJobPositionIds.Contains(p.Payslip.JobPositionId))
                    .WhereIf(detail.ListLevelIds.Any(), p => detail.ListLevelIds.Contains(p.Payslip.LevelId))
                    .WhereIf(detail.ListUserTypes.Any(), p => detail.ListUserTypes.Contains(p.Payslip.UserType))
                    .WhereIf(detail.ListTeamIds.Any(), p => detail.ListTeamIds.Any(teamId => p.Payslip.TeamIds.Contains(teamId)))
                    .OrderBy(pd => pd.ApplyMonth)
                    .GroupBy(pd => pd.MonthYear)
                    .ToDictionary(
                        g => g.Key,
                        g => g.Sum(p => Math.Abs(p.Money)) // CASE: payslip detail type == Punishment
                    );

                return result;
            }
            else
            {
                var result = payslipDetails
                    .WhereIf(detail.ListGender.Any(), p => detail.ListGender.Contains(p.Payslip.Gender))
                    .WhereIf(detail.ListBranchIds.Any(), p => detail.ListBranchIds.Contains(p.Payslip.BranchId))
                    .WhereIf(detail.ListJobPositionIds.Any(), p => detail.ListJobPositionIds.Contains(p.Payslip.JobPositionId))
                    .WhereIf(detail.ListLevelIds.Any(), p => detail.ListLevelIds.Contains(p.Payslip.LevelId))
                    .WhereIf(detail.ListUserTypes.Any(), p => detail.ListUserTypes.Contains(p.Payslip.UserType))
                    .WhereIf(detail.ListTeamIds.Any(), p => detail.ListTeamIds.Any(teamId => p.Payslip.TeamIds.Contains(teamId)))
                    .OrderBy(p => p.ApplyMonth)
                    .GroupBy(p => p.MonthYear)
                    .ToDictionary(
                            g => g.Key,
                            g => g.Sum(p => p.Payslip.Salary > 0 ? p.Payslip.Salary : 0)
                    );

                return result;
            }

        }

        public async Task<List<ResultCircleChartDto>> GetDataCircleCharts(List<long> chartIds, DateTime startDate, DateTime endDate)
        {
           
            var listChartInfo = (await GetAllActiveChartSetting())
                .Where(c => c.ChartType == ChartType.Circle && chartIds.Contains(c.Id))
                .ToList();

            if (listChartInfo.IsNullOrEmpty())
            {
                return null;
            }

            var payslipDetailsInSelectedTime = QueryAllPayslipDetail(startDate, endDate)
                .ToList();

            var totalResult = new List<ResultCircleChartDto>();

            foreach (var chartInfo in listChartInfo)
            {
                var result = GetDataCircleChart(chartInfo, payslipDetailsInSelectedTime);
                totalResult.Add(result);
            }

            return totalResult;
        }

        public ResultCircleChartDto GetDataCircleChart(
            ChartSettingDto chartInfo,
            List<PayslipDetailDataChartDto> payslipDetails)
        {
            var result = new ResultCircleChartDto
            {
                Id = chartInfo.Id,
                ChartName = chartInfo.Name,
                Pies = new List<CircleChartData>()
            };

            foreach (var chartDetail in chartInfo.Details)
            {
                var chart = new CircleChartData
                {
                    Id = chartDetail.Id,
                    PieName = chartDetail.Name,
                    Color = chartDetail.Color,
                    Data = chartInfo.ChartDataType switch
                    {
                        ChartDataType.Employee => 0,
                        ChartDataType.Salary => GetDataCircleSalaryChart(chartDetail, payslipDetails),
                        _ => 0,
                    }
                };
                result.Pies.Add(chart);
            }
            return result;
        }

        public double GetDataCircleSalaryChart(
            ChartDetailDto chartDetail,
            List<PayslipDetailDataChartDto> payslipDetails)
        {
            if (chartDetail.ListPayslipDetailTypes.Any())
            {
                var payslipDetailFilteredData = payslipDetails
                    .Where(pd => chartDetail.ListPayslipDetailTypes.Contains(pd.Type))
                .WhereIf(chartDetail.ListGender.Any(), p => chartDetail.ListGender.Contains(p.Payslip.Gender))
                .WhereIf(chartDetail.ListBranchIds.Any(), p => chartDetail.ListBranchIds.Contains(p.Payslip.BranchId))
                .WhereIf(chartDetail.ListJobPositionIds.Any(), p => chartDetail.ListJobPositionIds.Contains(p.Payslip.JobPositionId))
                .WhereIf(chartDetail.ListLevelIds.Any(), p => chartDetail.ListLevelIds.Contains(p.Payslip.LevelId))
                .WhereIf(chartDetail.ListUserTypes.Any(), p => chartDetail.ListUserTypes.Contains(p.Payslip.UserType))
                .WhereIf(chartDetail.ListTeamIds.Any(), p => chartDetail.ListTeamIds.Any(teamId => p.Payslip.TeamIds.Contains(teamId)));

                var result = payslipDetailFilteredData.Sum(p => Math.Abs(p.Money));

                return result;
            }
            else
            {
                var payslipFilteredData = payslipDetails
                .WhereIf(chartDetail.ListGender.Any(), p => chartDetail.ListGender.Contains(p.Payslip.Gender))
                .WhereIf(chartDetail.ListBranchIds.Any(), p => chartDetail.ListBranchIds.Contains(p.Payslip.BranchId))
                .WhereIf(chartDetail.ListJobPositionIds.Any(), p => chartDetail.ListJobPositionIds.Contains(p.Payslip.JobPositionId))
                .WhereIf(chartDetail.ListLevelIds.Any(), p => chartDetail.ListLevelIds.Contains(p.Payslip.LevelId))
                .WhereIf(chartDetail.ListUserTypes.Any(), p => chartDetail.ListUserTypes.Contains(p.Payslip.UserType))
                .WhereIf(chartDetail.ListTeamIds.Any(), p => chartDetail.ListTeamIds.Any(teamId => p.Payslip.TeamIds.Contains(teamId)));

                var result = payslipFilteredData.Sum(p => p.Payslip.Salary > 0 ? p.Payslip.Salary : 0);

                return result;
            }
        }
    }
}