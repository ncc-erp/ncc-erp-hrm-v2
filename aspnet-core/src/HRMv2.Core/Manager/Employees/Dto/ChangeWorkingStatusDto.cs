using HRMv2.Manager.Benefits.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HRMv2.Constants.Enum.HRMEnum;

namespace HRMv2.Manager.Employees.Dto
{
    public class ChangeWorkingStatusDto
    {
        public long EmployeeId { get; set; }
        public long LevelId { get; set; }
        public UserType UserType { get; set; }
        public long JobPositionId { get; set; }
        public long ToLevelId { get; set; }
        public UserType ToUserType { get; set; }
        public long ToJobPositionId { get; set; }
        public long ToSalary { get; set; }
        public long BasicSalary { get; set; }
        public bool HasContract { get;set;}

        public EmployeeStatus Status { get; set; }
        public EmployeeStatus ToStatus { get; set; }

        public long Salary { get; set; }
        public DateTime ApplyDate {get;set;}
        public DateTime BackDate {get;set;}
        public string Note { get; set; }

        public DateTime? ContractEndDate { get; set; }
        public double ProbationPercentage { get; set; }

        public List<GetBenefitsOfEmployeeDto> ListCurrentBenefits { get; set; }

    }
}
