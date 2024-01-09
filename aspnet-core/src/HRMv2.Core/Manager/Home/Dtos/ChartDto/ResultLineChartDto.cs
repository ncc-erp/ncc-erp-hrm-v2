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

    public class EmployeeDetailDto
    {
        public long EmployeeId { get; set; }
        public string FullName { get; set; }
        public long LevelId { get; set; }
        public long JobPositionId { get; set; }
        public List<long> TeamIds { get; set; }
        public UserType UserType { get; set; }
        public Sex Gender { get; set; }
        public long BranchId { get; set; }
        public EmployeeStatus Status { get; set; }
        public DateTime Month { get; set; }

    }
}
