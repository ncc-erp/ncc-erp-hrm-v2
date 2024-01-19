using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HRMv2.Constants.Enum.HRMEnum;

namespace HRMv2.Manager.Home.Dtos.ChartDto
{
    public class ResultChartDto 
    {
        public ChartDataType ChartDataType { get; set; }
        public List<ResultCircleChartDto> CircleCharts { get; set; } = new List<ResultCircleChartDto>();
        public List<ResultLineChartDto> LineCharts { get; set; } = new List<ResultLineChartDto>();
    }
}
