using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMv2.Manager.SalaryRequests.Dto
{
    public class ImportCheckpointDto
    {
        public IFormFile File { get; set; }
        public long SalaryChangeRequestId { get; set; }
    }
    public class FailResponeDto
    {
        public int Row { get; set; }
        public string Email { get; set; }
        public string ReasonFail { get; set; }
    }

    public class GetDataToImportCheckpointFromFileDto
    {
        public string Email { get; set; }
        public string LevelName { get; set; }
        public double Salary { get; set; }
        public bool HasContract { get; set; }
        public int Row { get; set; }
    }
}
