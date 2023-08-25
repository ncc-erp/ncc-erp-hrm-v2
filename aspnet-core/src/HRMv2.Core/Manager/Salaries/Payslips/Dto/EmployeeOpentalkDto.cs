using HRMv2.WebServices.Timesheet.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMv2.Manager.Salaries.Payslips.Dto
{
    public class EmployeeOpentalkDto : ChamCongInfoDto
    {
        public long EmployeeId { get; set; }
        public int TotalOpenTalkCount
        {
            get
            {
                return OpenTalkDates.Count;
            }
        }
    }
}
