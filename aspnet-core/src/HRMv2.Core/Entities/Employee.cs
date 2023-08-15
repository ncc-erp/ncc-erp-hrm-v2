using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HRMv2.Constants.Enum.HRMEnum;

namespace HRMv2.Entities
{
    public class Employee : NccAuditEntity, IMayHaveTenant
    {
        public int? TenantId { get; set; }
        [Required]
        [StringLength(256)]
        public string FullName { get; set; }
        [Required]
        [StringLength(256)]
        public string Email { get; set; }
        public Sex Sex { get; set; }
        [Required]
        [StringLength(20)]
        public string Phone { get; set; }
        public DateTime? Birthday { get; set; }
        [StringLength(50)]
        public string IdCard { get; set; }
        public DateTime? IssuedOn { get; set; }
        [StringLength(1000)]
        public string IssuedBy { get; set; }
        [StringLength(1000)]
        public string PlaceOfPermanent { get; set; }
        [StringLength(1000)]
        public string Address { get; set; }
        public long? BankId { get; set; }
        [ForeignKey(nameof(BankId))]
        public Bank Bank { get; set; }
        [StringLength(256)]
        public string BankAccountNumber { get; set; }
        public UserType UserType  { get; set; }
        public long JobPositionId { get; set; }
        [ForeignKey(nameof(JobPositionId))]
        public JobPosition JobPosition { get; set; }
        public long LevelId { get; set; }
        [ForeignKey(nameof(LevelId))]
        public Level Level { get; set; }
        public long BranchId { get; set; }
        [ForeignKey(nameof(BranchId))]
        public Branch Branch { get; set; }
        public EmployeeStatus Status { get; set; }
        public float RemainLeaveDay { get; set; }
        public double Salary { get; set; }
        public double RealSalary { get; set; }
        public double ProbationPercentage { get; set; }
        [StringLength(256)]
        public string Avatar { get; set; }
        [StringLength(100)]
        public string TaxCode { get; set; }
        public InsuranceStatus InsuranceStatus { get; set; }
        public string PersonalEmail { get; set; }
        /// <summary>
        /// Dùng để tính thâm niên
        /// Khi UserType = Staff => Không update StartWorkingDate nữa
        /// </summary>
        public DateTime StartWorkingDate { get; set; } 

        public virtual ICollection<EmployeeSkill> EmployeeSkills { get; set; }
        public virtual ICollection<EmployeeTeam> EmployeeTeams { get; set; }
    }
}
