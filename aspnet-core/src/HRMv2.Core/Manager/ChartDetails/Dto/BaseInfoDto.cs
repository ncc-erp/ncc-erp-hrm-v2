using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMv2.Manager.ChartDetails.Dto
{
    public class BaseInfoDto : EntityDto<long>
    {
        public string Name { get; set; }
    }
}
