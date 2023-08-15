using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using HRMv2.Entities;
using NccCore.Anotations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HRMv2.Constants.Enum.HRMEnum;

namespace HRMv2.Manager.Categories.Benefits.Dto
{
    [AutoMapTo(typeof(Benefit))]
    public class BenefitDto : EntityDto<long>
    {
        [Required]
        [ApplySearch]
        public string Name { get; set; }
        [Required]
        public double Money { get; set; }
        [Required]
        public BenefitType Type { get; set; }
        public bool IsActive { get; set; }
        public bool IsBelongToAllEmployee { get; set; }
        public DateTime? ApplyDate { get; set; }
    }
}
