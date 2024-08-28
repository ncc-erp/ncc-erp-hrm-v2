using HRMv2.Manager.WorkingHistories.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMv2.Manager.Home.Dtos
{
    public class OnboardQuitEmployeesToExportDto
    {
        public List<LastEmployeeWorkingHistoryDto> OnboardEmployees { get; set; }
        public List<LastEmployeeWorkingHistoryDto> QuitEmployees { get; set; }
        public List<LastEmployeeWorkingHistoryDto> OnboardAndQuitEmployees { get; set; }
        public int OnboardTotal => OnboardEmployees.Count;
        public int QuitTotal => QuitEmployees.Count;       
    }
}
