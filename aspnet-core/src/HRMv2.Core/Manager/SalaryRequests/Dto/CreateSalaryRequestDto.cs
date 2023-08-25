using Abp.AutoMapper;
using HRMv2.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HRMv2.Constants.Enum.HRMEnum;

namespace HRMv2.Manager.SalaryRequests.Dto
{
    [AutoMapTo(typeof(SalaryChangeRequest))]
    public class CreateSalaryRequestDto
    {
        public string Name { get; set; }
        public DateTime ApplyMonth { get; set; }
    }
}
