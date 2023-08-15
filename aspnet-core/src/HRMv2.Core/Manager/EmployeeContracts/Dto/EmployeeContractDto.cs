using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HRMv2.Entities;
using HRMv2.Manager.Common.Dto;
using HRMv2.Utils;
using HRMv2.Constants;
using static HRMv2.Constants.Enum.HRMEnum;

namespace HRMv2.Manager.EmployeeContracts
{
    [AutoMapTo(typeof(EmployeeContract))]
    public class EmployeeContractDto:EntityDto<long>{
        public long EmployeeId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string File { get; set; }
        public string FullFilePath => FileUtil.FullFilePath(File);
        public string FileName => FileUtil.GetFileName(File);
        public string Code { get; set; }
        public UserType UserType { get; set; }
        public long JobPositionId { get; set; }
        public long LevelId { get; set; }
        public BadgeInfoDto LevelInfo { get; set; }
        public BadgeInfoDto JobPositionInfo { get; set; }
        public double BasicSalary { get; set; }
        public double RealSalary { get; set; }
        public double ProbationPercentage { get; set; }
        public DateTime CreationTime { get; set; }
        public string FilePath { get; set; }
        public long? SalaryRequestEmployeeId { get; set; }
        public string Note { get; set; }
        public RequestInfoDto Request { get; set; } = new();
        public string CreatorUserFullName { get; set; }
        public string LastModifierUserFullName { get; set; }
        public DateTime? LastModifierTime { get; set; }
        public string UpdatedUser => !string.IsNullOrEmpty(LastModifierUserFullName) ? LastModifierUserFullName : CreatorUserFullName;
        public DateTime? UpdatedTime => LastModifierTime.HasValue ? LastModifierTime : CreationTime;
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

    public class RequestInfoDto
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public SalaryRequestStatus Status { get; set; }
    }

    public class EmployeeToUpdateContractDto: EmployeeContractDto
    {
        public long EmployeeId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Code { get; set; }
    }

    public class EmployeeContractBasicInfo
    {
        public long EmployeeId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }

    public class EmployeeContractForExportDto
    {
        public long EmployeeId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public double BasicSalary { get; set; }
        public double ProbationPercentage { get; set; }
        public string Code { get;set; }
        public string JobPositionName { get; set; }
    }

}
