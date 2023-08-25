using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using HRMv2.Entities;
using NccCore.Anotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMv2.Manager.Refunds.Dto
{
    public class GetRefundDto:EntityDto<long>
    {
        [ApplySearch]
        public string Name { get; set; }
        public DateTime Date { get; set; }
        public int EmployeeCount { get; set; }
        public double TotalMoney { get; set; }
        public bool IsActive { get; set; }
    }
}
