﻿using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using HRMv2.Entities;
using NccCore.Anotations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMv2.Manager.Categories.Levels.Dto
{
    [AutoMapTo(typeof(Level))]
    public class LevelDto : EntityDto<long>
    {
        [Required]
        [ApplySearch]
        public string Name { get; set; }
        [Required]
        [ApplySearch]
        public string ShortName { get; set; }
        [Required]
        [ApplySearch]
        public string Code { get; set; }
        public string Color { get; set; }
    }
}
