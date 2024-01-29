using Abp.Application.Services.Dto;
using HRMv2.Entities;
using HRMv2.Manager.Categories.Charts.ChartDetails.Dto;
using HRMv2.Manager.Common.Dto;
using HRMv2.Utils;
using NccCore.Uitls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HRMv2.Constants.Enum.HRMEnum;

namespace HRMv2.Manager.Home.Dtos.ChartDto
{
    public class PayslipDataChartDto : EntityDto<long>
    {
        public long EmployeeId { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Avatar { get; set; }
        public string AvatarFullPath => FileUtil.FullFilePath(Avatar);

        public Sex Gender { get; set; }
        public long BranchId { get; set; }
        public long JobPositionId { get; set; }
        public long LevelId { get; set; }
        public List<long> TeamIds { get; set; }
        public UserType UserType { get; set; }
        //employee
        public EmployeeMonthlyStatus MonthlyStatus { get; set; }
        public EmployeeStatus Status { get; set; }
        public DateTime DateAt { get; set; }
        public DateTime StatusMonth => DateTimeUtils.FirstDayOfMonth(DateAt);
        public string StatusMonthYear => DateTimeUtils.GetMonthYearLabelChart(StatusMonth);
        //salary
        public long PayrollId { get; set; }
        public double Salary { get; set; }
        public DateTime PayrollMonth { get; set; }
        public string PayrollMonthYear => DateTimeUtils.GetMonthYearLabelChart(PayrollMonth);
        public double Money { get; set; }

        public List<PayslipDetailDataChartDto> PayslipDetails {  get; set; }


        public List<KeyValueDto> TeamInfos { get; set; }
        public BadgeInfoDto LevelInfo { get; set; }
        public BadgeInfoDto BranchInfo { get; set; }
        public BadgeInfoDto JobPositionInfo { get; set; }
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
    }

    public class PayslipDetailDataChartDto : EntityDto<long>
    {
        public double Money { get; set; }
        public PayslipDetailType Type { get; set; }
    }

    public class BadgeInfoChartDetail
    {
        public List<KeyValueDto> TeamInfos { get; set; }
        public List<BadgeInfoDto> LevelInfo { get; set; }
        public List<BadgeInfoDto> BranchInfo { get; set; }
        public List<BadgeInfoDto> JobPositionInfo { get; set; }
    }
}
