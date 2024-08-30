using HRMv2.Manager.Categories.Charts.DisplayChartDto;
using System.Collections.Generic;

namespace HRMv2.Manager.Home.Dtos
{
    public class DataFromChartDetailDto
    {
        public string ChartName { get; set; }
        public string ChartDetailName { get; set; }
        public List<EmployeeDataFromChartDetailDto> Employees { get; set; }
    }
}
