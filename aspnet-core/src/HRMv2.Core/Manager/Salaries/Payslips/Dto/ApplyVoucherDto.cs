using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMv2.Manager.Salaries.Payslips.Dto
{
    public class InputApplyVoucherDto
    {
        public string Email { get; set; }
        public double VoucherValue { get; set; }
    }

    public class ResponseApplyVoucherDto
    {
        public string Email { get; set; }
        public double RemainVoucherValue { get; set; }
    }
}
