using HRMv2.Manager.Common.Dto;
using HRMv2.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HRMv2.Constants.Enum.HRMEnum;

namespace HRMv2.Manager.Timesheet.Dto
{
    public class GetUserInfoByEmailDto
    {
        public string FullName { get; set; }
        public string Email { get; set; }
        public Sex Sex { get; set; }
        public EmployeeStatus Status { get; set; }
        public UserType UserType { get; set; }
        public List<string> SkillNames { get; set; }
        public List<string> Teams { get; set; }
        public string Phone { get; set; }
        public DateTime? Birthday { get; set; }
        public string IdCard { get; set; }
        public DateTime? IssuedOn { get; set; }
        public string IssuedBy { get; set; }
        public string PlaceOfPermanent { get; set; }
        public string Address { get; set; }
        public string BankAccountNumber { get; set; }
        public float RemainLeaveDay { get; set; }
        public string TaxCode { get; set; }
        public InsuranceStatus InsuranceStatus { get; set; }
        public string Branch { get; set; }
        public string Level { get; set; }
        public string JobPosition { get; set; }
        public string Bank { get; set; }
        public string InsuranceStatusName => CommonUtil.GetInsuranceStatusName(InsuranceStatus);
        public string UsertypeName => CommonUtil.GetUserTypeNameVN(UserType);
        public string StatusName => CommonUtil.GetWorkingStatusName(Status);
        public long? BankId { get; set; }
        public List<long> TeamIds { get; set; }
    }

    public class ItemInfoDto
    {
        public string Name { get; set; }     
        public long Id { get; set; }
        public bool IsDefault => Name.ToLower().Equals("techcombank");
    }

 
}
