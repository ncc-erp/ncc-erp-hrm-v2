using Abp.AutoMapper;
using HRMv2.Manager.Categories.Charts.ChartDetails.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMv2.Manager.Categories.Charts.Dto
{
    [AutoMap(typeof(ChartDto))]
    public class ChartFullInfoDto : ChartDto
    {
        public List<ChartDetailFullDto> ChartDetails { get; set; }

        public ChartFullInfoDto()
        {
            ChartDetails = new List<ChartDetailFullDto>();
        }
    }
}
