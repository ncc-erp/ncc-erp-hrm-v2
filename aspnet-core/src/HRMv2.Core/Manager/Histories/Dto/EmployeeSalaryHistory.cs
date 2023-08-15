using Abp.AutoMapper;
using Abp.Domain.Entities;
using Castle.MicroKernel.SubSystems.Conversion;
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
    public class EmployeeSalaryHistoryDto : Entity<long>
    {
        public long EmployeeId { get; set; }
        public double FromSalary { get; set; }
        public double ToSalary { get; set; }
        public UserType FromUserType { get; set; }
        public UserType ToUserType { get; set; }
        public long FromJobPositionId { get; set; }
        public long ToJobPositionId { get; set; }
        public long FromLevelId { get; set; }
        public BadgeInfoDto FromLevelInfo { get; set; }
        public long ToLevelId { get; set; }
        public BadgeInfoDto ToLevelInfo { get; set; }
        public DateTime ApplyDate { get; set; }
        public DateTime CreationTime { get; set; }
        public string ContractCode { get; set; }
        public string Note { get; set; }
        public DateTime? UpdatedTime { get; set; }
        public string UpdatedUser { get; set; }
        public bool IsNotAllowToDelete { get; set; }
        public ChangeRequestInfoDto Request { get; set; } = new();
        public SalaryRequestType Type { get; set; }
        public bool HasContract { get; set; }
        public string TypeName 
        { 
            get 
            { 
                return Enum.GetName(typeof(SalaryRequestType), Type); 
            } 
        }

        public BadgeInfoDto FromUserTypeInfo
        {
            get
            {
                return new BadgeInfoDto
                {
                    Name = CommonUtil.GetUserTypeNameVN(FromUserType),
                    Color = CommonUtil.GetUserType(FromUserType).Color
                };
            }
        }
        public BadgeInfoDto ToUserTypeInfo
        {
            get
            {
                return new BadgeInfoDto
                {
                    Name = CommonUtil.GetUserTypeNameVN(ToUserType),
                    Color = CommonUtil.GetUserType(ToUserType).Color
                };
            }
        }
    }


    public class ChangeRequestInfoDto
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public SalaryRequestStatus Status { get; set; }

    }


}
