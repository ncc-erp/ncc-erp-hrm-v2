using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMv2.Manager.EmployeeContracts.Dto
{
    public class ContractFileDto
    {
        public long ContractId { get; set; }
        public IFormFile File { get; set; }
    }

    public class ExportInputDto
    {
        public string Html { get; set; }
    }
}
