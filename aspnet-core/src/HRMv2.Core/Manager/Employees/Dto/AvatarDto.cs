using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMv2.Manager.Employees.Dto
{
    public class AvatarDto
    {
        public IFormFile File { get; set; }
        public long EmployeeId { get; set; }
    }
}