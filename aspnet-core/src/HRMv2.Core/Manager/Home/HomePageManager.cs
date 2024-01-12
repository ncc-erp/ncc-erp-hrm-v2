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

namespace HRMv2.Manager.Home
{
    public class HomePageManager : BaseManager
    {
        protected readonly WorkingHistoryManager _workingHistoryManager;
        protected readonly ChartManager _chartManager;
        protected readonly ChartDetailManager _chartDetailManager;
        

        public HomePageManager(
            IWorkScope workScope,
            WorkingHistoryManager workingHistoryManager,
            ChartManager chartManager,
            ChartDetailManager chartDetailManager
            ) : base(workScope)
        {
            _workingHistoryManager = workingHistoryManager;
            _chartManager = chartManager;
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

        public async Task<List<ResultLineChartDto>> GetDataLineChart(
                List<long> chartIds,
                [Required] DateTime startDate,
                [Required] DateTime endDate)
        {
            var query = WorkScope.GetAll<Chart>()
                .Where(s => s.IsActive == true)
                .Where(s => s.ChartType == ChartType.Line);

            if (chartIds != null && chartIds.Any())
            {
                query = query.Where(s => chartIds.Contains(s.Id));
            }

            var listChartInfo = await query
                .Select(s => new ChartInfoDto
                {
                    Id = s.Id,
                    Name = s.Name,
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
                                                UserTypes = x.UserTypes,
                                                PayslipDetailTypes = x.PayslipDetailTypes,
                                                Gender = x.Gender,
                                                WorkingStatuses = x.WorkingStatuses
                                            }).ToList()
                }).ToListAsync();

            if (listChartInfo.IsNullOrEmpty())
            {
                return null;
            }

            var totalResult = new List<ResultLineChartDto>();

            foreach (var chartInfo in listChartInfo)
            {
                var result = GetDataLineChart(chartInfo, startDate, endDate);
                totalResult.Add(result);
            }

            return totalResult;

        }

        public IQueryable<PayslipChartDto> QueryAllPayslipInSelectedTime(DateTime startDate, DateTime endDate)
        {
            var result = WorkScope.GetAll<Payslip>()
                        .Where(p => p.CreationTime >= startDate && p.CreationTime <= endDate)
                        .Select(p => new PayslipChartDto
                        {
                            Id = p.Id,
                            EmployeeId = p.EmployeeId,
                            FullName = p.Employee.FullName,
                            Gender = p.Employee.Sex,
                            Salary = p.Salary,
                            BranchId = p.BranchId,
                            JobPositionId = p.JobPositionId,
                            LevelId = p.LevelId,
                            UserType = p.UserType,
                            TeamIds = p.PayslipTeams.Select(team => team.TeamId).ToList(),
                            CreationTime = p.CreationTime // should set same time in month to easy for solve
                        });

            return result;
        }

        public IQueryable<PayslipDetailChartDto> QueryAllPayslipDetailInSelectedTime(DateTime startDate, DateTime endDate)
        {
            var result = WorkScope.GetAll<PayslipDetail>()
                .Where(pd => pd.CreationTime >= startDate && pd.CreationTime <= endDate)
                .Select(pd => new PayslipDetailChartDto
                {
                    Id = pd.Id,
                    PayslipId = pd.PayslipId,
                    Money = pd.Money,
                    Type = pd.Type,
                    CreationTime = pd.CreationTime
                });

            return result;
        }


