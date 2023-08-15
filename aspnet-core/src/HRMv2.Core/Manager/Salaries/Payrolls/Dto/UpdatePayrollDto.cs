using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using HRMv2.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HRMv2.Constants.Enum.HRMEnum;

namespace HRMv2.Manager.Payrolls.Dto
{
    [AutoMapTo(typeof(Payroll))]
    public class UpdatePayrollDto : EntityDto<long>
    {
        public DateTime ApplyMonth { get; set; }
        public PayrollStatus Status { get; set; }
        public float NormalWorkingDay { get; set; }
        public int OpenTalk { get; set; }
    }
}
