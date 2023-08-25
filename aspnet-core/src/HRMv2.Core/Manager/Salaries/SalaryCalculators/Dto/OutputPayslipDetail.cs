using Abp.AutoMapper;
using HRMv2.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HRMv2.Constants.Enum.HRMEnum;

namespace HRMv2.Manager.Salaries.SalaryCalculators.Dto
{

    [AutoMapTo(typeof(PayslipDetail))]
    public class OutputPayslipDetail
    {
        public double Money { get; set; }
        public string Note { get; set; }
        public bool IsProjectCost { get; set; }
        public PayslipDetailType Type { get; set; }
        public bool IsExpired { get; set; } = false;
        public long? ReferenceId { get; set; }
    }
}
