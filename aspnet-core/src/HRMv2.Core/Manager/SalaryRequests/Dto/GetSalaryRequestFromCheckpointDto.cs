using Abp.AutoMapper;
using Abp.Domain.Entities;
using HRMv2.Authorization.Users;
using HRMv2.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMv2.Manager.SalaryRequests.Dto
{
    public class GetSalaryRequestFromCheckpointDto : Entity<long>
    {
        public string Name { get; set; }
        public DateTime ApplyMonth { get; set; }
        public List<EmployeeInRequestChageSalaryDto> RequestChangeSalaryEmployee { get ; set; }

    }
    public class ResultSendChageRequest
    {
        public string EmaillAddress { get; set; }

        public string SyncNote { get; set; }
    }
    public class EmployeeInRequestChageSalaryDto
    {
        public string EmailAddress { get; set; }
        public double ToSlary { get; set; }
        public double Salary { get; set; }
        public string Note { get; set; }

    }
}
