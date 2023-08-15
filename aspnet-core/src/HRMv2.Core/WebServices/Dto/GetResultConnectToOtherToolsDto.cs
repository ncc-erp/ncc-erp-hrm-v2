using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMv2.WebServices.Dto
{
    public class GetResultConnectDto
    {
        public bool IsConnected { get; set; }
        public string Message { get; set; }
    }

    public class GetResultConnectAllToolsDto
    {
        public GetResultConnectDto FinfastService { get; set; }
        public GetResultConnectDto ProjectService { get; set; }
        public GetResultConnectDto TimesheetService { get; set; }
        public GetResultConnectDto IMSService { get; set; }
        public GetResultConnectDto TalentService { get; set; }
    }
}
