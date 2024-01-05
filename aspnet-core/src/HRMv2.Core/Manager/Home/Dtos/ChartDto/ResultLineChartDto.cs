using NccCore.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMv2.Manager.Home.Dtos.ChartDto
{
    public class ResultLineChartDto
    {
        public IEnumerable<string> Labels { get; set; }
        public List<DataLineChartDto> Charts { get; set; } = new List<DataLineChartDto>();

    }
    
    public class DataLineChartDto
    {
        public string Name { get; set; }
        public ChartStyleDto ItemStyle { get; set; }
        public string Type { get; set; }
        public List<double> Data { get; set; }
        public string Total => Data.Sum().ToString();
        public int BarGap => 0;
        public string BarMaxWidth => "80";

    }

    public class ChartStyleDto
    {
        public string Color { get; set; }
    }
}
