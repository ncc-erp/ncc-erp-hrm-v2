using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMv2.Manager.SalaryRequests.Dto
{
    public class GetEmployeeNotInRequestDto
    {
        public long EmployeeId { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
    }
}
