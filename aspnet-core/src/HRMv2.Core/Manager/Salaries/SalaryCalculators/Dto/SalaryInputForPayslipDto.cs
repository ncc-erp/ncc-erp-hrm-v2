using Abp.AutoMapper;
using HRMv2.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HRMv2.Constants.Enum.HRMEnum;

namespace HRMv2.Manager.Salaries.CalculateSalary.Dto
{
    [AutoMapTo(typeof(PayslipSalary))]
    public class SalaryInputForPayslipDto
    {
        public long EmployeeId { get; set; }
        public double Salary { get; set; }
        public DateTime Date { get; set; }
        public UserType UserType { get; set; }
        public SalaryRequestType SalaryType { get; set; }
    }
}
