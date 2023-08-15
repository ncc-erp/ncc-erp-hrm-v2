using Abp.AutoMapper;
using Abp.Domain.Entities;
using HRMv2.Entities;
using HRMv2.Manager.Common.Dto;
using HRMv2.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HRMv2.Constants.Enum.HRMEnum;

namespace HRMv2.Manager.Histories.Dto
{
    public class EmployeePayslipHistoryDto: BaseEmployeeDto
    {
        public long EmployeeId { get; set; }
        public long PayslipId { get; set; }
        public List<StandardSalaryDto> StandardSalary { get; set; }
        public double RealSalary { get; set; }
        public double NormalSalary { get; set; }
        public double RemainLeaveHour { get; set; }
        public double OTSalary { get; set; }
        public double Benefit { get; set; }
        public double Bonus { get; set; }
        public double Punishment { get; set; }
        public double Debt { get; set; }
        public float RemainLeaveDayBefore { get; set; }
        public float RemainLeaveDayAfter { get; set; }
        public DateTime? UpdatedTime { get; set; }
        public string UpdatedUser { get; set; }
        public string Note { get; set; }
        public DateTime ApplyMonth { get; set; }
    
        public UserType PayslipUserType { get; set; }
        public BadgeInfoDto PayslipUserTypeInfo
        {
            get
            {
                return new BadgeInfoDto
                {
                    Name = CommonUtil.GetUserType(PayslipUserType).Name,
                    Color = CommonUtil.GetUserType(PayslipUserType).Color
                };
            }
        }

        public BadgeInfoDto PayslipBranchInfo { get; set; }
        public BadgeInfoDto PayslipLevelInfo { get;set; }
        public BadgeInfoDto PayslipJobPositionInfo { get; set; } 
        public PayrollInfoDto PayrollInfo { get; set; }
    }


    public class StandardSalaryDto
    {
        public double Salary { get; set; }
        public DateTime Date { get; set; }

        public static implicit operator List<object>(StandardSalaryDto v)
        {
            throw new NotImplementedException();
        }
    }

    public class PayslipSalaryTypeDto
    {
        public PayslipDetailType Type { get; set; }
        public long PayslipId { get; set; }
    }

    public class PayrollInfoDto
    {
        public long PayrollId { get; set; }
        public PayrollStatus PayrollStatus { get; set; }
    }
}
