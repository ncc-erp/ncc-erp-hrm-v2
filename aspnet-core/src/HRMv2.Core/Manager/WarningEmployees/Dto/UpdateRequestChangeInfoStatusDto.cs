using NccCore.Paging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HRMv2.Constants.Enum.HRMEnum;

namespace HRMv2.Manager.WarningEmployees.Dto
{
    public class RejectChangeInfoDto
    {
        public long Id { get; set; }
    }

    public class ApproveChangeInfoDto
    {
        public long Id { get; set; }
        public long EmployeeId { get; set; }
        public RequestStatus RequestStatus { get; set; }
        public string Phone { get; set; }
        public DateTime? Birthday { get; set; }
        public string IdCard { get; set; }
        public long? BankId { get; set; }
        public string BankAccountNumber { get; set; }
        public DateTime? IssuedOn { get; set; }
        public string IssuedBy { get; set; }
        public string PlaceOfPermanent { get; set; }
        public string Address { get; set; }
        public InsuranceStatus InsuranceStatus { get; set; }
        public string TaxCode { get; set; }
    }


    public class InputMultiFilterRequestDto
    {
        public List<RequestStatus> RequestStatuses { get; set; }
        public GridParam GridParam { get; set; }
    }
}
