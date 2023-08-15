using HRMv2.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HRMv2.Constants.Enum.HRMEnum;

namespace HRMv2.Manager.Salaries.Payslips.Dto
{
    public class SumaryInfoDto
    {
        public virtual string Name { get; set; }
        public int Quantity { get; set; }
        public double TotalSalary { get; set; }
        public double AvgSalary => Quantity > 0 ? CommonUtil.RoundMoneyVND(TotalSalary / Quantity) : 0;
        public PayslipDetailType? Type { get; set; }
    }

    public class ReCalculateDto
    {
        public long PayslipId { get;set; }
    }

}
