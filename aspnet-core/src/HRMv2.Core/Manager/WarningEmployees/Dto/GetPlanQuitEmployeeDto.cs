using HRMv2.Manager.Common.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HRMv2.Constants.Enum.HRMEnum;

namespace HRMv2.Manager.WarningEmployees.Dto
{
    public class GetPlanQuitEmployeeDto:BaseEmployeeDto
    {
        public long? JobId { get; set; }
        public long? WorkingHistoryId { get; set; }
        public string Name { get; set; }
        public string WorkingStatus { get; set; }
        public DateTime DateAt { get; set; }
        public DateTime CreationTime { get; set; }
        public bool? IsAbandoned { get;set; }
    }

    public class UpdatePlanToQuitEmployeeDto
    {
        public long? JobId { get; set; }
        public long? WorkingHistoryId { get; set; }
        public DateTime DateAt { get; set; }
    }
}
