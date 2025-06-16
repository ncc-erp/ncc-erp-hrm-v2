using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using HRMv2.Entities;
using HRMv2.Utils;
using NccCore.Anotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HRMv2.Constants.Enum.HRMEnum;

namespace HRMv2.Manager.Payrolls.Dto
{
    [AutoMapTo(typeof(Payroll))]
    public class GetPayrollDto : EntityDto<long>
    {
        [ApplySearch]
        public DateTime ApplyMonth { get; set; }
        public PayrollStatus Status { get; set; }
        public int StandardOpentalk { get; set; }
        public double StandardWorkingDay { get; set; }
        public string StatusName => CommonUtil.GetPayRollStatusName(Status);
    }
}
