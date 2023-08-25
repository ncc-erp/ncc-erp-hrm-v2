using HRMv2.Constants.Enum;
using HRMv2.Manager.Common.Dto;
using HRMv2.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HRMv2.Manager.WorkingHistories.Dtos
{
    public class LastEmployeeWorkingHistoryDto
    {
        /// <summary>
        /// DateAt of the last working history
        /// </summary>
        public DateTime DateAt { get; set; }
        public HRMEnum.EmployeeStatus LastStatus { get; set; }

        /// <summary>
        /// List All Working History of employee order by Date At desc
        /// </summary>
        public List<StatusDateAtDto> WorkingHistories { get; set; }

        private bool IsOnboardInTimeSpan => WorkingHistoriesInTimeSpan.Any(s => s.Status == HRMEnum.EmployeeStatus.Working);
        private bool IsQuitInTimeSpan => WorkingHistoriesInTimeSpan.Any(s => s.Status == HRMEnum.EmployeeStatus.Quit);
        public bool IsOnboardAndQuitInTimeSpan => IsOnboardInTimeSpan && IsQuitInTimeSpan;


        /// <summary>
        /// List All Working History in time span( startDate, end date) of employee order by Date At desc
        /// </summary>
        public List<StatusDateAtDto> WorkingHistoriesInTimeSpan { get; set; }
        public long EmployeeId { get; set; }
        public long BranchId { get; set; }
        public HRMEnum.UserType UserType { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Avatar { get; set; }
        public HRMEnum.Sex Sex { get; set; }
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
        public BadgeInfoDto JobPositionInfo { get; set; }

        public BadgeInfoDto LevelInfo { get; set; }

        public BadgeInfoDto BranchInfo { get; set; }
    }

    public class StatusDateAtDto
    {
        public DateTime DateAt { get; set; }
        public HRMEnum.EmployeeStatus Status { get; set; }
    }

}
