using Abp.AutoMapper;
using HRMv2.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMv2.Manager.Refunds.Dto
{
    [AutoMapTo(typeof(RefundEmployee))]
    public class AddEmployeeToRefundDto
    {
        public long RefundId { get; set; }
        public long EmployeeId { get; set; }
        public double Money { get; set; }
        public string Note { get; set; }
    }
}
