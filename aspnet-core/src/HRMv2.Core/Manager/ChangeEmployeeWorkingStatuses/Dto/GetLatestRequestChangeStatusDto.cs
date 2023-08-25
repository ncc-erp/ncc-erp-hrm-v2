using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMv2.Manager.ChangeEmployeeWorkingStatuses.Dto
{
    public class GetLastestRequestChangeStatusDto
    {
        public long EmployeeId { get; set; }
        public DateTime CreationTime { get; set; }
        public long JobId { get; set; }
    }
}
