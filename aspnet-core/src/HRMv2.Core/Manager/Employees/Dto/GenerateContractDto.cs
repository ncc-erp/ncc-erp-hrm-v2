using HRMv2.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMv2.Manager.Employees.Dto
{
    public class GenerateContractDto
    {
        public Employee Employee { get; set; }
        public DateTime ContractStartDate { get; set; }
        public DateTime? ContractEndDate { get; set; }
        public long SalaryRequestemployeeId { get; set; }
    }
}
