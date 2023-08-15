using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMv2.Manager.ChangeEmployeeWorkingStatuses.Dto
{
    public class InputToUpdateUserStatusDto
    {
        public string EmailAddress { get; set; }

        public DateTime DateAt { get; set; }
    }
}
