using HRMv2.Manager.Common.Dto;
using HRMv2.Utils;
using NccCore.Anotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HRMv2.Constants.Enum.HRMEnum;

namespace HRMv2.Manager.WarningEmployees.Dto
{
    public class GetRequestUpdateInfoDto
    {
        public long Id { get; set; }
        public long EmployeeId { get; set; }
        public RequestStatus RequestStatus { get; set; }
        [ApplySearch]
        public string FullName { get; set; }
        [ApplySearch]
        public string Email { get; set; }
        public Sex Sex { get; set; }
        public BadgeInfoDto LevelInfo { get; set; }
        public BadgeInfoDto BranchInfo { get; set; }
        public BadgeInfoDto JobPositionInfo { get; set; }
        public string Avatar { get; set; }
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
        public BadgeInfoDto RequestStatusInfo => CommonUtil.GetRequestUpdateInfoStatusName(RequestStatus);
        public string AvatarFullPath => FileUtil.FullFilePath(Avatar);
        public string UpdatedUser { get; set; }
        public DateTime? UpdatedTime { get; set; }
        public DateTime? CreationTime { get; set; }

    }

    public class GetRequestDetailDto
    {
        public long Id { get; set; }
        public long EmployeeId { get; set; }
        public RequestStatus RequestStatus { get; set; }
        public string Phone { get; set; }
        public string EmployeePhone { get; set; }
        public bool IsChangePhone => Phone == EmployeePhone;
        public DateTime? Birthday { get; set; }
        public DateTime? EmployeeBirthDay { get; set; }
        public bool IsChangeBirthday => Birthday?.Date == EmployeeBirthDay?.Date;
        public string IdCard { get; set; }
        public string EmployeeIdCard { get; set; }
        public bool IsChangeIdCard => IdCard == EmployeeIdCard;
        public long? BankId { get; set; }
        public long? EmployeeBankId { get; set; }
        public bool IsChangeBankId => BankId == EmployeeBankId;
        public string BankAccountNumber { get; set; }
        public string EmployeeBankAccountNumber { get; set; }
        public bool IsChangeBankAccountNumber => BankAccountNumber == EmployeeBankAccountNumber;
        public DateTime? IssuedOn { get; set; }
        public DateTime? EmployeeIssuedOn { get; set; }
        public bool IsChangeIssuedOn => IssuedOn?.Date == EmployeeIssuedOn?.Date;
        public string IssuedBy { get; set; }
        public string EmployeeIssuedBy { get; set; }
        public bool IsChangeIssuedBy => IssuedBy == EmployeeIssuedBy;
        public string PlaceOfPermanent { get; set; }
        public string EmployeePlaceOfPermanent { get; set; }
        public bool IsChangePlaceOfPermanent => PlaceOfPermanent == EmployeePlaceOfPermanent;
        public string Address { get; set; }
        public string EmployeeAddress { get; set; }
        public bool IsChangeAddress => Address == EmployeeAddress;
        public string TaxCode { get; set; }
        public string EmployeeTaxCode { get; set; }
        public bool IsChangeTaxCode => TaxCode == EmployeeTaxCode;
        
    }
}
