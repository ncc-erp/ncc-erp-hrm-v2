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
        public string ChartName { get; set; }
        public ChartType ChartType { get; set; }
        public List<CircleChartData> Pies { get; set; } = new List<CircleChartData>();
        public List<LineChartData> Lines { get; set; } = new List<LineChartData>();
    }
}
