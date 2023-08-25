using HRMv2.Entities;
using HRMv2.Manager.CheckIn.Dto;
using HRMv2.NccCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMv2.Manager.CheckIn
{
    public class CheckInManager : BaseManager
    {
        public CheckInManager(IWorkScope workScope) : base(workScope)
        {
        }

        public List<GetUserForCheckInDto> GetUserForCheckIn()
        {
            var listUsers = WorkScope.GetAll<Employee>()
                .Where(s => s.Status == Constants.Enum.HRMEnum.EmployeeStatus.Working)
                .Select(x=> new GetUserInfo
                {
                    Email = x.Email,
                    FullName = x.FullName,                    
                })
                .Select(x=> new GetUserForCheckInDto
                {
                    Email = x.Email,
                    FirstName = x.FirstName,
                    LastName = x.LastName,
                })
                .ToList();
            return listUsers;
        }

         
    }
}
