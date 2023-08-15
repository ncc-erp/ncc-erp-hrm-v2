using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMv2.Manager.Bonuses.Dto
{
    public class ImportFileDto
    {
        public IFormFile File { get; set; }
        public long BonusId { get; set; }
    }
    public class ResponseFailDto
    {
        public int Row { get; set; }
        public string ?Email { get; set; }
        public string ?Money { get; set; }
        public string ReasonFail { get; set; }
    }
}
