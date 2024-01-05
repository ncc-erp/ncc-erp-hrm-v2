using Abp.Authorization;
using HRMv2.Manager.Home;
using HRMv2.Manager.Home.Dtos;
using HRMv2.Manager.Home.Dtos.Chart;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

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

        [HttpGet]
        public List<dynamic> GetAllCharts(HomepageChartFilterDto homepageChartFilterDto)
        {
            var result = _homePageManager.GetAllCharts(homepageChartFilterDto);

            return result;
        }

        [HttpGet]
        public async Task<List<ResultChartDto>> GetDataChart(
            InputListChartDto input)
        {
            var result = _homePageManager.GetDataChart(input.ChartIds, input.ChartType, input.StartDate, input.EndDate);
            return await result;
        }

        [HttpGet]
        public List<double> TestDataChart(
            DateTime startDate, DateTime endDate)
        {
            var result = _homePageManager.GetLineChartEmployeeTest(startDate, endDate);
            return result;
        }
    }
}
