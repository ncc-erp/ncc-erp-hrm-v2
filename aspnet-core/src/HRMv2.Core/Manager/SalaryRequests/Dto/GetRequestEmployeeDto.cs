using Abp.Application.Services.Dto;
using HRMv2.Manager.Common.Dto;
using HRMv2.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HRMv2.Constants.Enum.HRMEnum;

namespace HRMv2.Manager.SalaryRequestEmployees.Dto
{
    public class GetRequestEmployeeDto : BaseEmployeeDto
    {
        public long? SalaryChangeRequestId { get; set; }
        public long EmployeeId { get; set; }
        public long FromLevelId { get; set; }
        public string LevelName { get; set; }
        public long ToLevelId { get; set; }
        public string ToLevelName { get; set; }
        public UserType FromUserType { get; set; }
        public UserType ToUserType { get; set; }
        public long FromJobPositionId { get; set; }
        public string JobPositionName { get; set; }
        public long ToJobPositionId { get; set; }
        public string ToJobPositionName { get; set; }
        public double Salary { get; set; }
        public double ToSalary { get; set; }
        public DateTime ApplyDate { get; set; }
        public string Note { get; set; }
        public SalaryRequestType Type { get; set; }
        public DateTime UpdatedTime { get; set; }
        public string UpdatedUser { get; set; }
        public bool HasContract { get; set; }
        public int ContractPeriod { get; set; }
        public string TypeName => Enum.GetName(typeof(SalaryRequestType), Type);
        public string FromUserTypeName => CommonUtil.GetUserTypeNameVN(FromUserType);
        public string ToUserTypeName => CommonUtil.GetUserTypeNameVN(ToUserType);
        public string SalaryChangeRequestName { get; set; }
        public DateTime ApplyMonth { get; set; }
    }
}
