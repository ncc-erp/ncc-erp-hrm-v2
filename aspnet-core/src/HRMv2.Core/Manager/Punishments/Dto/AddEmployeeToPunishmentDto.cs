using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using HRMv2.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMv2.Manager.Punishments.Dto
{
    [AutoMapTo(typeof(PunishmentEmployee))]
    public class AddEmployeeToPunishmentDto : EntityDto<long>
    {
        [Required]
        public long  EmployeeId { get; set; }
        [Required]
        public double Money { get; set; }
        [Required]
        public long PunishmentId { get; set; }
        public string Note { get; set; }
    }

    [AutoMapTo(typeof(PunishmentEmployee))]
    public class UpdateEmployeeInPunishmentDto : EntityDto<long>
    {
        public long EmployeeId { get; set; }
        public double Money { get; set; }
        public string Note { get; set; }
        public long PunishmentId { get; set; }
    }
}
