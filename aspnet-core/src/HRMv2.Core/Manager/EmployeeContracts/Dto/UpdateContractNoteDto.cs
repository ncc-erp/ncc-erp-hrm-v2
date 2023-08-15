using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMv2.Manager.EmployeeContracts.Dto
{
    public class UpdateContractNoteDto
    {
        public long ContractId { get; set; }
        public string Note { get; set; }
    }
}
