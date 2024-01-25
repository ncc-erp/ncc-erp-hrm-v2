using Abp.Authorization;
using HRMv2.Authorization;
using HRMv2.Manager.Categories.Charts;
using HRMv2.Manager.Home;
using HRMv2.Manager.Home.Dtos;
using HRMv2.Manager.Home.Dtos.ChartDto;
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

        public HomeAppService(HomePageManager homePageManager, ChartManager chartManager)
        {
            _homePageManager = homePageManager;
            _chartManager = chartManager;
        }

        [HttpGet]
        public List<HomepageEmployeeStatisticDto> GetAllWorkingHistory(DateTime startDate, DateTime endDate)
        {
            return _homePageManager.GetAllEmployeeWorkingHistoryByTimeSpan(startDate, endDate);
        }

        [HttpPost]
        [AbpAuthorize(PermissionNames.Home_ViewLineChart)]
        public async Task<ResultChartDto> GetAllDataEmployeeCharts(
            InputListChartDto input)
        {
            var result = await _chartManager.GetAllDataEmployeeCharts(input.StartDate, input.EndDate);
            return result;
        }

        [HttpPost]
        [AbpAuthorize(PermissionNames.Home_ViewLineChart)]
        public async Task<ResultChartDto> GetDataEmployeeCharts(
            InputListChartDto input)
        {
            var result = await _chartManager.GetDataEmployeeCharts(input.ChartIds, input.StartDate, input.EndDate);
            return result;
        }

        [HttpPost]
        [AbpAuthorize(PermissionNames.Home_ViewCircleChart)]
        public async Task<ResultChartDto> GetAllDataPayslipCharts(
            InputListChartDto input)
        {
            var result = await _chartManager.GetAllDataPayslipCharts(input.StartDate, input.EndDate);
            return result;
        }

        [HttpPost]
        [AbpAuthorize(PermissionNames.Home_ViewCircleChart)]
        public async Task<ResultChartDto> GetDataPayslipCharts(
            InputListChartDto input)
        {
            var result = await _chartManager.GetDataPayslipCharts(input.ChartIds, input.StartDate, input.EndDate);
            return result;
        }

        [HttpPost]
        [AbpAuthorize(PermissionNames.Home_ViewCircleChart)]
        public async Task<ResultChartDto> GetDetailDataEmployeeChart(InputListChartDto input)
        {
            var result = await _chartManager.GetDataPayslipCharts(input.ChartIds, input.StartDate, input.EndDate);
            return result;
        }
    }
}
