using HRMv2.Manager.CheckIn;
using HRMv2.Manager.CheckIn.Dto;
using HRMv2.NCC;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMv2.APIs.CheckIn
{
    public class CheckInAppService: HRMv2AppServiceBase
    {
        private readonly CheckInManager _checkInManager;
        public CheckInAppService(CheckInManager checkInManager)
        {
            _checkInManager = checkInManager;
        }
        [HttpGet]
        public List<GetUserForCheckInDto> GetUserForCheckIn()
        {
            return _checkInManager.GetUserForCheckIn();
        }
    }
}