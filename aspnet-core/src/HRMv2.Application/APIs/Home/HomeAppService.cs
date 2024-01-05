using Abp.Authorization;
using HRMv2.Manager.Home;
using HRMv2.Manager.Home.Dtos;
using HRMv2.Manager.Home.Dtos.ChartDto;
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
        public async Task<List<ResultLineChartDto>> GetDataLineChart(
            InputListChartDto input)
        {
            var result = _homePageManager.GetDataLineChart(input.ChartIds, input.StartDate, input.EndDate);
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
