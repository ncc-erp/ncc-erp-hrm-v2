using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMv2.Manager.Home.Dtos.ChartDto
{
    public class ResultCircleChartDto : EntityDto<long>
    {
        public string ChartName { get; set; }
        public List<DataCircleChartDetailDto> ChartDetails { get; set; } = new List<DataCircleChartDetailDto>();
    }

    public class DataCircleChartDetailDto : EntityDto<long>
    {
        public string ChartDetailName { get; set; }
        public ChartStyleDto ItemStyle { get; set; }
        public double Data { get; set; }
    }
}
