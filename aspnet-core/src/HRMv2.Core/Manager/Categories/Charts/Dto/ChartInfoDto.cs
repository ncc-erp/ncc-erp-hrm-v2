﻿using HRMv2.Manager.Categories.Charts.ChartDetails.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMv2.Manager.Categories.Charts.Dto
{
    public class ChartInfoDto : ChartDto
    {
        public List<ChartDetailDto> Details { get; set; }

    }
}