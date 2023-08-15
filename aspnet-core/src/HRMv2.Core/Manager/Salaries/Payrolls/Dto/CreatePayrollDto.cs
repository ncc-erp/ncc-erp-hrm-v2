using Abp.AutoMapper;
using HRMv2.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HRMv2.Constants.Enum.HRMEnum;

namespace HRMv2.Manager.Payrolls.Dto
{
    public class CreatePayrollDto
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public int StandardOpenTalk { get; set; }
    }
}
