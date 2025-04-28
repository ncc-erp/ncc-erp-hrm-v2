using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMv2.Manager.Salaries.Payslips.Dto
{
    public class GetPayrollApply
    {
        public long Value { get; set; }
        public DateTime ApplyDate { get; set; }
        public string Name => ApplyDate.ToString("yyyy-MM");
    }
}
