using Abp.AutoMapper;
using NccCore.Anotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMv2.Manager.ChartDetails.Dto
{
    [AutoMap(typeof(ChartDetailDto))]
    public class ChartDetailFullDto : ChartDetailSelectionDto
    {
        public long Id { get; set; }

        public long ChartId { get; set; }

        public string Name { get; set; }

        public string Color { get; set; }

        public bool IsActive { get; set; }
    }
}
