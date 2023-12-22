using NccCore.Anotations;
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
        [Required]
        public long ChartId { get; set; }

        [ForeignKey(nameof(ChartId))]
        public Chart Chart { get; set; }

        [Required]
        [StringLength(256)]
        public string Name { get; set; }

        public string Color { get; set; }

        public bool IsActive { get; set; } = true;

        /// <summary>
        /// PostgreSQL support List datatype => it will store in database as an array but sqlServer dont
        /// </summary>
        public List<long> JobPositionIds { get; set; }

        public List<long> LevelIds { get; set; }

        public List<long> BranchIds { get; set; }

        public List<long> TeamIds { get; set; }

        public List<UserType> UserTypes { get; set; }

        public List<PayslipDetailType> PayslipDetailTypes { get; set; }

        public List<EmployeeStatus> WorkingStatuses { get; set; }

    }
}
