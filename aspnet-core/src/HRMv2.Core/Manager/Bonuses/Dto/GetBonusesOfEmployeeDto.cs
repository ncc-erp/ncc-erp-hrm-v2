using Abp.Application.Services.Dto;
using NccCore.Anotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMv2.Manager.Bonuses.Dto
{
    public class GetBonusesOfEmployeeDto :EntityDto<long>
    {
        public long BonusId { get; set; }
        [ApplySearch]
        public string BonusName { get; set; }
        public double Money { get; set; }
        public bool IsActive { get; set; }
        public DateTime ApplyMonth { get; set; }
        public string Note { get; set; }
        public DateTime? UpdatedTime { get; set; }
        public string UpdatedUser { get; set; }

    }
}
