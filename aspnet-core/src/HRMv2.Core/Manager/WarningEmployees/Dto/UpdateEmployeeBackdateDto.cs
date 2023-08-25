using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMv2.Manager.WarningEmployees.Dto
{
    public class UpdateEmployeeBackdateDto
    {
        public int EmployeeId { get; set; }
        public DateTime BackDate { get; set; }
    }
}
