using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMv2.Manager.Salaries.Dto
{
    public class GetEmployeeDetailDto
    {
        public CollectedDataDto CollectedData { get; set; }
        public StandardInformationDto StandardInformation { get; set; }
        public ContractSalaryDto ContractSalary { get; set; }
        public Result Result { get; set; }
    }
    public class CollectedDataDto
    {
        public double LeaveHoursBeforeCal { get; set; }
        public double MonthlyAddedLeaveHours { get; set; }
        public double NormalWorkingHours { get; set; }
        public double OpenTalkHours { get; set; }
        public double TotalHours { get; set; }
        public double OffHours { get; set; }
        public double OffHoursCalculated { get; set; }
        public double NonSalaryDeductedOffHours  { get; set; }
    }
    public class StandardInformationDto
    {
        public double StandardWorkingHour { get; set; }
        public double StandardOpentalkHour { get; set; }
        public double StandardTotalHour { get; set; }
    }
    public class ContractSalaryDto
    {
        public double Salary { get; set; }
        public DateTime FromDate { get; set; }
    }
    public class Result
    {
        public double NormalSalary { get; set; }
        public double OTSalary { get; set; }
        public double TotalBenefit { get; set; }
        public double TotalBonus { get; set; }
        public double TotalPunishment { get; set; }
        public double TotalDebt { get; set; }
        public double TotalRealSalary { get; set; }
        public double RemainingLeaveHours { get; set; }

    }
}
