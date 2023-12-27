using Abp.AutoMapper;
using HRMv2.Manager.Charts.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMv2.Manager.ChartDetails.Dto
{
    [AutoMap(typeof(ChartDto))]
    public class ChartFullDetailDto : ChartDto
    {
        public List<ChartDetailDto> ChartDetails { get; set; }
    }
}
