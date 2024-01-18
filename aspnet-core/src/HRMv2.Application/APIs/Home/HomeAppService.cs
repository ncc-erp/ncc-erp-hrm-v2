using Abp.Authorization;
using Abp.Collections.Extensions;
using HRMv2.Authorization;
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
        [AbpAuthorize(PermissionNames.Home_ViewLineChart)]
        public async Task<ResultChartDto> GetAllDataEmployeeCharts(
            InputListChartDto input)
        {
            var result = await _homePageManager.GetAllDataEmployeeCharts(input.StartDate, input.EndDate);
            return result;
        }

        [HttpPost]
        [AbpAuthorize(PermissionNames.Home_ViewLineChart)]
        public async Task<ResultChartDto> GetDataEmployeeCharts(
            InputListChartDto input)
        {
            var result = await _homePageManager.GetDataEmployeeCharts(input.ChartIds, input.StartDate, input.EndDate);
            return result;
        }

        [HttpPost]
        [AbpAuthorize(PermissionNames.Home_ViewCircleChart)]

        public async Task<ResultChartDto> GetAllDataPayslipCharts(
            InputListChartDto input)
        {
            var result = await _homePageManager.GetAllDataPayslipCharts(input.StartDate, input.EndDate);
            return result;
        }

        [HttpPost]
        [AbpAuthorize(PermissionNames.Home_ViewCircleChart)]

        public async Task<ResultChartDto> GetDataPayslipCharts(
            InputListChartDto input)
        {
            var result = await _homePageManager.GetDataPayslipCharts(input.ChartIds, input.StartDate, input.EndDate);
            return result;
        }
    }
}
