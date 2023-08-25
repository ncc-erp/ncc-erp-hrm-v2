using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using HRMv2.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMv2.Manager.PunishmentFunds.Dto
{
    [AutoMapTo(typeof(PunishmentFund))]
    public class AddEditPunishmentFundDto: EntityDto<long>
    {
        public double Amount { get; set; }
        public DateTime Date { get; set; }
        public string Note { get; set; }
    }
}
