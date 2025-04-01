using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using HRMv2.Entities;
using NccCore.Anotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HRMv2.Constants.Enum.HRMEnum;

namespace HRMv2.Manager.PayrollTokens.Dto
{

    public class PayslipTokenDto: EntityDto<long>
    {
        public long Id { get; set; }
        [ApplySearch]
        public string EmailAddress { get; set; }
        public string Note { get; set; }
        public int Amount {  get; set; }
        public StatusSendToken Status {  get; set; }

    }
}
