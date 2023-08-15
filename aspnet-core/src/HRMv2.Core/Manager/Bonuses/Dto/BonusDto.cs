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

namespace HRMv2.Manager.Bonuses.Dto
{
    [AutoMapTo(typeof(Bonus))]
    public class BonusDto : EntityDto<long>
    {
        [Required]
        [ApplySearch]
        public string Name { get; set; }
        public DateTime ApplyMonth { get; set; }
        public bool IsActive { get; set; }
    }
}
