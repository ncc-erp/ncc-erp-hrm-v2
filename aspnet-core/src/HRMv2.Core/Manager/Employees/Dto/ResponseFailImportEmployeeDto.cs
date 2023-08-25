using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMv2.Manager.Employees.Dto
{
    public class ResponseFailImportEmployeeDto
    {
        public int Row { get; set; }
        public string Email { get; set; }
        public string ReasonFail { get; set; }
    }
}