        public ResultLineChartDto GetDataLineChart(
            ChartInfoDto chartInfo,
            [Required] DateTime startDate,
            [Required] DateTime endDate)
        {
            var allMonths = DateTimeUtils.GetMonthYearLabelDateTime(DateTimeUtils.GetFirstDayOfMonth(startDate), endDate);
            var labels = allMonths.Select(x => x.ToString("MM-yyyy")).ToList();
            var employeeMonthlyDetail = GetEmployeeMonthlyDetail(allMonths);

            var result = new ResultLineChartDto
            {
                Labels = labels
            };

            var payslipsInSelectedTime = QueryAllPayslipInSelectedTime(startDate, endDate).ToList();

            var payslipDetailsInSelectedTime = QueryAllPayslipDetailInSelectedTime(startDate, endDate)
                .Select(pd => new PayslipDetailChartDto
                {
                    Id = pd.Id,
                    Type = pd.Type,
                    CreationTime = pd.CreationTime,
                    PayslipId = pd.PayslipId,
                    Money = pd.Money,
                    Payslip = payslipsInSelectedTime.FirstOrDefault(p => p.Id == pd.PayslipId)
                })
                .ToList();

            foreach (var detail in chartInfo.Details)
            {
                var chart = new DataLineChartDto 
                {
                    Name = detail.Name,
                    ItemStyle = new ChartStyleDto
                    {
                        Color = detail.Color
                    },
                    Type = ChartType.Line,
                    Data = chartInfo.ChartDataType switch {
                        ChartDataType.Employee => GetDataEmployeeLineChart(employeeMonthlyDetail, detail, labels),
                        ChartDataType.Salary => GetDataLineSalaryChart(detail, payslipsInSelectedTime, payslipDetailsInSelectedTime, labels),
                        _ => new List<double>(),
                    }
                };
                result.ChartDetails.Add(chart);
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
                        .WhereIf(detail.JobPositionIds.Any(), x => detail.JobPositionIds.Contains(x.JobPositionId))
                        .WhereIf(detail.LevelIds.Any(), x => detail.LevelIds.Contains(x.LevelId))
                        .WhereIf(detail.BranchIds.Any(), x => detail.BranchIds.Contains(x.BranchId))
                        .WhereIf(detail.TeamIds.Any(), x => detail.TeamIds.Any(teamIds => x.TeamIds.Contains(teamIds)))
                        .WhereIf(detail.UserTypes.Any(), x => detail.UserTypes.Contains(x.UserType))
                        .WhereIf(detail.Gender.Any(), x => detail.Gender.Contains(x.Gender))
                        .WhereIf(detail.WorkingStatuses.Any(), x => detail.WorkingStatuses.Contains(x.Status))
                        .OrderBy(x => x.Month)
                        .GroupBy(x => x.MonthYear)
                        .ToDictionary(
                            g => g.Key,
                            g => g.ToList()
                        );
            return employeeMonthlyDetailForChart;
        }

            public IEnumerable<EmployeeDetailDto> GetEmployeeDetailFromPreviousMonths (List<DateTime> previousMonths)
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
                        Month = firstDayOfCurrentMonth
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
                    Month = DateTimeUtils.GetLastDayOfMonth(x.DateAt)
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
                    EmployeeStatus.Working => allEmloyeeWorkingHistories.Any(wh => wh.EmployeeId == employee.EmployeeId
                                                                                    && wh.DateAt < matchingHistory.DateAt
                                                                                    && (wh.Status == EmployeeStatus.MaternityLeave || wh.Status == EmployeeStatus.Pausing))
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
        public List<double> GetDataLineSalaryChart(
            ChartDetailDto detail, 
            List<PayslipChartDto> payslipsInSelectedTime,
            List<PayslipDetailChartDto> payslipDetailsInSelectedTime,
            List<string> labels)
        {
            var salaryMonthlyDetailForChart = FilterDataSalaryLineChart(detail, payslipsInSelectedTime, payslipDetailsInSelectedTime);
            // lấy data string dựa trên label
            List<double> result = labels
                .Select(label => salaryMonthlyDetailForChart.ContainsKey(label) ?
                                salaryMonthlyDetailForChart[label] : 0)
                .ToList();
            return result;
        }

        //Filter data and render to Dictionary
        public Dictionary<string, double> FilterDataSalaryLineChart (
            ChartDetailDto detail,
            List<PayslipChartDto> payslipsInSelectedTime,
            List<PayslipDetailChartDto> payslipDetailsInSelectedTime)
        {
            if (detail.PayslipDetailTypes.Any())
            {
                var payslipDetailFilteredData = payslipDetailsInSelectedTime
                    .Where(pd => detail.PayslipDetailTypes.Contains(pd.Type))
                    .WhereIf(detail.Gender.Any(), p => detail.Gender.Contains(p.Payslip.Gender))
                    .WhereIf(detail.BranchIds.Any(), p => detail.BranchIds.Contains(p.Payslip.BranchId))
                    .WhereIf(detail.JobPositionIds.Any(), p => detail.JobPositionIds.Contains(p.Payslip.JobPositionId))
                    .WhereIf(detail.LevelIds.Any(), p => detail.LevelIds.Contains(p.Payslip.LevelId))
                    .WhereIf(detail.UserTypes.Any(), p => detail.UserTypes.Contains(p.Payslip.UserType))
                    .WhereIf(detail.TeamIds.Any(), p => detail.TeamIds.Any(teamId => p.Payslip.TeamIds.Contains(teamId)))
                    .OrderBy(pd => pd.CreationTime)
                    .GroupBy(pd => pd.MonthYear)
                    .ToDictionary(
                        g => g.Key,
                        g => g.Select(p => p.Money).Sum()
                    );

                return payslipDetailFilteredData;
            }
            else
            {
                var payslipFilteredData = payslipsInSelectedTime
                    .WhereIf(detail.Gender.Any(), p => detail.Gender.Contains(p.Gender))
                    .WhereIf(detail.BranchIds.Any(), p => detail.BranchIds.Contains(p.BranchId))
                    .WhereIf(detail.JobPositionIds.Any(), p => detail.JobPositionIds.Contains(p.JobPositionId))
                    .WhereIf(detail.LevelIds.Any(), p => detail.LevelIds.Contains(p.LevelId))
                    .WhereIf(detail.UserTypes.Any(), p => detail.UserTypes.Contains(p.UserType))
                    .WhereIf(detail.TeamIds.Any(), p => detail.TeamIds.Any(teamId => p.TeamIds.Contains(teamId))) //p.TeamIds.Any(teamId => detail.TeamIds.Contains(teamId))
                    .OrderBy(p => p.CreationTime)
                    .GroupBy(p => p.MonthYear)
                    .ToDictionary(
                            g => g.Key,
                            g => g.Select(p => p.Salary).Sum()
                    );

                return payslipFilteredData;
            }
        }
    }
}