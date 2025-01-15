using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace HRMv2.Entities
{
    public class EmployeeSalaryInfo
    {
        public string Email { get; set; }
        [JsonIgnore]
        public SalaryInfo SalaryInfo { get; set; }
        public double Salary => Math.Max(SalaryInfo.ToSalary,SalaryInfo.ContractRealSalary);
    }
}
