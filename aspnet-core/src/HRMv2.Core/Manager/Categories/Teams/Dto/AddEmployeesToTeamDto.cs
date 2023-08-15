using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMv2.Manager.Categories.Teams.Dto
{
    public class AddEmployeesToTeamDto
    {
        public List<long> EmployeeIds { get; set; }
        public long TeamId { get; set; }
    }
}
