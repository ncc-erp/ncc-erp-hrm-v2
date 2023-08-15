using Abp.AutoMapper;
using HRMv2.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMv2.Manager.Bonuses.Dto
{
    [AutoMapTo(typeof(BonusEmployee))]
    public class AddBonusForEmployeeDto
    {
        public long EmployeeId { get; set; }
        public long BonusId { get; set; }
        public double Money { get; set; }
        public string Note { get; set; }
    }
}
