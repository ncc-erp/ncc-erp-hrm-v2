using Abp.Collections.Extensions;
using HRMv2.Entities;
using HRMv2.Manager.Categories;
using HRMv2.Manager.Categories.Charts;
using HRMv2.Manager.Categories.Charts.ChartDetails;
using HRMv2.Manager.Categories.Charts.ChartDetails.Dto;
using HRMv2.Manager.Categories.Charts.Dto;
using HRMv2.Manager.Categories.JobPositions;
using HRMv2.Manager.Categories.Levels;
using HRMv2.Manager.Categories.Teams;
using HRMv2.Manager.Home.Dtos;
using HRMv2.Manager.Home.Dtos.ChartDto;
using HRMv2.Manager.WorkingHistories;
using HRMv2.Manager.WorkingHistories.Dtos;
using HRMv2.NccCore;
using Microsoft.EntityFrameworkCore;
using NccCore.Extension;
using NccCore.Uitls;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
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
        protected readonly JobPositionManager _jobPositionManager;
        protected readonly LevelManager _levelManager;
        protected readonly BranchManager _branchManager;
        protected readonly TeamManager _teamManager;

        public HomePageManager(
            IWorkScope workScope,
            WorkingHistoryManager workingHistoryManager,
            ChartManager chartManager,
            ChartDetailManager chartDetailManager,
            JobPositionManager jobPositionManager,
            LevelManager levelManager,
            BranchManager branchManager,
            TeamManager teamManager
            ) : base(workScope)
        {
            _workingHistoryManager = workingHistoryManager;
            _chartManager = chartManager;
            _chartDetailManager = chartDetailManager;
            _jobPositionManager = jobPositionManager;
            _levelManager = levelManager;
            _branchManager = branchManager;
            _teamManager = teamManager;
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

        public List<dynamic> GetAllCharts(HomepageChartFilterDto filter)
        {
            List<LastEmployeeWorkingHistoryDto> empHistories = _workingHistoryManager.GetLastEmployeeWorkingHistories(filter.StartDate, filter.StartDate);

            var charts = _chartManager.GetAll();
            var chartDetails = _chartDetailManager.GetAll();

            var datas = charts
                .GroupJoin(
                chartDetails,
                c => c.Id,
                cd => cd.ChartId,
                (c, cd) => new
                {
                    c,
                    cd = cd.ToList()
                })
                .ToList();

            foreach (var data in datas)
            {

                foreach (var chartDetail in data.cd)
                {
                    var job = _jobPositionManager.GetAll()
                        .Where(j =>
                        chartDetail.JobPositionIds.Any(jobId => jobId == j.Id))
                        .ToList();

                    var level = _levelManager.GetAll()
                        .Where(l =>
                        chartDetail.LevelIds.Any(levelId => levelId == l.Id))
                        .ToList();

                    var branch = _branchManager.GetAll()
                        .Where(b =>
                        chartDetail.BranchIds.Any(branchId => branchId == b.Id))
                        .ToList();

                    var team = _teamManager.GetAll()
                        .Where(t =>
                        chartDetail.TeamIds.Any(teamId => teamId == t.Id))
                        .ToList();

                }
            }

            return null;
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
                    TimePeriodType = s.TimePeriodType,
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
                var result = await GetDataLineChart(chartInfo, startDate, endDate);
                totalResult.Add(result);
            }

            return totalResult;

        }

        private async Task<ResultLineChartDto> GetDataLineChart(
            ChartInfoDto chartInfo,
            [Required] DateTime startDate,
            [Required] DateTime endDate)
        {
            var labels = DateTimeUtils.GetMonthYearLabelChartFromDate(startDate, endDate);

            var result = new ResultLineChartDto();
            result.Labels = labels;

            foreach (var detail in chartInfo.Details)
            {
                var chart = new DataLineChartDto();
                chart.Name = detail.Name;
                chart.ItemStyle = new ChartStyleDto
                {
                    Color = detail.Color
                };
                chart.Type = "line";

                var setEntryIds = new HashSet<long>();
                if (chartInfo.ChartDataType == ChartDataType.Employee)
                {
                    chart.Data = GetLineChartEmployee(startDate, endDate, chartInfo.TimePeriodType, labels, detail);
                }

                result.Charts.Add(chart);
            }

            return result;

        }

        public List<double> GetLineChartEmployee(
            DateTime startDate,
            DateTime endDate,
            TimePeriodType timePeriodType,
            IEnumerable<string> labels,
            ChartDetailDto detail
        )
        {
            var employees = WorkScope.GetAll<Employee>()
                .Select(x => new
                {
                    x.JobPositionId,
                    x.LevelId,
                    BranchHistories = x.BranchHistories
                                    .Where(b => b.DateAt >= startDate && b.DateAt <= endDate)
                                    .Select(b => new { b.BranchId, b.DateAt }),
                    TeamIds = x.EmployeeTeams.Select(t => t.TeamId).ToList(),
                    x.UserType,
                    x.Sex,
                    WorkingHistories = x.WorkingHistories
                                    .Where(h => h.DateAt >= startDate && h.DateAt <= endDate)
                                    .Select(h => new { h.Status, h.DateAt }),

                })
                .WhereIf(detail.JobPositionIds.Any(), x => detail.JobPositionIds.Contains(x.JobPositionId))
                .WhereIf(detail.LevelIds.Any(), x => detail.LevelIds.Contains(x.LevelId))
                .WhereIf(detail.TeamIds.Any(), x => detail.TeamIds.Any(teamIds => x.TeamIds.Contains(teamIds)))
                .WhereIf(detail.UserTypes.Any(), x => detail.UserTypes.Contains(x.UserType))
                .WhereIf(detail.Gender.Any(), x => detail.Gender.Contains(x.Sex))
                .ToList();


            var monthlySummaries = new Dictionary<(int Month, string BranchOrStatus), string>();

            foreach (var employee in employees)
            {
                // Xử lý WorkingHistories và BranchHistories trong khoảng thời gian đã cho
                var histories = employee.WorkingHistories
                            .Select(h => new
                            {
                                h.DateAt,
                                Type = "Status",
                                Value = h.Status.ToString()
                            })
                            .Concat(
                                employee.BranchHistories
                                .Select(b => new
                                {
                                    b.DateAt,
                                    Type = "Branch",
                                    Value = b.BranchId.ToString()
                                }))
                            .OrderBy(h => h.DateAt)
                            .ToList();

                string lastStatus = null;
                string lastBranchId = null;

                for (int month = startDate.Month; month <= endDate.Month; month++)
                {
                    var monthHistories = histories.Where(h => h.DateAt.Month == month).ToList();

                    if (monthHistories.Any())
                    {
                        var lastHistory = monthHistories.Last();
                        if (lastHistory.Type == "Status")
                        {
                            lastStatus = lastHistory.Value;
                        }
                        else if (lastHistory.Type == "Branch")
                        {
                            lastBranchId = lastHistory.Value;
                        }
                    }

                    string keyForStatus = $"{month}-Status";
                    string keyForBranch = $"{month}-Branch";
                    monthlySummaries[(month, keyForStatus)] = lastStatus ?? (month > startDate.Month ? monthlySummaries[(month - 1, keyForStatus)] : null);
                    monthlySummaries[(month, keyForBranch)] = lastBranchId ?? (month > startDate.Month ? monthlySummaries[(month - 1, keyForBranch)] : null);
                }
            }



            return null;
        }

        public List<double> GetLineChartEmployeeTest(
            DateTime startDate,
            DateTime endDate
        )
        {
            startDate = DateTimeUtils.GetFirstDayOfMonth(startDate);
            endDate = DateTimeUtils.GetFirstDayOfMonth(endDate);
            DateTime firstDayOfCurrentMonth = DateTimeUtils.GetFirstDayOfMonth(DateTime.Now);

            var allMonths = DateTimeUtils.GetMonthYearLabelDateTime(startDate, endDate);
            var previousMonths = allMonths.Where(m => m < firstDayOfCurrentMonth).ToList();


            var employeeMonthlyDetails = WorkScope.GetAll<Payslip>()
                .Select(payslip => new
                {
                    payslip.EmployeeId,
                    payslip.Employee.FullName,
                    payslip.JobPositionId,
                    payslip.BranchId,
                    payslip.UserType,
                    payslip.LevelId,
                    PayslipTeams = payslip.PayslipTeams
                                    .Select(team => team.TeamId).ToList(),
                    payslip.Payroll.ApplyMonth

                })
                .Where(payslip => payslip.ApplyMonth >= DateTimeUtils.GetFirstDayOfMonth(previousMonths.FirstOrDefault())
                                && payslip.ApplyMonth <= DateTimeUtils.GetLastDayOfMonth(previousMonths.LastOrDefault()))
                .ToList()
                .GroupBy(p => p.EmployeeId)
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(p => new EmployeeDetailDto
                    {
                        EmployeeId = p.EmployeeId,
                        FullName = p.FullName,
                        JobPositionId = p.JobPositionId,
                        LevelId = p.LevelId,
                        TeamIds = p.PayslipTeams,
                        UserType = p.UserType,
                        BranchId = p.BranchId,
                        Month = p.ApplyMonth
                    }).ToList()
                );

            var listEmployeeIds = employeeMonthlyDetails.Keys.ToList();

            var emloyeeWorkingHistories = WorkScope.GetAll<EmployeeWorkingHistory>()
                .Select(wh => new
                {
                    wh.EmployeeId,
                    wh.Status,
                    wh.DateAt
                })
                .Where(wh => listEmployeeIds.Contains(wh.EmployeeId))
                .Where(wh => wh.DateAt >= DateTimeUtils.GetFirstDayOfMonth(previousMonths.FirstOrDefault())
                        && wh.DateAt <= DateTimeUtils.GetLastDayOfMonth(previousMonths.LastOrDefault()))
                .ToList();

            foreach (var employeeDetailsEntry in employeeMonthlyDetails)
            {
                var employeeId = employeeDetailsEntry.Key;
                var employeeDetails = employeeDetailsEntry.Value;

                foreach (var employeeDetail in employeeDetails)
                {
                    // Tìm kiếm bản ghi lịch sử làm việc tương ứng
                    var matchingHistory = emloyeeWorkingHistories
                        .Where(wh => wh.EmployeeId == employeeId && DateTimeUtils.GetFirstDayOfMonth(wh.DateAt) == DateTimeUtils.GetFirstDayOfMonth(employeeDetail.Month))
                        .OrderByDescending(wh => wh.DateAt)
                        .FirstOrDefault();

                    if (matchingHistory != null)
                    {
                        // Cập nhật trạng thái dựa trên lịch sử làm việc
                        employeeDetail.Status = matchingHistory.Status switch
                        {
                            EmployeeStatus.Pausing or EmployeeStatus.Quit or EmployeeStatus.MaternityLeave => matchingHistory.Status,
                            EmployeeStatus.Working => emloyeeWorkingHistories.Any(wh => wh.EmployeeId == employeeId && wh.DateAt < matchingHistory.DateAt && wh.Status == EmployeeStatus.MaternityLeave) ? EmployeeStatus.Working : EmployeeStatus.Onboard,
                            _ => employeeDetail.Status
                        };
                    }
                    else
                    {
                        // Nếu không có lịch sử làm việc phù hợp, kiểm tra trạng thái gần nhất trước ApplyMonth
                        var lastStatusBeforeApplyMonth = emloyeeWorkingHistories
                            .Where(wh => wh.EmployeeId == employeeId && wh.DateAt < employeeDetail.Month)
                            .OrderByDescending(wh => wh.DateAt)
                            .Select(wh => wh.Status)
                            .FirstOrDefault();

                        employeeDetail.Status = lastStatusBeforeApplyMonth == EmployeeStatus.MaternityLeave ? EmployeeStatus.MaternityLeave : EmployeeStatus.Working;
                    }
                }
            }


            if (allMonths.Contains(firstDayOfCurrentMonth))
            {
                var employeesDetailCurrentMonth = WorkScope.GetAll<Employee>()
                    .Select(x => new
                    {
                        EmployeeId = x.Id,
                        x.FullName,
                        x.JobPositionId,
                        x.LevelId,
                        x.BranchId,
                        TeamIds = x.EmployeeTeams.Select(t => t.TeamId).ToList(),
                        x.UserType,
                        x.Sex,
                        x.Status,
                        WorkingHistories = x.WorkingHistories
                                            .Select(wh => new { wh.Status, wh.DateAt })
                                            .OrderByDescending(wh => wh.DateAt)
                                            .FirstOrDefault(),
                    })
                    .Where(x => listEmployeeIds.Contains(x.EmployeeId))
                    .ToList();




                var dicEmployeesDetailCurrentMonth = employeesDetailCurrentMonth
                    .GroupBy(x => x.EmployeeId)
                    .ToDictionary(
                        g => g.Key,
                        g => g.Select(x => new EmployeeDetailDto
                        {
                            EmployeeId = x.EmployeeId,
                            FullName = x.FullName,
                            JobPositionId = x.JobPositionId,
                            LevelId = x.LevelId,
                            BranchId = x.BranchId,
                            TeamIds = x.TeamIds,
                            UserType = x.UserType,
                            Gender = x.Sex,
                            Status = x.Status,
                            Month = firstDayOfCurrentMonth
                        }).ToList()
                    );

                foreach (var kvp in dicEmployeesDetailCurrentMonth)
                {
                    if (employeeMonthlyDetails.TryGetValue(kvp.Key, out var listEmployeeMonthlyDetail))
                    {
                        listEmployeeMonthlyDetail.AddRange(kvp.Value);
                    }
                    else
                    {
                        // Nếu key chưa tồn tại, thêm key và giá trị mới từ dictionary2 vào dictionary1
                        employeeMonthlyDetails[kvp.Key] = kvp.Value;
                    }

                }
            }
            return null;
        }
    }
}