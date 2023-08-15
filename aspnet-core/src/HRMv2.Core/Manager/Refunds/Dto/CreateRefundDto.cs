using Abp.AutoMapper;
using HRMv2.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMv2.Manager.Refunds.Dto
{
    [AutoMapTo(typeof(Refund))]
    public class CreateRefundDto
    {
        public string Name { get; set; }
        public DateTime Date { get; set; }
    }
}
