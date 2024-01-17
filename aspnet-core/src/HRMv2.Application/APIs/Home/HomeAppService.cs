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
        public async Task<List<ResultLineChartDto>> GetDataLineCharts(
            InputListChartDto input)
        {
            var result = await _homePageManager.GetDataLineCharts(input.ChartIds, input.StartDate, input.EndDate);
            return result;
        }

        [HttpPost]
        public async Task<List<ResultCircleChartDto>> GetDataCircleCharts(
            InputListChartDto input)
        {
            var result = await _homePageManager.GetDataCircleCharts(input.ChartIds, input.StartDate, input.EndDate);
            return result;
        }

        [HttpPost]
        public async Task<List<ResultChartDto>> GetAllActiveCharts(
            InputListChartDto input)
        {
            var result = await _homePageManager.GetAllDataCharts(input.StartDate, input.EndDate);
            return result;
        }
    }
}
