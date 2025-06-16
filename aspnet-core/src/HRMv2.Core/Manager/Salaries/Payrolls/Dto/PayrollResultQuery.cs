using HRMv2.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HRMv2.Constants.Enum.HRMEnum;

namespace HRMv2.Manager.Salaries.Payrolls.Dto
{
    public class PayrollResultQuery
    {
        public long Value { get; set; }
        public DateTime ApplyDate { get; set; }

        public PayrollStatus Status { get; set; }
        public string Name => ApplyDate.ToString("yyyy-MM");

        public string StatusName => CommonUtil.GetPayRollStatusName(Status);

    }
}
