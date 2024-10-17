using DocumentFormat.OpenXml.Office.CoverPageProps;
using HRMv2.Entities;
using HRMv2.Manager.Common.Dto;
using HRMv2.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HRMv2.Constants.Enum.HRMEnum;

namespace HRMv2.Manager.Benefits.Dto
{
    public class GetEmployeeBenefitsDto
    {
        public long Id { get; set; }
        public string FullName { get; set; }
        public Sex Sex { get; set; }
        public string AvatarFullPath { get; set; }
        public string Email { get; set; }
        public BadgeInfoDto BranchInfo { get; set; }
        public BadgeInfoDto JobPositionInfo { get; set; }
        public BadgeInfoDto LevelInfo { get; set; }
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
        public List<GetBenefitsOfEmployeeDto> benefits { get; set; }
    }
}
