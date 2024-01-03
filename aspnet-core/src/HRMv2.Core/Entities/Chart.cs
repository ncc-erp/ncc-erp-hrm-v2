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
        public string Name { get; set; }

        public ChartType ChartType { get; set; }

        public ChartDataType ChartDataType { get; set; }

        public TimePeriodType TimePeriodType { get; set; }

        public bool IsActive { get; set; } = true;

        public ICollection<ChartDetail> ChartDetails { get; set; } // không sử dụng virtual => eager loading
    }
}
