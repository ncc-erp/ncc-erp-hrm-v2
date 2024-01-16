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
        public List<CircleChartData> Pies { get; set; } = new List<CircleChartData>();
    }

    public class CircleChartData : EntityDto<long>
    {
        public string ChartDetailName { get; set; }
        public string Color { get; set; }
        public double Data { get; set; }
    }
}
