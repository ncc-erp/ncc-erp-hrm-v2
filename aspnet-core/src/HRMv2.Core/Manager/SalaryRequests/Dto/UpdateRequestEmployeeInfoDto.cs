using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using HRMv2.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HRMv2.Constants.Enum.HRMEnum;

namespace HRMv2.Manager.SalaryRequests.Dto
{
    [AutoMapTo(typeof(SalaryChangeRequestEmployee))]
    public class UpdateRequestEmployeeInfoDto :EntityDto<long>
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
        public string Note { get; set; }
        public SalaryRequestType Type { get; set; }
        public bool HasContract { get; set; }
    }
}
