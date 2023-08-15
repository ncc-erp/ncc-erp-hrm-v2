using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HRMv2.Constants.Enum.HRMEnum;

namespace HRMv2.Manager.Salaries.Payslips.Dto
{
    public class CollectBenefitForPayslipDetailDto
    {
        public long EmployeeId { get; set; }
        public BenefitType BenefitType { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public double Money { get; set; }
        public string Note { get; set; }
        public long? ReferenceId { get; set; }
    }
}
