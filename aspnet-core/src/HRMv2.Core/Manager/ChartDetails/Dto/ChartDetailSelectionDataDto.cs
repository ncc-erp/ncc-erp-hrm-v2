using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HRMv2.Constants.Enum.HRMEnum;

namespace HRMv2.Manager.ChartDetails.Dto
{
    public class ChartDetailSelectionDataDto
    {
        public List<BaseInfoDto> JobPositions {  get; set; }
        public List<BaseInfoDto> Branches { get; set; }
        public List<BaseInfoDto> Levels { get; set; }
        public List<BaseInfoDto> Teams { get; set; }
        public List<BaseInfoDto> UserTypes { get; set; }
        public List<BaseInfoDto> PayslipDetailTypes { get; set; }
        public List<BaseInfoDto> WorkingStatuses { get; set; }

    }
}
