using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMv2.Manager.Salaries.Payslips.Dto
{
    public class GeneratePayslipResultDto
    {
        public List<GenerateErrorDto> ErrorList { get; set; }
        public  List<long> PayslipIds { get; set; }
    }

    public class GenerateErrorDto
    {
        public string Message { get; set; }
        public long ReferenceId { get; set; }
    }
}
