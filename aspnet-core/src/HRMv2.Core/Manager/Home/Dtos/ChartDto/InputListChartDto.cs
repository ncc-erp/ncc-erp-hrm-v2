using System;
using System.Collections.Generic;
using static HRMv2.Constants.Enum.HRMEnum;

namespace HRMv2.Manager.Home.Dtos.ChartDto
{
    public class InputListChartDto
    {
        public List<long> ChartIds { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
    public class InputChartDetailDto
    {
        public long ChartDetailId { get; set; }
        public ChartDataType ChartDataType { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }

}
