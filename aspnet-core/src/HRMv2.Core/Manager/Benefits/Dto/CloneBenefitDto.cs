using Abp.AutoMapper;
using HRMv2.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HRMv2.Constants.Enum.HRMEnum;

namespace HRMv2.Manager.Benefits.Dto
{
    [AutoMapTo(typeof(Benefit))]
    public class CloneBenefitDto
    {
        public long BenefitId { get; set; }
        public string Name { get; set; }
        public double Money { get; set; }
        public BenefitType Type { get; set; }
        public bool IsActive { get; set; }
        public bool IsBelongToAllEmployee { get; set; }
        public bool IsCloneEmployee { get; set; }
        public DateTime ApplyDate { get; set; }
    }
}
