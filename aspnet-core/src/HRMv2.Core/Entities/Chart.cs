using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HRMv2.Constants.Enum.HRMEnum;

namespace HRMv2.Entities
{
    public class Chart : NccAuditEntity
    {
        [Required]
        [StringLength(256)]
        public string Name { get; set; }

        [Required]
        public ChartType ChartType { get; set; }

        [Required]
        public TimePeriodType TimePeriodType { get; set; }

        public bool IsActive { get; set; } = true;

        public ICollection<ChartDetail> ChartDetails { get; set; } // not use virtual => eager loading
    }
}
