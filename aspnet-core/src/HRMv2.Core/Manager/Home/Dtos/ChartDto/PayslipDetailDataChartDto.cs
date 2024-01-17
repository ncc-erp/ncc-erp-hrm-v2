﻿using Abp.Application.Services.Dto;
using HRMv2.Entities;
using NccCore.Uitls;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HRMv2.Constants.Enum.HRMEnum;

namespace HRMv2.Manager.Home.Dtos.ChartDto
{
    public class PayslipDetailDataChartDto : EntityDto<long>
    {
        public long PayslipId { get; set; }
        public double Money { get; set; }
        public PayslipDetailType Type { get; set; }
        public DateTime ApplyMonth { get; set; } // Payroll ApplyMonth 
        public string MonthYear => DateTimeUtils.GetMonthYearLabelChart(ApplyMonth);
        public PayslipDataChartDto Payslip { get; set; }
    }
}
