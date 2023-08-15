using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using HRMv2.Entities;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HRMv2.Constants.Enum.HRMEnum;

namespace HRMv2.Manager.SalaryRequests.Dto
{
    [AutoMapTo(typeof(SalaryChangeRequestEmployee))]
    public class AddOrUpdateEmployeeRequestDto: EntityDto<long>
    {
        public long? SalaryChangeRequestId { get; set; }
        public long EmployeeId { get; set; }
        public long LevelId { get; set; }
        public long ToLevelId { get; set; }
        public UserType FromUserType { get; set; }
        public UserType ToUserType { get; set; }
        public long JobPositionId { get; set; }
        public long ToJobPositionId { get; set; }
        public double Salary { get; set; }
        public double ToSalary { get; set; }
        public DateTime ApplyDate { get; set; }
        public DateTime? ContractEndDate { get; set; }
        public double ProbationPercentage { get; set; }
        public double BasicSalary { get; set; }
        public string Note { get; set; }
        public string ContractCode { get; set; }
        public bool HasContract { get; set; }
        public SalaryRequestType? Type { get; set; }
        public int? TenantId { get; set; }
    }
}
