using Abp.Application.Services.Dto;
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
    public class UpdateRefundDto:EntityDto<long>
    {
        public string Name { get; set; }
        public DateTime Date { get; set; }
        public bool IsActive { get; set; }
        public bool UpdateNote { get; set; }
    }
}
