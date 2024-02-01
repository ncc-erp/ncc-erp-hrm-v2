using HRMv2.Manager.Common.Dto;
using HRMv2.Utils;
using NccCore.Uitls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HRMv2.Constants.Enum.HRMEnum;

namespace HRMv2.Manager.Categories.Charts.ChartDetails.Dto
{
    public class EmployeeDataDto
    {
        public long Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }

        public Sex Gender { get; set; }
        public BadgeInfoDto LevelInfo { get; set; }
        public BadgeInfoDto BranchInfo { get; set; }
        public BadgeInfoDto JobPositionInfo { get; set; }
        public List<EmployeeTeamDto> Teams { get; set; }
        public UserType UserType { get; set; }
        public BadgeInfoDto UserTypeInfo
        {
            get
            {
                return new BadgeInfoDto
                {
                    Name = CommonUtil.GetUserTypeNameVN(UserType),
                    Color = CommonUtil.GetUserType(UserType).Color
                };
            }
        }

        //employee
        public EmployeeMonthlyStatus Status { get; set; }
        public DateTime Month { get; set; }
        public string MonthYear => Month.ToString("MM-yyyy");
        //salary
        public long PayrollId { get; set; }
        public double Money { get; set; }
    }
}
