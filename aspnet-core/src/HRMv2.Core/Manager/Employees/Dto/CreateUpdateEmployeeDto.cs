using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using HRMv2.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HRMv2.Constants.Enum.HRMEnum;

namespace HRMv2.Manager.Employees.Dto
{
    [AutoMapTo(typeof(Employee))]
    public class CreateUpdateEmployeeDto :EntityDto<long>
    {
        [StringLength(256)]
        public string FullName { get; set; }
        [StringLength(256)]
        [DataType(DataType.EmailAddress, ErrorMessage = "Email not valid")]
        public string Email { get; set; }
        public Sex Sex { get; set; }
        [StringLength(20)]
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
        public UserType UserType { get; set; }
        public long JobPositionId { get; set; }
        public long LevelId { get; set; }
        public long BranchId { get; set; }
        public List<long> Skills { get; set; }
        public List<long> Teams { get; set; }
        public EmployeeStatus Status { get; set; }
        public float RemainLeaveDay { get; set; }
        public double Salary { get; set; }
        public double RealSalary { get; set; }
        public double ProbationPercentage { get; set; }
        [StringLength(256)]
        public string Avatar { get; set; }
        [StringLength(100)]
        public string TaxCode { get; set; }
        public string ContractCode { get; set; }
        public InsuranceStatus InsuranceStatus { get; set; }
        public DateTime StartWorkingDate { get; set; }
        public DateTime ContractStartDate { get; set; }
        public DateTime? ContractEndDate { get; set; }
        public string PersonalEmail { get; set; }
    }
}
