using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMv2.Manager.Punishments.Dto
{

    public class ResponseApiPunishmentDto
    {
        public List<EmailMoneyDto> Result { get; set; }
        public string Error { get; set; }
    }


    public class EmailMoneyDto
    {
        public string Email { get; set; }
        public double Money { get; set; }
    }

  
}
