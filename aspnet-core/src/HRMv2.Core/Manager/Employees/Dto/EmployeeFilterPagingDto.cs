using NccCore.Paging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMv2.Manager.Employees.Dto
{
    public class EmployeeFilterPagingDto:GridParam
    {
        public List<long> TeamsId { get; set; }
        public bool IsAndCondition { get; set; }
    }
}
