using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HRMv2.Constants.Enum.HRMEnum;

namespace HRMv2.Manager.Categories.Charts.ChartDetails.Dto
{
    public class ChartDetailSelectionDto
    {
        public List<KeyValueDto> JobPositions { get; set; }
        public List<KeyValueDto> Branches { get; set; }
        public List<KeyValueDto> Levels { get; set; }
        public List<KeyValueDto> Teams { get; set; }
        public List<KeyValueDto> UserTypes { get; set; }
        public List<KeyValueDto> PayslipDetailTypes { get; set; }
        public List<KeyValueDto> Gender { get; set; }
        public List<KeyValueDto> WorkingStatuses { get; set; }

    }
}
