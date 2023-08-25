using Abp.Application.Services.Dto;
using NccCore.Anotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HRMv2.Constants.Enum.HRMEnum;

namespace HRMv2.Manager.Benefits.Dto
{
    public class GetBenefitsOfEmployeeDto: EntityDto<long>
    {
        [ApplySearch]
        public string BenefitName { get; set; }
        public long BenefitId { get; set; }
        public BenefitType BenefitType { get; set; }
        public double Money { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool Status { get; set; }
    }
}
