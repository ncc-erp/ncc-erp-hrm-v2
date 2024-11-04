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
    public class CreateSalaryChangeRequestFromCheckpointDto : Entity<long>
    {
        public string Name { get; set; }
        public DateTime ApplyMonth { get; set; }
        public List<EmployeeSalaryChangeRequestDto> RequestChangeSalaryEmployee { get ; set; }

    }
    public class EmployeeSalaryChangeRequestDto 
    {
        public string EmailAddress { get; set; }

        public string EmailAddressToLowerTrim => EmailAddress.ToLower().Trim();
        public string ToLevelCode { get; set; }
        public double SalaryIncrease { get; set; }
        public bool HasContract { get; set; }
        public UserType ToUserType { get; set; }
        public DateTime ApplyDate { get; set; }
        public string ToJobPositionCode { get; set; }

        public string Note { get; set; }

    }
}
