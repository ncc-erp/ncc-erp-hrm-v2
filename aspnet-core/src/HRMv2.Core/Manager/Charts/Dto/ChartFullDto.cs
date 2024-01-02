using Abp.AutoMapper;
using HRMv2.Manager.ChartDetails.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMv2.Manager.Charts.Dto
{
    [AutoMap(typeof(ChartDto))]
    public class ChartFullDto : ChartDto
    {
        public List<ChartDetailFullDto> ChartDetails { get; set; }

        public ChartFullDto()
        {
            ChartDetails = new List<ChartDetailFullDto>();
        }
    }
}
