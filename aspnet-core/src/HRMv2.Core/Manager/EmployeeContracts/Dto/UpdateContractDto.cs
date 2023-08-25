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

namespace HRMv2.Manager.EmployeeContracts.Dto
{
        [AutoMapTo(typeof(EmployeeContract))]
        public class UpdateContractDto:EntityDto<long>
        {
            public DateTime StartDate { get; set; }
            public DateTime? EndDate { get; set; }
            public string Code { get; set; }
            public UserType UserType { get; set; }
            public long JobPositionId { get; set; }
            public long LevelId { get; set; }
            public double BasicSalary { get; set; }
            public double RealSalary { get; set; }
            public double ProbationPercentage { get; set; }
        }
}
