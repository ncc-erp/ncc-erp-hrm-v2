using HRMv2.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMv2.Manager.Salaries.Payslips.Dto
{
    public class PayslipDateDto
    {
        public List<PayslipDateDetailDto> Details { get; set; }
        public double NormalSalary => this.Details.Sum(s => s.RealDateNormalSalary);
        public double OTSalary => this.Details.Sum(s => s.OTSalary);
        public double TotalSalary => this.NormalSalary + this.OTSalary;

        /// <summary>        
        /// including Opentalk
        /// </summary>
        public float TotalWorkingDay => this.Details.Sum(s => 1 - s.OffDateValue);
        public double RoundNormalSalary => CommonUtil.RoundMoneyVND(NormalSalary);
        public double RoundOTSalary => CommonUtil.RoundMoneyVND(OTSalary);

    }

    public class PayslipDateDetailDto
    {
        public DateTime Date { get; set; }
        public double DateSalary { get; set; }

        /// <summary>
        /// Ngày có thể làm việc bình thường
        /// được bù phép cho ngày này
        /// User bđ làm việc ngày 14/4 và nghỉ ngày 25/4 => ngày 1->13/4; 25->30/4 ko phải NormalWorkingDate
        /// </summary>
        public bool IsNormalWorkingDate { get; set; }
        public float OffDateValue { get; set; }
        public float RefundOffDateValue { get; set; }
        public float DateValueForCaculate => 1 - this.OffDateValue + this.RefundOffDateValue;
        public double RealDateNormalSalary => this.DateSalary * this.DateValueForCaculate;

        public float OTHour { get; set; }
        public double OTSalary => this.DateSalary * this.OTHour / 8;

    }
}
