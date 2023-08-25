using HRMv2.Utils;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HRMv2.Constants.Enum.HRMEnum;

namespace HRMv2.Manager.Employees.Dto
{
    public class ImportEmployeeFromFileDto: CreateUpdateEmployeeDto
    {

        public string Surname { get;set; }
        public string Name { get;set; }
        public string StatusName { get; set; } 
        public string BranchCode { get; set; }
        public string UserTypeName { get; set; }
        public string LevelCode { get; set; }
        public string SexCode { get; set; }
        public string JobPositionCode { get; set; }
        public string InsuranceStatusCode { get; set; }
        public string BankCode { get; set; }
        public int Row { get; set; }
        public EmployeeStatus Status => EmployeeStatus.Working;
        public  string FullName => Surname + " " + Name;
        public InsuranceStatus InsuranceStatus => string.IsNullOrEmpty(InsuranceStatusCode) ? InsuranceStatus.NONE : (InsuranceStatus)CommonUtil.GetValueOfInsuranceStatus(InsuranceStatusCode);
        public Sex Sex => string.IsNullOrEmpty(SexCode) ? Sex.Male : (Sex)CommonUtil.GetValueOfSex(SexCode);

    }
    public class UpdtaeEmployeeFromFileDto
    {
        public string Email { get; set; }
        public string Phone { get; set; }
        public DateTime? Birthday { get; set; }
        [StringLength(20)]
        public string IdCard { get; set; }
        public DateTime? IssuedOn { get; set; }
        [StringLength(1000)]
        public string IssuedBy { get; set; }
        [StringLength(1000)]
        public string PlaceOfPermanent { get; set; }
        [StringLength(1000)]
        public string Address { get; set; }
        public long? BankId { get; set; }
        [StringLength(256)]
        public string BankAccountNumber { get; set; }
        public string TaxCode { get; set; }
        public string ContractCode { get; set; }
        public string InsuranceStatusCode { get; set; }
        public InsuranceStatus InsuranceStatus => string.IsNullOrEmpty(InsuranceStatusCode) ? InsuranceStatus.NONE : (InsuranceStatus)CommonUtil.GetValueOfInsuranceStatus(InsuranceStatusCode);
        public string BankCode { get; set; }
        public int Row { get; set; }

    }
    public class InputFileDto
    {
        public IFormFile File { get; set; }
    }

    public class FileBase64Dto
    {
        public string FileName { get; set; }
        public string FileType { get; set; }
        public string Base64 { get; set; }
    }
}
