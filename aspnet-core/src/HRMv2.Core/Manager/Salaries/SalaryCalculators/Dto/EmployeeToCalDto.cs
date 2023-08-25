using HRMv2.Manager.Salaries.Payslips.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HRMv2.Constants.Enum.HRMEnum;

namespace HRMv2.Manager.Salaries.CalculateSalary.Dto
{
    public class EmployeeToCalDto: EmployeeWorkingStatusDto
    {
        public string NormalizeEmailAddress { get; set; }
        public UserType UserType { get; set; }
        public long LevelId { get; set; }
        public long BranchId { get; set; }
        public long JobPositionId { get; set; }
        public float RemainLeaveDay { get; set; }
        public long? BankId { get; set; }
        public string BankAccountNumber { get; set; }
        public string BankName { get; set; }
        public List<long> Teams { get; set; }
    }
}
