using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMv2.Manager.Benefits.Dto
{
    public class QuickAddEmployeeDto
    {
        public long BenefitId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public long EmployeeId { get; set; }
    }
}
