using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMv2.Manager.Benefits.Dto
{
    public class UpdateBenefitStatusDto
    {
        public bool IsActive { get; set; }
        public long Id { get; set; }
    }
}
