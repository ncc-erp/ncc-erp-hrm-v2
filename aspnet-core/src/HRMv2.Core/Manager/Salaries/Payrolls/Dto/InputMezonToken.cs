using NccCore.Paging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMv2.Manager.Salaries.Payrolls.Dto
{
    public class InputMezonToken
    {
        public List<long> PayrollIds { set; get; }
        public GridParam GridParam { set; get; }
    }
}
