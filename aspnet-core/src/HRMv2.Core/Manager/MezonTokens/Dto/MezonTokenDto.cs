using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using HRMv2.Entities;
using HRMv2.Manager.Common.Dto;
using NccCore.Anotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HRMv2.Constants.Enum.HRMEnum;

namespace HRMv2.Manager.MezonTokens.Dto
{
    [AutoMap(typeof(MezonToken))]
    public class MezonTokenDto : BaseEmployeeDto
    {

        [ApplySearch]
        public string Note { get; set; }
        public DateTime? SentToEmployeeAt { get; set; }
        public long EmployeeId { get; set; }
        public long Amount {  get; set; }
        public StatusSendToken StatusToken {  get; set; }
        public long ReferenceId { get; set; }
        public int? TenantId { get; set; }
        public long? CurrentUserLoginId { get; set; }
        public long? PayrollId { get; set; }
        public string PayrollName => PayrollApplyMonth.HasValue ? PayrollApplyMonth.Value.ToString("MM-yyyy") : "";
        public DateTime? PayrollApplyMonth { get; set; }

    }
}
