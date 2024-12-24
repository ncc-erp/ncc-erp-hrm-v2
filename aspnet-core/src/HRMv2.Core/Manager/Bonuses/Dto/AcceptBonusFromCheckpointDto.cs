using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMv2.Manager.Bonuses.Dto
{
    public class AcceptBonusFromCheckpointDto : EntityDto<long>
    {
        public string Name { get; set; }
        public DateTime ApplyMonth { get; set; }
        public bool IsActive { get; set; }
        public int numberMonth { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public List<EmployeeInBonusDto> BonusEmployees { get; set; }
    }
}
