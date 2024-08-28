using Abp.Authorization;
using HRMv2.Manager.Categories.Charts;
using HRMv2.Manager.Categories.Charts.ChartDetails;
using HRMv2.Manager.Categories.Charts.DisplayChartDto;
using HRMv2.Manager.Employees.Dto;
using HRMv2.Manager.Home;
using HRMv2.Manager.Home.Dtos;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HRMv2.APIs.Home
{
    [AbpAuthorize]
    public class HomeAppService : HRMv2AppServiceBase
    {
        private readonly HomePageManager _homePageManager;
        private readonly ChartManager _chartManager;
        private readonly ChartDetailManager _chartDetailManager;

        public HomeAppService(HomePageManager homePageManager, ChartManager chartManager, ChartDetailManager chartDetailManager)
        {
            _homePageManager = homePageManager;
            _chartManager = chartManager;
            _chartDetailManager = chartDetailManager;
        }

        [HttpGet]
        public List<HomepageEmployeeStatisticDto> GetAllWorkingHistory(DateTime startDate, DateTime endDate)
        {
            return _homePageManager.GetAllEmployeeWorkingHistoryByTimeSpan(startDate, endDate);
        }

        [HttpPost]
        public async Task<ResultChartDto> GetAllDataEmployeeCharts(
            InputListChartDto input)
        {
            var result = await _chartManager.GetAllDataEmployeeCharts(input.StartDate, input.EndDate);
            return result;
        }

        [HttpPost]
        public async Task<ResultChartDto> GetDataEmployeeCharts(
            InputListChartDto input)
        {
            var result = await _chartManager.GetDataEmployeeCharts(input.ChartIds, input.StartDate, input.EndDate);
            return result;
        }

        [HttpPost]
        public async Task<ResultChartDto> GetAllDataPayslipCharts(
            InputListChartDto input)
        {
            var result = await _chartManager.GetAllDataPayslipCharts(input.StartDate, input.EndDate);
            return result;
        }

        [HttpPost]
        public async Task<ResultChartDto> GetDataPayslipCharts(
            InputListChartDto input)
        {
            var result = await _chartManager.GetDataPayslipCharts(input.ChartIds, input.StartDate, input.EndDate);
            return result;
        }

        [HttpPost]
        public async Task<List<EmployeeDataFromChartDetailDto>> GetDetailDataChart(InputChartDetailDto input)
        {
            var result = await _chartDetailManager.GetDetailDataChart(input.ChartDetailId, input.ChartDataType, input.StartDate, input.EndDate);
            return result;
        }

        [HttpPost]
        public async Task<FileBase64Dto> ExportOnboardQuitEmployees(InputDateRangeDto input)
        {
            return await _homePageManager.ExportOnboardQuitEmployees(input);
        }
    }
}
