using HRMv2.Manager.Common.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMv2.Manager.Salaries.Dto
{
    public class GetsalaryDetailDto:BaseEmployeeDto
    {
        public long EmployeeId { get; set; }
        public double RealSalary { get; set; }
        public double RemainLeaveHour { get; set; }
    }
}
