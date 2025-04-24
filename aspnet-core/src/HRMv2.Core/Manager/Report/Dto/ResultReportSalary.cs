using NccCore.Paging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMv2.Manager.Report.Dto
{
    public class ResultReportSalary
    {
       public List<string> Payroll {  get; set; }
       public GridResult<ReportSalaryDto> Result { get; set; }

       public List<ResultReport> ResultReport { get; set; }
    }
}
