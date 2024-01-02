using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HRMv2.Constants.Enum.HRMEnum;

namespace HRMv2.Manager.ChartDetails.Dto
{
    public class ChartDetailSelectionDto
    {
        public List<BaseInfoDto> JobPositions {  get; set; }
        public List<BaseInfoDto> Branches { get; set; }
        public List<BaseInfoDto> Levels { get; set; }
        public List<BaseInfoDto> Teams { get; set; }
        public List<EnumKeyValueDto<UserType>> UserTypes { get; set; }
        public List<EnumKeyValueDto<PayslipDetailType>> PayslipDetailTypes { get; set; }
        public List<EnumKeyValueDto<Sex>> Sexes { get; set; }
        public List<EnumKeyValueDto<EmployeeStatus>> WorkingStatuses { get; set; }

    }
}
