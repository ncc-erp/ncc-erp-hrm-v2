using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMv2.Manager.Employees.Dto
{
    public class GetEmployeeToSelect
    {
        public string Email { get; set; }
        public string Name => Email.Split('@')[0];
        public long Value {  get; set; }
    }
    public class GetEmployeeExceptStausQuit
    {
        public string Email { get; set; }
        public string FullName { get; set; }
        public long Id { get; set; }
    }
}
