using Abp.Application.Services.Dto;
using HRMv2.Manager.Common.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMv2.Manager.Refunds.Dto
{
    public class GetRefundEmployeeDto: BaseEmployeeDto
    {
        public long RefundId { get; set; }
        public long EmployeeId { get; set; }
        public double Money { get; set; }
        public string Note { get; set; }
        public DateTime? UpdatedTime { get; set; }
        public string UpdatedUser { get; set; }
    }
}
