using HRMv2.Manager.Common.Dto;
using NccCore.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HRMv2.Constants.Enum.HRMEnum;

namespace HRMv2.Manager.Home.Dtos.ChartDto
{
    public class ResultCircleChartDto
    {
        public string ChartName { get; set; }
        public IEnumerable<string> Labels { get; set; }
        public List<DataLineChartDto> ChartDetails { get; set; } = new List<DataLineChartDto>();

    }

}
