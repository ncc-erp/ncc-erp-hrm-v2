using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HRMv2.Constants.Enum.HRMEnum;

namespace HRMv2.Manager.Timesheet.Dto
{
    public class UpdateUserInfoDto
    {
        public string Email { get; set; }
        public string Phone { get; set; }
        public DateTime? Birthday { get; set; }
        public string IdCard { get; set; }
        public DateTime? IssuedOn { get; set; }
        public string IssuedBy { get; set; }
        public string PlaceOfPermanent { get; set; }
        public string Address { get; set; }
        public string BankAccountNumber { get; set; }
        public string TaxCode { get; set; }
        public long? BankId { get; set; }
    }
    public class UpdateRequestEditInfoDto
    {
        public int Id { get; set; }
    }
    public class ResultUpdateInfo
    {
        public bool IsSucess { get; set; }
        public string ResultMessage { get; set; }
    }
    public class GetInfoToUPDateProfile : UpdateUserInfoDto
    {
        public RequestStatus RequestStatus { get; set; }
        public string RequestStatusName => RequestStatus != default ? RequestStatus.ToString(): "";
    }
}
