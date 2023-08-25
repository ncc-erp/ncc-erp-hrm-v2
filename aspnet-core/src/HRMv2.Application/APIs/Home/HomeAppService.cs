using Abp.Authorization;
using HRMv2.Manager.Home;
using HRMv2.Manager.Home.Dtos;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

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
    }
}
