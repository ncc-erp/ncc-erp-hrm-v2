using Abp.Application.Services.Dto;
using Abp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HRMv2.Constants.Enum.HRMEnum;

namespace HRMv2.Manager.Salaries.Payslips.Dto
{
    public class UpdatePayslipInfoDto:EntityDto<long>
    {
        public float RemainLeaveDayBefore { get; set; }//payslip
        public float AddedLeaveDay { get; set; }//payslip
        public float NormalDay { get; set; }//payslip
        public int OpentalkCount { get; set; }//payslip
        public float OffDay { get; set; }//payslip
        public float OTHour { get; set; }//đã nhân hệ số
        public float RefundLeaveDay { get; set; }//payslip
        public float RemainLeaveDayAfter { get; set; }//payslip
        public double NormalSalary { get; set; }
        public double OTSalary { get; set; }
    }
}
