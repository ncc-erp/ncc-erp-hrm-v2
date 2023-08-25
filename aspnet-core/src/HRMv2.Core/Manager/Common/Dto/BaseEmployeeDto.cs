using Abp.Application.Services.Dto;
using HRMv2.Utils;
using NccCore.Anotations;
using System;
using System.Collections.Generic;
using static HRMv2.Constants.Enum.HRMEnum;

namespace HRMv2.Manager.Common.Dto
{
    public class BaseEmployeeDto : EntityDto<long>
    {
        [ApplySearch]
        public string FullName { get; set; }
        [ApplySearch]
        public string Email { get; set; }
        public Sex Sex { get; set; }
        public EmployeeStatus Status { get; set; }
        public UserType UserType { get; set; }
        public long LevelId { get; set; }
        public BadgeInfoDto LevelInfo { get; set; }
        public long BranchId { get; set; }
        public BadgeInfoDto BranchInfo { get; set; }
        public long JobPositionId { get; set; }
        public BadgeInfoDto JobPositionInfo { get; set; }
        public string Avatar { get; set; }
        public List<EmployeeSkillDto> Skills { get; set; }
        public List<EmployeeTeamDto> Teams { get; set; }
        public string AvatarFullPath => FileUtil.FullFilePath(Avatar);
        public string UserTypeName => Enum.GetName(typeof(UserType), UserType);
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


    public class BadgeInfoDto
    {
        public string Name { get; set; }
        public string Color { get; set; }
    }
}