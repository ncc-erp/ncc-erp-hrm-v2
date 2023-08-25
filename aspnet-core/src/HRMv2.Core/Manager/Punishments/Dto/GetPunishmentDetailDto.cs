using HRMv2.Entities;
using HRMv2.Manager.Common.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HRMv2.Constants.Enum.HRMEnum;

namespace HRMv2.Manager.Punishments.Dto
{

    public class GetPunishmentDetailDto: BaseEmployeeDto
    {
        public long PunishmentId { get; set; }
        public long EmployeeId { get; set; }
        public double Money { get; set; }
        public string Note { get; set; }
        public DateTime? UpdatedTime { get; set; }
        public string UpdatedUser { get; set; }


    }
}
