using Abp.Application.Services.Dto;
using HRMv2.Manager.Common.Dto;
using NccCore.Helper;
using NccCore.Uitls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HRMv2.Constants.Enum.HRMEnum;

namespace HRMv2.Manager.Home.Dtos.ChartDto
{
    public class ResultLineChartDto : EntityDto<long>
    {
        public string ChartName { get; set; }
        public List<LineChartData> Lines { get; set; } = new List<LineChartData>();

    }
    
    public class LineChartData : EntityDto<long>
    {
        public string LineName { get; set; }
        public string Color { get; set; }
        public List<double> Data { get; set; }
        public int BarGap => 0;
        public string BarMaxWidth => "80";

    }
}
