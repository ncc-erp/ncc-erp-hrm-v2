using HRMv2.Manager.Categories.Charts.DisplayChartDto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMv2.Manager.Categories.Charts.ChartDetails.Dto
{
    public class DataFromChartDetailDto
    {
        public string ChartName { get; set; }
        public string ChartDetailName { get; set; }
        public List<EmployeeDataFromChartDetailDto> Employees { get; set; }
    }
}
