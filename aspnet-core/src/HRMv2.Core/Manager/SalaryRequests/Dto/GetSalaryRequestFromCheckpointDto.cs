using Abp.AutoMapper;
using Abp.Domain.Entities;
using HRMv2.Authorization.Users;
using HRMv2.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HRMv2.Constants.Enum.HRMEnum;

namespace HRMv2.Manager.SalaryRequests.Dto
{
    public class GetSalaryRequestFromCheckpointDto : Entity<long>
    {
        public string Name { get; set; }
        public DateTime ApplyMonth { get; set; }
        public List<EmployeeInRequestChageSalaryDto> RequestChangeSalaryEmployee { get ; set; }

    }
    public class ResultSendChageRequestDto
    {
        public string EmaillAddress { get; set; }
        public string SyncNote { get; set; }
    }
    public class EmployeeInRequestChageSalaryDto
    {
        public long ToLevelId { get; set; }
        public long ToJobPositionId { get; set; }
        public UserType ToUserType { get; set; }
        public string EmailAddress { get; set; }
        public double Slary { get; set; }
        public double ToSlary { get; set; }
        public string Note { get; set; }

    }
}
