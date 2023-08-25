using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using HRMv2.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HRMv2.Constants.Enum.HRMEnum;

namespace HRMv2.Manager.Salaries.Payslips.Dto
{
    public class GetSalaryDetailDto
    {
        public long PayslipId { get; set; }
        public double Money { get; set; }
        public string Note { get; set; }
        public PayslipDetailType Type { get; set; }
        public bool IsProjectCost { get; set; }
        public string TypeName => Enum.GetName(typeof(PayslipDetailType), this.Type);
    }

    public class GetPayslipDetailByTypeDto : EntityDto<long>
    {
        public long PayslipId { get; set; }
        public double Money { get; set; }
        public string Note { get; set; }
        public PayslipDetailType Type { get; set; }
        public DateTime? UpdatedTime { get; set; }
        public string UpdatedUser { get; set; }
    }

    [AutoMapTo(typeof(PayslipDetail))]
    public class CreatePayslipDetailDto: EntityDto<long>
    {
        public long PayslipId { get; set; }
        public double Money { get; set; }
        public string Note { get; set; }
        public PayslipDetailType Type { get; set; }
        public bool IsProjectCost { get; set; }    
    }

    public class CreatePayslipDetailPunishmentDto:CreatePayslipDetailDto
    {
        /// <summary>
        /// PunishmentId
        /// </summary>
        public long PunishmentId { get; set; }
    }

    public class CreatePayslipBonusDto : CreatePayslipDetailDto
    {
        public long BonusId { get; set; }
    }

    [AutoMapTo(typeof(PayslipDetail))]
    public class UpdatePayslipDetailDto: EntityDto<long>
    {
        public double Money { get; set; }
        public string Note { get; set; }
    }
}
