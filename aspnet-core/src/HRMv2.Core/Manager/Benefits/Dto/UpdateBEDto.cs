using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using HRMv2.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMv2.Manager.Benefits.Dto
{
    [AutoMapTo(typeof(BenefitEmployee))]
    public class UpdateBEDto:EntityDto<long>
    {
        public long BenefitId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public long EmployeeId { get; set; }
    }
}
