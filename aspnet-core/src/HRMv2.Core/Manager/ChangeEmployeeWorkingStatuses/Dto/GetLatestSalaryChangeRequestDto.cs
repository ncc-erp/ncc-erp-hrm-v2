using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HRMv2.Constants.Enum.HRMEnum;

namespace HRMv2.Manager.ChangeEmployeeWorkingStatuses.Dto
{
    public class GetLatestSalaryChangeRequestDto
    {
        public long? SalaryChangeRequestId { get; set; }
        public long EmployeeId { get; set; }
        public long ToLevelId { get; set; }
        public UserType ToUserType { get; set; }
        public long ToJobPositionId { get; set; }
        public double RealSalary { get; set; }
        public double BasicSalary { get; set; }
        public double ProbationPercentage { get; set; }
        public DateTime ApplyDate { get; set; }
        public string Note { get; set; }
        public SalaryRequestType Type { get; set; }
        public bool HasContract { get; set; }
        public string TypeName => Enum.GetName(typeof(SalaryRequestType), Type);

        public DateTime? ContractEndDate { get; set; }

    }
}
