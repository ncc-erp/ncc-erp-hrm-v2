using Abp.Domain.Entities;
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
    public class TempEmployeeTS : NccAuditEntity, IMayHaveTenant
    {
        public int? TenantId { get; set; }
        public long EmployeeId { get; set; }
        [ForeignKey(nameof(EmployeeId))]
        public virtual Employee Employee { get; set; }
        public RequestStatus RequestStatus { get; set; }
        public string Phone { get; set; }
        public DateTime? Birthday { get; set; }
        [StringLength(50)]
        public string IdCard { get; set; }
        public long? BankId { get; set; }
        [ForeignKey(nameof(BankId))]
        public string BankAccountNumber { get; set; }
        public DateTime? IssuedOn { get; set; }
        [StringLength(1000)]
        public string IssuedBy { get; set; }
        [StringLength(1000)]
        public string PlaceOfPermanent { get; set; }
        [StringLength(1000)]
        public string Address { get; set; }
        [StringLength(100)]
        public string TaxCode { get; set; }
    }
}
