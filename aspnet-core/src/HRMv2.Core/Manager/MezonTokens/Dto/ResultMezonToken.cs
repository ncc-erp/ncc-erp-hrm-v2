using NccCore.Paging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMv2.Manager.MezonTokens.Dto
{
    public class ResultMezonToken
    {
        public GridResult<MezonTokenDto> Result { get; set; }
        public long TotalAmout { get; set; }    
    }
}
