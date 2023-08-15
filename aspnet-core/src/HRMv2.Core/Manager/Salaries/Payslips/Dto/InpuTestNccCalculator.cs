using Abp.AutoMapper;
using HRMv2.Manager.Salaries.CalculateSalary.Dto;
using HRMv2.Manager.Salaries.SalaryCalculators;
using HRMv2.Manager.Salaries.SalaryCalculators.Dto;
using HRMv2.WebServices.Timesheet.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HRMv2.Constants.Enum.HRMEnum;

namespace HRMv2.Manager.Salaries.Payslips.Dto
{
    public class InpuTestNccCalculator
    {
        public PayrollDto InputPayroll { get; set; }
        public EmployeeToCalDto InputEmployee { get; set; }
        public HashSet<DateTime> InputSettingOffDates { get; set; }
        public List<OffDateDto> InputOffDates { get; set; }
        public List<OffDateDto> InputOffDateLastMonth { get; set; }
        public HashSet<DateTime> InputWorkAtHomeOnlyDates { get; set; }
        public List<DateTime> InputOpenTalkDates { get; set; }
        public List<DateOTHourDto> InputOTs { get; set; }
        public List<SalaryInputForPayslipDto> InputSalaries { get; set; }//orderby applydate desc
        public List<CollectBenefitForPayslipDetailDto> InputBenefits { get; set; }
        public List<InputPayslipDetailDto> InputBonuses { get; set; }
        public List<InputPayslipDetailDto> InputPunishments { get; set; }
        public List<InputPayslipDetailDto> InputDebts { get; set; }
        public float NormalWorkingDay { get; set; }
        public float OpenTalkCount { get; set; }
        public float OffDay { get; set; }
        public float OtHour { get; set; }
        public float RefunleaveDay { get; set; }
        public float MonthlyAddedLeaveDay { get; set; }
        public float LeaveDayAfter { get; set; }
        public float WorkAtOfficeOrOnsiteDay { get; set; }

        public double NormalSalary { get; set; }
        public double OtSalary { get; set; }
        public double MaternityLeavesalary { get; set; }
        public double TotalBenefit { get; set; }
        public double TotalBonus { get; set; }
        public double TotalPunishment { get; set; }
        public double TotalDebt { get; set; }
        public double TotalRealSalary { get; set; }
    }

    public class CalculateResult
    {
        public double NormalSalary { get; set; }
        public double OTsalary { get; set; }
        public double MaternityLeavesalary { get; set; }

        public float NormalDay { get; set; }
        public float OpentalkCount { get; set; }
        public float OffDay { get; set; }
        public float OTHour { get; set; }
        public float RefundLeaveDay { get; set; }
        public float AddedLeaveDay { get; set; }
        public float LeaveDayAfter { get; set; }
        public float WorkAtOfficeOrOnsiteDay { get; set; }

        public double TotalBenefit { get; set; }
        public double TotalBonus { get; set; }
        public double TotalPunishment { get; set; }
        public double TotalDebt { get; set; }
        public double TotalRealSalary { get; set; }
       
    }
}




