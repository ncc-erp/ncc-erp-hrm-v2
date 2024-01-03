﻿using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using HRMv2.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HRMv2.Constants.Enum.HRMEnum;

namespace HRMv2.Manager.Charts.Dto
{
    [AutoMapTo(typeof(Chart))]
    public class UpdateChartDto : EntityDto<long>
    {
        public string Name { get; set; }

        public ChartType? ChartType { get; set; }

        public ChartDataType? DataType { get; set; }

        public TimePeriodType? TimePeriodType { get; set; }

        public bool? IsActive { get; set; }
    }
}
