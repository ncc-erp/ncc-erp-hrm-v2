using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMv2.Manager.Punishments.Dto
{
    public class GetDataFromFileDto
    {
        public long EmployeeId { get; set; }
        public string Note { get; set; }
        public double Money { get; set; }
        public long PunishmentId { get; set; }

    }
}