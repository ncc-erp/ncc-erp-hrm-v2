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
    public class EditBonusDto : EntityDto<long>
    {
        [Required]
        public string Name { get; set; }
        public bool IsApply { get; set; }
        public bool IsActive { get; set; }
        public DateTime ApplyMonth { get; set; }
    }
}
