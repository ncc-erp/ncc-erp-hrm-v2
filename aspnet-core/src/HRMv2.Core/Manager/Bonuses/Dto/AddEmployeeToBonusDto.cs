using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using HRMv2.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMv2.Manager.Bonuses.Dto
{
    [AutoMapTo(typeof(BonusEmployee))]
    public class AddEmployeeToBonusDto : EntityDto<long>
    {
        public long BonusId { get; set; }
        [Required]
        public double Money { get; set; }
        public string Note { get; set; }
        public List<long> EmployeeIds { get; set; }
    }
}
