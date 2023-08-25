using Abp.Authorization;
using HRMv2.Authorization;
using HRMv2.Manager.ChangeEmployeeWorkingStatuses;
using HRMv2.Manager.ChangeEmployeeWorkingStatuses.Dto;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMv2.APIs.ChangeEmployeeWorkingStatus
{
    [AbpAuthorize]
    public class ChangeEmployeeWorkingStatusAppService : HRMv2AppServiceBase
    {
        private readonly ChangeEmployeeWorkingStatusManager _changeEmployeeWorkingStatus;
        public ChangeEmployeeWorkingStatusAppService(ChangeEmployeeWorkingStatusManager changeEmployeeWorkingStatus)
        {
            _changeEmployeeWorkingStatus = changeEmployeeWorkingStatus;
        }

        [HttpPost]
        [AbpAuthorize(PermissionNames.Employee_EmployeeDetail_TabPersonalInfo_ChangeWorkingStatus_Quit)]
        public void ChangeStatusToQuit(ToQuitDto input)
        {
             _changeEmployeeWorkingStatus.ChangeStatusToQuit(input);
        }

        [HttpPost]
        [AbpAuthorize(PermissionNames.Employee_EmployeeDetail_TabPersonalInfo_ChangeWorkingStatus_Pause)]
        public void ChangeStatusToPause(ToPauseDto input)
        {
            _changeEmployeeWorkingStatus.ChangeStatusToPause(input);
        }

        [HttpPost]
        [AbpAuthorize(PermissionNames.Employee_EmployeeDetail_TabPersonalInfo_ChangeWorkingStatus_MaternityLeave)]
        public void ChangeStatusToMaterityLeave (ToMaternityLeaveDto input)
        {
             _changeEmployeeWorkingStatus.ChangeStatusToMaternityLeave(input);
        }

        [HttpPost]
        [AbpAuthorize(PermissionNames.Employee_EmployeeDetail_TabPersonalInfo_ChangeWorkingStatus_BackToWork)]
        public void ChangeStatusToWorking(ToWorkingDto input)
        {
            _changeEmployeeWorkingStatus.ChangeStatusToWorking(input);
        }

        [HttpPost]
        [AbpAuthorize(PermissionNames.Employee_EmployeeDetail_TabPersonalInfo_ChangeWorkingStatus_ExtendMaternityLeave)]
        public void ExtendMaternityLeave(ExtendWorkingStatusDto input)
        {
            _changeEmployeeWorkingStatus.ExtendManternityLeave(input);
        }
        [HttpPost]
        [AbpAuthorize(PermissionNames.Employee_EmployeeDetail_TabPersonalInfo_ChangeWorkingStatus_ExtendPausing)]
        public void ExtendPausing(ExtendWorkingStatusDto input)
        {
             _changeEmployeeWorkingStatus.ExtendPausing(input);
        }

        [HttpGet]
        public GetLatestSalaryChangeRequestDto GetLatestSalaryChangeRequest(long employeeId)
        {
           return  _changeEmployeeWorkingStatus.GetLatestSalaryChangeRequest(employeeId);
        }
    }
}
