using Abp.Application.Services.Dto;
using NccCore.Anotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HRMv2.Constants.Enum.HRMEnum;

namespace HRMv2.Manager.Punishments.Dto
{
    public class GetPunishmentsOfEmployeeDto : EntityDto<long>
    {
        public long PunishmentId { get; set; }
        [ApplySearch]
        public string PunishmentName { get; set; }
        public double Money { get; set; }
        public bool IsActive { get; set; }
        public DateTime ApplyMonth { get; set; }
        [ApplySearch]
        public string Note { get; set; }
        public DateTime? UpdatedTime { get; set; }
        public string UpdatedUser { get; set; }
    }
}
