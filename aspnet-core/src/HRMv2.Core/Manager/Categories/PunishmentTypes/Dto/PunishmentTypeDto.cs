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

namespace HRMv2.Manager.Categories.PunishmentTypes.Dto
{
    [AutoMapTo(typeof(PunishmentType))]
    public class PunishmentTypeDto : EntityDto<long>
    {
        [Required]
        [ApplySearch]
        public string Name { get; set; }
        public bool IsActive { get; set; }
        public string Api { get; set; }
    }
}
