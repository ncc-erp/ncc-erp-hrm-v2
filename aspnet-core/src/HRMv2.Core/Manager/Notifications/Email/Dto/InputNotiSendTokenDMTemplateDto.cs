using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMv2.Manager.Notifications.Email.Dto
{
    public class InputNotiSendTokenDMTemplateDto
    {
        public string MezonUsername { get; set; }
        public string Amount { get; set; }
        public string EmployeeFullName { get; set; }
        public string Note {  get; set; }
    }
}
