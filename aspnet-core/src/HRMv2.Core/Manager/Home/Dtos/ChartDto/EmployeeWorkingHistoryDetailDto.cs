using HRMv2.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HRMv2.Constants.Enum.HRMEnum;

namespace HRMv2.Manager.Home.Dtos.ChartDto
{
    public class EmployeeWorkingHistoryDetailDto
    {
        public long EmployeeId { get; set; }
        public Employee Employee { get; set; }
        public EmployeeStatus Status { get; set; }
        public DateTime DateAt { get; set; }
    }
}
