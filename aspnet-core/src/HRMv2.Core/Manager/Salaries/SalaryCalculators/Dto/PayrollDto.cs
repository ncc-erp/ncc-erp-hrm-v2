using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HRMv2.Constants.Enum.HRMEnum;

namespace HRMv2.Manager.Salaries.SalaryCalculators.Dto
{
    public class PayrollDto
    {
        public long Id { get; set; }
        public DateTime ApplyMonth { get; set; }
        public PayrollStatus Status { get; set; }
        public float NormalWorkingDay { get; set; }
        public int OpenTalk { get; set; }
        public float OpentalkDayValue
        {
            get
            {
                return OpenTalk * 0.5f;
            }
        }

        public float StandardDay => this.NormalWorkingDay + this.OpentalkDayValue;
    }
}
