using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMv2.Manager.Benefits.Dto
{
    public class UpdateEmployeeEndDateDto
    {
        public long BenefitId { get; set; }
        public DateTime? Date { get; set; }
    }
}
