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

        #region Employee chart

        #region Data
        public IQueryable<ChartSettingDto> QueryAllEmployeeChartSetting()
        {
            var query = WorkScope.GetAll<Chart>()
                .Where(s => s.IsActive == true && s.ChartDataType == ChartDataType.Employee)
                .Select(s => new ChartSettingDto
                {
                    Id = s.Id,
                    Name = s.Name,
                    ChartType = s.ChartType,
                    ChartDataType = s.ChartDataType,
                    ChartDetails = s.ChartDetails.Where(x => x.IsActive == true)
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
                                            }).ToList() // chưa executed
                });

            return query;
        }

        public List<PayslipChartDto> GetDataForAllChartEmployee(List<DateTime> allMonths) // T: DateTime startDate, endDate
        {
            DateTime firstDayOfCurrentMonth = DateTimeUtils.GetFirstDayOfMonth(DateTime.Now); //lấy ngày đầu tiên của tháng hiện tại
            var previousMonths = allMonths.Where(m => m < firstDayOfCurrentMonth).ToList(); //lấy các tháng trước tháng hiện tại

            var employeesDetail = GetPayslips(previousMonths).ToList();

            var listEmployeeIds = employeesDetail.Select(emd => emd.EmployeeId).Distinct().ToList();

            var allEmloyeeWorkingHistories = _workingHistoryManager.QueryAllWorkingHistoryForChart().ToList(); // T: => dictionary employee Id, employee

            if (allMonths.Contains(firstDayOfCurrentMonth)) //nếu tháng được chọn có chứa tháng hiện tại
            {
                var employeeInCurrentMonth = GetEmployeeDetailFromCurrentMonth(firstDayOfCurrentMonth, allEmloyeeWorkingHistories);
                employeesDetail = employeeInCurrentMonth != null
                                ? employeesDetail.Concat(employeeInCurrentMonth).ToList()
                                : employeesDetail;
            }

            foreach (var employee in employeesDetail) //set trạng thái employee cho từng tháng dựa trên EmployeeWorkingHistory
            {
                employee.Status = GetMontlyStatus(employee, allEmloyeeWorkingHistories); // T: chỉ truyền vào status, không nên truyền vào cả cục to
            }

            return employeesDetail;
        }

        #endregion

        public async Task<ResultChartDto> GetAllDataEmployeeCharts(DateTime startDate, DateTime endDate)
        {
            var allChartIds = WorkScope.GetAll<Chart>()
                .Where(c => c.IsActive == true && c.ChartDataType == ChartDataType.Employee)
                .Select(c => c.Id)
                .ToList();

            var result = await GetDataEmployeeCharts(allChartIds, startDate, endDate);

            return result;

        }

        public async Task<ResultChartDto> GetDataEmployeeCharts(List<long> chartIds, [Required] DateTime startDate, [Required] DateTime endDate)
        {
            var listChartInfo = QueryAllEmployeeChartSetting()
                .Where(c => chartIds.Contains(c.Id))
                .ToList();

            if (listChartInfo.IsNullOrEmpty())
            {
                return null;
            }

            var listDate = DateTimeUtils.GetListDate(startDate, endDate);
            var labels = listDate.Select(x => x.ToString("MM-yyyy")).ToList();
            var allDataForChartEmployee = GetDataForAllChartEmployee(listDate); // T: rename

            var resultChart = new ResultChartDto()
            {
                ChartDataType = ChartDataType.Employee
            };

            foreach (var chartInfo in listChartInfo)
            {
                if (chartInfo.ChartType == ChartType.Line)
                {
                    var result = GetDataForOneLineChartEmployee(chartInfo, allDataForChartEmployee, labels); // T: rename
                    resultChart.LineCharts.Add(result);
                }
                else if (chartInfo.ChartType == ChartType.Circle)
                {
                    //var result = GetDataCircleEmployeeChart(chartInfo, employeeMonthlyDetail, labels);
                    //resultChart.CircleCharts.Add(result);
                }
            }

            return resultChart;
        }

        #region line chart
        public ResultLineChartDto GetDataForOneLineChartEmployee(
            ChartSettingDto chartInfo,
            List<PayslipChartDto> employeeMonthlyDetail,
            List<string> labels)
        {
            var result = new ResultLineChartDto
            {
                Id = chartInfo.Id,
                ChartName = chartInfo.Name
            };

            foreach (var chartDetail in chartInfo.ChartDetails)
            {
                var chart = new LineChartData
                {
                    LineName = chartDetail.Name,
                    Color = chartDetail.Color,
                    Data = GetDataLineEmployeeChart(employeeMonthlyDetail, chartDetail, labels) ?? new List<double>(),
                };
                result.Lines.Add(chart);
            }
            return result;
        }


        public List<double> GetDataLineEmployeeChart(List<PayslipChartDto> employeeMonthlyDetail, ChartDetailDto detail, List<string> labels)
        {
            //lấy data theo chart setting 
            var employeeMonthlyDetailForChart = FilterDataLineEmployeeChart(employeeMonthlyDetail, detail);
            // lấy data string dựa trên label
            List<double> result = labels.Select(label => employeeMonthlyDetailForChart.ContainsKey(label)
                                                        ? (double)employeeMonthlyDetailForChart[label].ToList().Count : 0).ToList();
            return result;
        }

        public Dictionary<string, List<PayslipChartDto>> FilterDataLineEmployeeChart(List<PayslipChartDto> employeeMonthlyDetail, ChartDetailDto detail)
        {
            var employeeMonthlyDetailForChart = employeeMonthlyDetail
                        .WhereIf(detail.ListJobPositionId.Any(), x => detail.ListJobPositionId.Contains(x.JobPositionId))
                        .WhereIf(detail.ListLevelId.Any(), x => detail.ListLevelId.Contains(x.LevelId))
                        .WhereIf(detail.ListBranchId.Any(), x => detail.ListBranchId.Contains(x.BranchId))
                        .WhereIf(detail.ListTeamId.Any(), x => detail.ListTeamId.Any(teamIds => x.TeamIds.Contains(teamIds)))
                        .WhereIf(detail.ListUserType.Any(), x => detail.ListUserType.Contains(x.UserType))
                        .WhereIf(detail.ListGender.Any(), x => detail.ListGender.Contains(x.Gender))
                        .WhereIf(detail.ListWorkingStatus.Any(), x => detail.ListWorkingStatus.Contains(x.Status))
                        .OrderBy(x => x.Month)
                        .GroupBy(x => x.MonthYear)
                        .ToDictionary(
                            g => g.Key,
                            g => g.ToList()
                        );
            return employeeMonthlyDetailForChart;
        }

        #endregion

        #region Process data
        public IEnumerable<PayslipChartDto> GetPayslips(DateTime startDate, DateTime endDate) // T: rename from getEmployee -> getPayslip
        {
            var firstDateOfMonth = DateTimeUtils.FirstDayOfMonth(startDate);
            var lastDateOfMonth = DateTimeUtils.GetLastDayOfMonth(endDate);
            var employeesInPreviousMonth = WorkScope.GetAll<Payslip>()
                .Select(p => new PayslipChartDto // T: rename from employeeDto -> PayslipChartDto
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
                .Where(payslip => payslip.Month >= startDate
                                && payslip.Month <= endDate);
            return employeesInPreviousMonth;
        }

        public List<PayslipChartDto> GetEmployeeDetailFromCurrentMonth(DateTime firstDayOfCurrentMonth, List<EmployeeWorkingHistoryDetailDto> allEmloyeeWorkingHistories)
        {
            //lấy các Employee đang working và MaternityLeave ở hiện tại
            var lastDayOfCurrentMonth = DateTimeUtils.GetLastDayOfMonth(firstDayOfCurrentMonth);

            var workingEmployees = WorkScope.GetAll<Employee>()
                    .Select(x => new PayslipChartDto
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
                .Select(x => new PayslipChartDto
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

            workingEmployees = workingEmployees ?? new List<PayslipChartDto>();
            otherEmployees = otherEmployees ?? new List<PayslipChartDto>();

            //ghép 2 list, chỉ lấy những nhân viên workingEmployees không nằm trong otherEmployees để ghép
            var employeesInCurrentMonth = otherEmployees
                .Where(x => !workingEmployees.Any(w => w.EmployeeId == x.EmployeeId))
                .Union(workingEmployees)
                .ToList();
            return employeesInCurrentMonth;
        }

        public EmployeeMonthlyStatus GetMontlyStatus(PayslipChartDto employee, List<EmployeeWorkingHistoryDetailDto> allEmloyeeWorkingHistories)
        {
            // Tìm kiếm bản ghi lịch sử làm việc tương ứng với tháng
            var matchingHistory = allEmloyeeWorkingHistories
                .Where(wh => wh.EmployeeId == employee.EmployeeId && DateTimeUtils.GetFirstDayOfMonth(wh.DateAt) == DateTimeUtils.GetFirstDayOfMonth(employee.Month))
                .OrderByDescending(wh => wh.DateAt)
                .FirstOrDefault();

            if (matchingHistory != null)
            {
                // Cập nhật trạng thái dựa trên lịch sử làm việc 
                employee.MonthlyStatus = matchingHistory.Status switch
                {
                    EmployeeMonthlyStatus.Pausing or EmployeeMonthlyStatus.MaternityLeave => matchingHistory.Status,
                    EmployeeMonthlyStatus.Working => IsBackToWork(employee, matchingHistory, allEmloyeeWorkingHistories)
                                            ? EmployeeMonthlyStatus.BackToWork
                                            : EmployeeMonthlyStatus.Onboard,
                    EmployeeMonthlyStatus.Quit => IsOnOffInMonth(employee, matchingHistory, allEmloyeeWorkingHistories)
                                            ? EmployeeMonthlyStatus.OnOffInMonth
                                            : EmployeeMonthlyStatus.Quit,
                    _ => employee.MonthlyStatus
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

                employee.MonthlyStatus = lastStatusBeforeApplyMonth == EmployeeMonthlyStatus.MaternityLeave ? EmployeeMonthlyStatus.MaternityLeave : EmployeeMonthlyStatus.Working;
            }
            return employee.MonthlyStatus;
        }

        public bool IsBackToWork(PayslipChartDto employee, EmployeeWorkingHistoryDetailDto matchingHistory, List<EmployeeWorkingHistoryDetailDto> allEmloyeeWorkingHistories)
        {
            var historiesBeforeWorking = allEmloyeeWorkingHistories
                        .Where(wh => wh.EmployeeId == employee.EmployeeId
                                        && wh.DateAt < matchingHistory.DateAt)
                        .OrderByDescending(wh => wh.DateAt)
                        .FirstOrDefault();
            if (historiesBeforeWorking != null && (historiesBeforeWorking.Status == EmployeeMonthlyStatus.Pausing || historiesBeforeWorking.Status == EmployeeMonthlyStatus.MaternityLeave))
                return true;
            return false;
        }

        public bool IsOnOffInMonth(PayslipChartDto employee, EmployeeWorkingHistoryDetailDto matchingHistory, List<EmployeeWorkingHistoryDetailDto> allEmloyeeWorkingHistories)
        {
            var historiesBeforeQuit = allEmloyeeWorkingHistories
                        .Where(wh => wh.EmployeeId == employee.EmployeeId
                                        && wh.DateAt < matchingHistory.DateAt)
                        .OrderByDescending(wh => wh.DateAt)
                        .Take(2)
                        .ToList();

            if (historiesBeforeQuit.Count == 2
                && historiesBeforeQuit[0].Status == EmployeeMonthlyStatus.Working
                && (historiesBeforeQuit[1].Status == EmployeeMonthlyStatus.Pausing || historiesBeforeQuit[1].Status == EmployeeMonthlyStatus.MaternityLeave))
            {
                return false;
            }
            else if (historiesBeforeQuit.Any()
                     && historiesBeforeQuit.First().Status == EmployeeMonthlyStatus.Working
                     && historiesBeforeQuit.First().DateAt >= DateTimeUtils.GetFirstDayOfMonth(matchingHistory.DateAt))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        #endregion

        #endregion

        #region Payslip chart

        public IQueryable<PayslipDetailDataChartDto> QueryAllPayslipDetail(DateTime startDate, DateTime endDate)
        {
            var result = WorkScope.GetAll<PayslipDetail>()
                .Where(pd => pd.Payslip.Payroll.Status >= PayrollStatus.ApprovedByCEO && // accept payroll status: 6 , 7
                    pd.Payslip.Payroll.ApplyMonth >= startDate && pd.Payslip.Payroll.ApplyMonth <= endDate)
                .Select(pd => new PayslipDetailDataChartDto
                {
                    Id = pd.Id,
                    PayslipId = pd.PayslipId,
                    Money = pd.Money, // Case: Punishment value is minus
                    Type = pd.Type,
                    ApplyMonth = pd.Payslip.Payroll.ApplyMonth,
                    Payslip = new PayslipDataChartDto
                    {
                        FullName = pd.Payslip.Employee.FullName,
                        Id = pd.PayslipId,
                        Gender = pd.Payslip.Employee.Sex,
                        Salary = pd.Payslip.Salary,
                        BranchId = pd.Payslip.BranchId,
                        EmployeeId = pd.Payslip.EmployeeId,
                        JobPositionId = pd.Payslip.JobPositionId,
                        LevelId = pd.Payslip.LevelId,
                        TeamIds = pd.Payslip.PayslipTeams.Select(team => team.TeamId).ToList(),
                        UserType = pd.Payslip.UserType
                    }
                })
                .OrderBy(p => p.ApplyMonth);

            return result;
        }

        public async Task<List<ChartSettingDto>> GetAllSalaryChartSetting()
        {
            var listChartInfo = await WorkScope.GetAll<Chart>()
                .Where(s => s.IsActive == true && s.ChartDataType == ChartDataType.Salary)
                .Select(s => new ChartSettingDto
                {
                    Id = s.Id,
                    Name = s.Name,
                    ChartType = s.ChartType,
                    ChartDataType = s.ChartDataType,
                    ChartDetails = s.ChartDetails.Where(x => x.IsActive == true)
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

        public async Task<ResultChartDto> GetAllDataPayslipCharts(DateTime startDate, DateTime endDate)
        {
            var allChartIds = WorkScope.GetAll<Chart>()
                .Where(c => c.IsActive == true && c.ChartDataType == ChartDataType.Salary)
                .Select(c => c.Id)
                .ToList();

            var result = await GetDataPayslipCharts(allChartIds, startDate, endDate);

            return result;

        }

        public async Task<ResultChartDto> GetDataPayslipCharts(List<long> chartIds, [Required] DateTime startDate, [Required] DateTime endDate)
        {
            var listChartInfo = (await GetAllSalaryChartSetting())
                .Where(c => chartIds.Contains(c.Id))
                .ToList();

            if (listChartInfo.IsNullOrEmpty())
            {
                return null;
            }

            var payslipDetails = QueryAllPayslipDetail(startDate, endDate).ToList();
            var allMonths = DateTimeUtils.GetListDate(startDate, endDate);
            var labels = allMonths.Select(x => x.ToString("MM-yyyy")).ToList();
            var resultChart = new ResultChartDto()
            {
                ChartDataType = ChartDataType.Salary,
            };

            foreach (var chartInfo in listChartInfo)
            {
                if (chartInfo.ChartType == ChartType.Line)
                {
                    var result = GetDataLinePayslipCharts(chartInfo, payslipDetails, labels);
                    resultChart.LineCharts.Add(result);
                }
                else if (chartInfo.ChartType == ChartType.Circle)
                {
                    var result = GetDataCirclePayslipChart(chartInfo, payslipDetails);
                    resultChart.CircleCharts.Add(result);
                }
            }

            return resultChart;
        }

        #region Line payslip chart
        public ResultLineChartDto GetDataLinePayslipCharts(ChartSettingDto chartInfo, List<PayslipDetailDataChartDto> payslipDetail, List<string> labels)
        {
            var result = new ResultLineChartDto
            {
                Id = chartInfo.Id,
                ChartName = chartInfo.Name
            };

            foreach (var chartDetail in chartInfo.ChartDetails)
            {
                var chart = new LineChartData
                {
                    LineName = chartDetail.Name,
                    Color = chartDetail.Color,
                    Data = GetDataPayslipLineChart(chartDetail, payslipDetail, labels) ?? new List<double>(),
                };
                result.Lines.Add(chart);
            }
            return result;
        }

        /// <summary>
        /// Get Data each line in SalaryChartType
        /// </summary>
        /// <returns>List&lt;money&gt;</returns>
        public List<double> GetDataPayslipLineChart(
            ChartDetailDto detail,
            List<PayslipDetailDataChartDto> payslipDetails,
            List<string> labels)
        {
            var salaryMonthlyDictionary = FilterDataSalaryLineChart(detail, payslipDetails);
            // lấy data string dựa trên label
            List<double> result = labels
                .Select(label => salaryMonthlyDictionary.ContainsKey(label) ? salaryMonthlyDictionary[label] : 0)
                .ToList();
            return result;
        }

        /// <summary>
        /// Filter data by filters selection and return a dictionary of time and money.
        /// </summary>
        /// <returns>Dictionary&lt;time, money&gt;</returns>
        public Dictionary<string, double> FilterDataSalaryLineChart(
            ChartDetailDto detail,
            List<PayslipDetailDataChartDto> payslipDetails)
        {
            if (detail.ListPayslipDetailType.Any())
            {
                var result = payslipDetails
                    .Where(pd => detail.ListPayslipDetailType.Contains(pd.Type))
                    .WhereIf(detail.ListGender.Any(), p => detail.ListGender.Contains(p.Payslip.Gender))
                    .WhereIf(detail.ListBranchId.Any(), p => detail.ListBranchId.Contains(p.Payslip.BranchId))
                    .WhereIf(detail.ListJobPositionId.Any(), p => detail.ListJobPositionId.Contains(p.Payslip.JobPositionId))
                    .WhereIf(detail.ListLevelId.Any(), p => detail.ListLevelId.Contains(p.Payslip.LevelId))
                    .WhereIf(detail.ListUserType.Any(), p => detail.ListUserType.Contains(p.Payslip.UserType))
                    .WhereIf(detail.ListTeamId.Any(), p => detail.ListTeamId.Any(teamId => p.Payslip.TeamIds.Contains(teamId)))
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
                    .WhereIf(detail.ListBranchId.Any(), p => detail.ListBranchId.Contains(p.Payslip.BranchId))
                    .WhereIf(detail.ListJobPositionId.Any(), p => detail.ListJobPositionId.Contains(p.Payslip.JobPositionId))
                    .WhereIf(detail.ListLevelId.Any(), p => detail.ListLevelId.Contains(p.Payslip.LevelId))
                    .WhereIf(detail.ListUserType.Any(), p => detail.ListUserType.Contains(p.Payslip.UserType))
                    .WhereIf(detail.ListTeamId.Any(), p => detail.ListTeamId.Any(teamId => p.Payslip.TeamIds.Contains(teamId)))
                    .GroupBy(p => p.MonthYear)
                    .ToDictionary(
                            g => g.Key,
                            g => g.Sum(p => p.Payslip.Salary > 0 ? p.Payslip.Salary : 0)
                    );

                return result;
            }

        }
        #endregion

        #region Circle payslip chart

        public ResultCircleChartDto GetDataCirclePayslipChart(
            ChartSettingDto chartInfo,
            List<PayslipDetailDataChartDto> payslipDetails)
        {
            var result = new ResultCircleChartDto
            {
                Id = chartInfo.Id,
                ChartName = chartInfo.Name,
                Pies = new List<CircleChartData>()
            };

            foreach (var chartDetail in chartInfo.ChartDetails)
            {
                var chart = new CircleChartData
                {
                    Id = chartDetail.Id,
                    PieName = chartDetail.Name,
                    Color = chartDetail.Color,
                    Data = GetDataCircleSalaryChart(chartDetail, payslipDetails)
                };
                result.Pies.Add(chart);
            }
            return result;
        }

        public double GetDataCircleSalaryChart(
            ChartDetailDto chartDetail,
            List<PayslipDetailDataChartDto> payslipDetails)
        {
            if (chartDetail.ListPayslipDetailType.Any())
            {
                var payslipDetailFilteredData = payslipDetails
                    .Where(pd => chartDetail.ListPayslipDetailType.Contains(pd.Type))
                .WhereIf(chartDetail.ListGender.Any(), p => chartDetail.ListGender.Contains(p.Payslip.Gender))
                .WhereIf(chartDetail.ListBranchId.Any(), p => chartDetail.ListBranchId.Contains(p.Payslip.BranchId))
                .WhereIf(chartDetail.ListJobPositionId.Any(), p => chartDetail.ListJobPositionId.Contains(p.Payslip.JobPositionId))
                .WhereIf(chartDetail.ListLevelId.Any(), p => chartDetail.ListLevelId.Contains(p.Payslip.LevelId))
                .WhereIf(chartDetail.ListUserType.Any(), p => chartDetail.ListUserType.Contains(p.Payslip.UserType))
                .WhereIf(chartDetail.ListTeamId.Any(), p => chartDetail.ListTeamId.Any(teamId => p.Payslip.TeamIds.Contains(teamId)));

                var result = payslipDetailFilteredData.Sum(p => Math.Abs(p.Money));

                return result != 0 ? result : 0;
            }
            else
            {
                var payslipFilteredData = payslipDetails
                .WhereIf(chartDetail.ListGender.Any(), p => chartDetail.ListGender.Contains(p.Payslip.Gender))
                .WhereIf(chartDetail.ListBranchId.Any(), p => chartDetail.ListBranchId.Contains(p.Payslip.BranchId))
                .WhereIf(chartDetail.ListJobPositionId.Any(), p => chartDetail.ListJobPositionId.Contains(p.Payslip.JobPositionId))
                .WhereIf(chartDetail.ListLevelId.Any(), p => chartDetail.ListLevelId.Contains(p.Payslip.LevelId))
                .WhereIf(chartDetail.ListUserType.Any(), p => chartDetail.ListUserType.Contains(p.Payslip.UserType))
                .WhereIf(chartDetail.ListTeamId.Any(), p => chartDetail.ListTeamId.Any(teamId => p.Payslip.TeamIds.Contains(teamId)));

                var result = payslipFilteredData.Sum(p => p.Payslip.Salary > 0 ? p.Payslip.Salary : 0);

                return result != 0 ? result : 0;
            }
        }

        #endregion

        #endregion

        #region GetDataCharts
        public async Task<ResultChartDto> GetDataCharts(List<long> ids, DateTime startDate, DateTime endDate)
        {
            var chart = (await GetAllSalaryChartSetting());

            var result = new ResultChartDto();
            return result;
        }

        #endregion
    }
}