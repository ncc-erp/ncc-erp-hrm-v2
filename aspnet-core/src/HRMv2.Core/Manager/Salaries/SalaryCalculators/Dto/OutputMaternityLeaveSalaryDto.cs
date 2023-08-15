using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMv2.Manager.Salaries.SalaryCalculators.Dto
{
    public class OutputMaternityLeaveSalaryDto
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public double InputSalary { get; set; }
        public double OutputSalary { get; set; }        
    }
}
