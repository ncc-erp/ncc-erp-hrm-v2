using HRMv2.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMv2.Manager.Notifications.Email.Dto
{
    public class EmployeeContractTemplateDto
    {
        public string EmployeeFullName { get; set; }
        public string EmployeePosition { get; set; }
        public string EmployeeBranch { get; set; }
        public string EmployeeProbationPercentage { get; set; }
        public string EmployeeBasicSalary { get; set; }
        public string EmployeeIssuedBy { get; set; }
        public string EmployeeIdCard { get; set; }
        public string ContractStartDate { get; set; }
        public string ContractEndDate { get; set; }
        public string ContractCode { get; set; }
        public string EmployeeIssuedOn { get; set; }
        public string EmployeeBirthday { get; set; }
        public string CompanyAddress { get; set; }
        public string CEOFullName { get; set; }
        public string SendToEmail { get; set; }

        public string EmployeePlaceOfResidence { get; set; }
        public string EmployeePhone { get; set; }

    }

    public class ConfidentialityContractDto : EmployeeContractTemplateDto
    {
        public string Date => CommonUtil.GetNow().Date.Day.ToString();
        public string Month => CommonUtil.GetNow().Month.ToString();
        public string Year => CommonUtil.GetNow().Year.ToString();

        public string CompanyTaxCode { get; set; }
        public string CompanyPhone { get; set; }
        public string Subject => $"[NCC][{EmployeeFullName}] HỢP ĐỒNG BẢO MẬT";


    }

    public class TrainingContractDto : EmployeeContractTemplateDto
    {
        public string Date => CommonUtil.GetNow().Date.Day.ToString();
        public string Month => CommonUtil.GetNow().Month.ToString();
        public string Year => CommonUtil.GetNow().Year.ToString();
        public string Subject => $"[NCC][{EmployeeFullName}] HỢP ĐỒNG ĐÀO TẠO";
    }
    
    public class ProbationaryContractDto : EmployeeContractTemplateDto
    {
        public string Subject => $"[NCC][{EmployeeFullName}] HỢP ĐỒNG THỬ VIỆC";
    }

    public class CollaboratorContractDto : EmployeeContractTemplateDto
    {
        public string Subject => $"[NCC][{EmployeeFullName}] HỢP ĐỒNG CỘNG TÁC VIÊN";
    }

    public class LaborContractDto : EmployeeContractTemplateDto
    {
        public string Subject => $"[NCC][{EmployeeFullName}] HỢP ĐỒNG LAO ĐỘNG";
    }

    public class CompanyInfoDto
    {
        public string CEOFullName { get; set; }
        public string CompanyAddress { get; set; }

        public string BranchName { get; set; }
    }


    public class GetEmployeeContractDataDto
    { 

        public DateTime? ContractStartDate { get; set; }
        public DateTime? ContractEndDate { get; set; }
        public string ContractCode { get; set; }
        public string Phone { get; set; }
        public string FullName { get; set; }
        public DateTime? Birthday { get; set; }
        public string IdCard { get; set; }
        public DateTime? IssuedOn { get; set; }
        public string IssuedBy { get; set; }
        public string PlaceOfPermanent { get; set; }
        public string Address { get; set; }
        public double BasicSalary { get; set; }
        public double ProbationPercentage { get; set; }
        public string JobPositionName { get; set; }
        public string BranchName { get; set; }
        public long? CEOId { get; set; }
        public string CEOFullName { get; set; }
        public string CompanyAddress { get; set; }
        public string CompanyPhone { get; set; }
        public string CompanyTaxCode { get; set; }

    }
}
