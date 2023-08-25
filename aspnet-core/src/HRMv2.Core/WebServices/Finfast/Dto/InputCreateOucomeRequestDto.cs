using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMv2.WebServices.Finfast.Dto
{
    public class InputCreateOucomeRequestDto
    {
        public string Name { get; set; }
        public List<OutcomingEntryDetailDto> Details { get; set; }
    }
    public class OutcomingEntryDetailDto
    {
        public string Name { get; set; }
        public double UnitPrice { get; set; }
        public string BranchCode { get; set; }
    }
}
