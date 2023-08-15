using HRMv2.Manager.EmployeeContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HRMv2.Constants.Enum.HRMEnum;

namespace HRMv2.Manager.SalaryRequests.Dto
{
    public class GetSalaryChangeHistoryDto
    {
        public long Id { get; set; }
        public long? SalaryChangeRequestId { get; set; }
        public long EmployeeId { get; set; }
        public long FromLevelId { get; set; }
        public string LevelName { get; set; }
        public long ToLevelId { get; set; }
        public string ToLevelName { get; set; }
        public long FromUserTypeId { get; set; }
        public string UserTypeName { get; set; }
        public long ToUserTypeId { get; set; }
        public string ToUserTypeName { get; set; }
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
        public string? ContractCode { get; set; }        
        public GetSalaryRequestDto? SalaryChangeRequest { get; set; }
        public int RequestStatus { get; set; }
        public string TypeName { 
            get 
            {
                if(Enum.IsDefined(typeof(SalaryRequestType), Type)) 
                {
                    return Enum.GetName<SalaryRequestType>(Type);
                }
                else
                {
                    return "";
                }
            } 
        }
    }
}
