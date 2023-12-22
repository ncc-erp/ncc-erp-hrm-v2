﻿using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using HRMv2.Entities;
using NccCore.Anotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HRMv2.Constants.Enum.HRMEnum;

namespace HRMv2.Manager.Charts.Dto
{
    [AutoMap(typeof(Chart))]
    public class ChartDto : EntityDto<long>
    {
        [ApplySearch]
        public string Name { get; set; }

        public ChartType ChartType { get; set; }

        public TimePeriodType TimePeriodType { get; set; }

        public bool IsActive { get; set; }

        public string ChartTypeName => Enum.GetName(typeof(ChartType), ChartType);

        public string TimePeriodTypeName => Enum.GetName(typeof(TimePeriodType), TimePeriodType);
    }
}
