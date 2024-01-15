using Abp.Authorization;
using Abp.Collections.Extensions;
using HRMv2.Manager.Home;
using HRMv2.Manager.Home.Dtos;
using HRMv2.Manager.Home.Dtos.ChartDto;
using Microsoft.AspNetCore.Mvc;
using NccCore.Uitls;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using static HRMv2.Constants.Enum.HRMEnum;

namespace HRMv2.APIs.Home
{
    [AbpAuthorize]
    public class HomeAppService : HRMv2AppServiceBase
    {
        private readonly HomePageManager _homePageManager;

        public HomeAppService(HomePageManager homePageManager)
        {
            _homePageManager = homePageManager;
        }

        [HttpGet]
        public List<HomepageEmployeeStatisticDto> GetAllWorkingHistory(DateTime startDate, DateTime endDate)
        {
            return _homePageManager.GetAllEmployeeWorkingHistoryByTimeSpan(startDate, endDate);
        }

        [HttpPost]
        public async Task<List<ResultLineChartDto>> GetDataLineChart(
            InputListChartDto input)
        {
            var result = await _homePageManager.GetDataLineChart(input.ChartIds, input.StartDate, input.EndDate);
            return result;
        }

        [HttpPost]
        public List<int> TestDataChart(DateTime startDate, DateTime endDate, List<EmployeeStatus> status)
        {
            var allMonths = DateTimeUtils.GetMonthYearLabelDateTime(DateTimeUtils.GetFirstDayOfMonth(startDate), endDate);
            var labels = allMonths.Select(x => x.ToString("MM-yyyy")).ToList();
            var employeeMonthlyDetailForChart = _homePageManager.GetEmployeeMonthlyDetail(allMonths)
                        .WhereIf(status.Any(), x => status.Contains(x.Status))
                        .OrderBy(x => x.Month)
                        .GroupBy(x => x.Month.ToString("MM-yyyy"))
                        .ToDictionary(
                            g => g.Key,
                            g => g.ToList().Count
                        );

            List<int> result = labels.Select(label => employeeMonthlyDetailForChart.ContainsKey(label) ? employeeMonthlyDetailForChart[label] : 0).ToList();

            return result;
        }

    }
}
