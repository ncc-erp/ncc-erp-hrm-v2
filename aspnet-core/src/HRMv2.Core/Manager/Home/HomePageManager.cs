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
using HRMv2.Manager.Histories;
using HRMv2.Manager.Home.Dtos;
using HRMv2.Manager.Home.Dtos.ChartDto;
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
                        ChartDataType.Employee => GetDataEmpoyeeLineChart(employeeMonthlyDetail, detail, labels),
                        _ => new List<double>()
                    }
                };
                result.Charts.Add(chart);
            }

            return result;

        }

        

        public List<EmployeeDetailDto> GetEmployeeMonthlyDetail(List<DateTime> allMonths)
        {
            DateTime firstDayOfCurrentMonth = DateTimeUtils.GetFirstDayOfMonth(DateTime.Now);
            var previousMonths = allMonths.Where(m => m < firstDayOfCurrentMonth).ToList();


            var employeesDetail = GetEmployeeDetailFromPreviousMonths(previousMonths).ToList();

            var listEmployeeIds = employeesDetail.Select(emd => emd.EmployeeId).Distinct().ToList();

            var allEmloyeeWorkingHistories = _workingHistoryManager.QueryAllWorkingHistoryForChart().ToList();

            if (allMonths.Contains(firstDayOfCurrentMonth))
            {
                var employeeInCurrentMonth = GetEmployeeDetailFromCurrentMonth(firstDayOfCurrentMonth, allEmloyeeWorkingHistories);
                employeesDetail = employeeInCurrentMonth != null
                                ? employeesDetail.Concat(employeeInCurrentMonth).ToList()
                                : employeesDetail;
            }

            foreach (var employee in employeesDetail)
            {
                employee.Status = GetMontlyStatus(employee, allEmloyeeWorkingHistories);
            }

            return employeesDetail;
        }


        public IEnumerable<EmployeeDetailDto> GetEmployeeDetailFromPreviousMonths (List<DateTime> previousMonths)
        {
            var employeesInPreviousMonth = WorkScope.GetAll<Payslip>()
                //.Include(p => p.Employee)
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
                    EmployeeStatus.Quit => allEmloyeeWorkingHistories.Any(wh => wh.EmployeeId == employee.EmployeeId
                                                                                    && wh.DateAt < matchingHistory.DateAt
                                                                                    && wh.DateAt >= DateTimeUtils.GetFirstDayOfMonth(matchingHistory.DateAt)
                                                                                    && wh.Status == EmployeeStatus.Working)
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

        public List<double> GetDataEmpoyeeLineChart(List<EmployeeDetailDto> employeeMonthlyDetail, ChartDetailDto detail, List<string> labels)
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
                        .GroupBy(x => x.Month.ToString("MM-yyyy"))
                        .ToDictionary(
                            g => g.Key,
                            g => (double)g.ToList().Count
                        );

            List<double> result = labels.Select(label => employeeMonthlyDetailForChart.ContainsKey(label) ? employeeMonthlyDetailForChart[label] : 0).ToList();
            return result;
        }
    }
}