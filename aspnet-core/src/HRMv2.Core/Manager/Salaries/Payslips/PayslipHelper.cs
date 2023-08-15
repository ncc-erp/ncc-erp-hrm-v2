using HRMv2.Manager.Salaries.CalculateSalary.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HRMv2.Constants.Enum.HRMEnum;

namespace HRMv2.Manager.Salaries.Payslips
{
    public class PayslipHelper
    {

        public static List<SalaryInputForPayslipDto> GetListSalaryInputForPayslipDto(List<SalaryInputForPayslipDto> listSalary, DateTime firstDayOfPayroll, DateTime lastDayOfPayroll)
        {
            var listSalaryResult = new List<SalaryInputForPayslipDto>();

            foreach (var salaryItem in listSalary)
            {
                if (salaryItem.Date > lastDayOfPayroll) continue;

                var lastSalaryResult = listSalaryResult.LastOrDefault();
                if (salaryItem.Date >= firstDayOfPayroll || lastSalaryResult == default)
                {
                    listSalaryResult.Add(salaryItem);
                    continue;
                }
                //nghi sinh
                if (
                    (lastSalaryResult?.SalaryType == SalaryRequestType.BackToWork
                        && salaryItem.SalaryType == SalaryRequestType.MaternityLeave
                        && salaryItem.Date.AddMonths(6) > firstDayOfPayroll)
                    
                    || (lastSalaryResult?.SalaryType == SalaryRequestType.StopWorking
                        && lastSalaryResult?.Date >= firstDayOfPayroll)
                    
                    || (lastSalaryResult?.UserType == UserType.Staff
                        && lastSalaryResult?.Date >= firstDayOfPayroll
                        && salaryItem.UserType == UserType.ProbationaryStaff)

                     || ((lastSalaryResult?.SalaryType == SalaryRequestType.Change || lastSalaryResult?.SalaryType == SalaryRequestType.MaternityLeave)
                        && lastSalaryResult?.Date > firstDayOfPayroll
                        && salaryItem.SalaryType != SalaryRequestType.StopWorking)
                    )
                {
                    listSalaryResult.Add(salaryItem);   
                   
                }

                break;

            }
            return listSalaryResult;
        }
    }
}
