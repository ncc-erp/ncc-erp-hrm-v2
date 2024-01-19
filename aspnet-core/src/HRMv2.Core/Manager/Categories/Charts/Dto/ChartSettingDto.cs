using HRMv2.Manager.Categories.Charts.ChartDetails.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMv2.Manager.Categories.Charts.Dto
{
    public class ChartSettingDto : ChartDto
    {
        public List<ChartDetailDto> ChartDetails { get; set; }

    }
}
