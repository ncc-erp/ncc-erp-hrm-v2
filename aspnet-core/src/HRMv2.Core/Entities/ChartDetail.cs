using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HRMv2.Constants.Enum.HRMEnum;

namespace HRMv2.Entities
{
    public class ChartDetail : NccAuditEntity
    {
        public long ChartId { get; set; }

        [ForeignKey(nameof(ChartId))]
        public Chart Chart { get; set; }

        [Required]
        public string Name { get; set; }

        public string Color { get; set; }

        public bool IsActive { get; set; } = true;

        public string JobPositionIds { get; set; }
        public string LevelIds { get; set; }
        public string BranchIds { get; set; }
        public string TeamIds { get; set; }
        public string UserTypes { get; set; }
        public string PayslipDetailTypes { get; set; }
        public string WorkingStatuses { get; set; }
        public string Gender { get; set; }

    }
}
